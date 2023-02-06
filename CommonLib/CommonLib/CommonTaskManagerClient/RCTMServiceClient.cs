using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
//using CommonTaskManagerClient.ServiceReference1;
using com.ivp.rad.RCommonTaskManager;
using System.IO;
using System.Timers;
using com.ivp.rad.RCTMUtils;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Reflection;
using System.Data;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using com.ivp.rad.dal;
using System.Threading.Tasks;
using com.ivp.rad.utils;
using com.ivp.srmcommon;

namespace CommonTaskManagerClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RCTMServiceClient : RServiceBaseClass, ICTMServiceCallback, IInterProcessConnector
    {
        internal const string SCHEDULED_PROCESS_EXECUTABLE_PATH = "CTMProcess.exe";
        const int KEYSIZE = 1024;
        static Dictionary<string, TaskInfo> processCollection = new Dictionary<string, TaskInfo>();

        ICTMService CTMService = null;
        private System.Timers.Timer mTimer = null;
        int clientId;
        static readonly object objLock = new object();
        static StreamWriter writer = null;
        string directoryName = AppDomain.CurrentDomain.BaseDirectory + "\\CTMClientErrorLogs";
        static string debugFilename = AppDomain.CurrentDomain.BaseDirectory + "\\CTMClientErrorLogs\\ClieRntErrorLog" + DateTime.Now.ToString("MMddyy") + ".txt";
        //string directoryName = "CTMClientErrorLogs";
        //static string debugFilename = AppDomain.CurrentDomain.BaseDirectory + "\\..\\CTMClientErrorLogs\\ClieRntErrorLog" + DateTime.Now.ToString("MMddyy") + ".txt";
        DuplexChannelFactory<ICTMService> dupFactory = null;

        /// <summary>
        /// Registers with the CTMService with the 'ModuleId' key in the config file
        /// </summary>
        public RCTMServiceClient()
        {
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CTMClientRelativePath"]))
            {
                directoryName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["CTMClientRelativePath"], "CTMClientErrorLogs");
                debugFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["CTMClientRelativePath"], "CTMClientErrorLogs\\ClientErrorLog" + DateTime.Now.ToString("MMddyy") + ".txt");
            }
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            try
            {
                clientId = Convert.ToInt32(ConfigurationManager.AppSettings["CTMModuleId"]);
                initializeRCTMClient(clientId);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:ctor>>Error while creating new instance of RCTMClient", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw;
            }
        }

        public static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        /// <summary>
        /// registers with the CTMService
        /// </summary>
        /// <param name="registeredModuleId">registered module id of the client</param>
        public RCTMServiceClient(int registeredModuleId)
        {
            CreateLog(debugFilename, "registereing with module id " + registeredModuleId, this);
            initializeRCTMClient(registeredModuleId);
        }

        public RCTMServiceClient(int[] moduleIds)
        {
            foreach (int moduleId in moduleIds)
            {
                initializeRCTMClient(moduleId);
            }

        }

        public void initializeRCTMClient(int moduleId)
        {
            try
            {
                CreateLog(debugFilename, "RCTMClient:ctor>>creating new channel with CTMService", this);
                clientId = moduleId;
                dupFactory = new DuplexChannelFactory<ICTMService>(this, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                dupFactory.Faulted += new EventHandler(dupFactory_Faulted);
                CTMService = dupFactory.CreateChannel();
                CreateLog(debugFilename, "RCTMClient:ctor>>Registering with CTMService with module id:" + clientId, this);

                SRMMTConfig.MethodCallPerClient(RegisterClient, moduleId);
                
                string retryInterval = ConfigurationManager.AppSettings["CTMClientRetryInterval"];
                mTimer = new System.Timers.Timer(double.Parse(retryInterval));
                mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                mTimer.Start();
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:ctor>>Error while creating new instance of RCTMClient", this);
                CreateLog(debugFilename, ex.ToString(), this);
                //throw;
            }
        }

        private void RegisterClient(string clientName, int clientId)
        {
            SRMMTConfig.SetClientName(clientName);
            CTMService.RegisterClient(clientId, clientName);
        }

        public string keepAlive(string clientName)
        {
            return "OK";
        }

        void dupFactory_Faulted(object sender, EventArgs e)
        {
            try
            {
                CreateLog(debugFilename, "WCF channel is in faulted state, trying to recover", this);
                dupFactory.Abort();
                dupFactory.Faulted -= new EventHandler(dupFactory_Faulted);
                dupFactory = new DuplexChannelFactory<ICTMService>(this, "NetTcpBinding_ICTMService");
                dupFactory.Faulted += new EventHandler(dupFactory_Faulted);
                dupFactory.Open();
                CTMService = dupFactory.CreateChannel();
                CreateLog(debugFilename, "RCTMClient:ctor>>Registering with CTMService with module id:" + clientId, this);
                SRMMTConfig.MethodCallPerClient(RegisterClient, clientId);
                CreateLog(debugFilename, "Successful in recovering from faulted state", this);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "Exception while recoverig from faulted state: " + ex.ToString(), this);
            }

        }


        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (CTMService.isAlive("") == false)
                {
                    dupFactory = new DuplexChannelFactory<ICTMService>(this, "NetTcpBinding_ICTMService");
                    dupFactory.Open();
                    CTMService = dupFactory.CreateChannel();
                    SRMMTConfig.MethodCallPerClient(RegisterClient, clientId);
                }
                CTMService.clientKeepAlive();
            }
            catch (Exception ex)
            {
                try
                {
                    CreateLog(debugFilename, "RCTMClient:timer_elapsed>>Not connected to CTM Service.. Trying to reconnect....", this);
                    CreateLog(debugFilename, ex.ToString(), this);
                    int clientId = Convert.ToInt32(ConfigurationManager.AppSettings["CTMModuleId"]);
                    dupFactory = new DuplexChannelFactory<ICTMService>(this, "NetTcpBinding_ICTMService");
                    dupFactory.Open();
                    CTMService = dupFactory.CreateChannel();
                    SRMMTConfig.MethodCallPerClient(RegisterClient, clientId);
                    CreateLog(debugFilename, "RCTMClient:timer_elapsed>>Attempt to re-connect and register was successful", this);
                }
                catch (Exception exc)
                {
                    CreateLog(debugFilename, "RCTMClient:timer_elapsed>>Attempt to reconnect with CTMService failed", this);
                    CreateLog(debugFilename, exc.ToString(), this);
                }
            }
        }

        public void triggerTask(TaskInfo taskInfo, string guid, string clientName)
        {
            SRMMTConfig.SetClientName(clientName);
            CreateLog(debugFilename, "RCTMClient:triggerTask>>Request from CTMService received to trigger task with taskInfo:" + taskInfo.ToString() + " and guid:" + guid, this);
            lock (((ICollection)processCollection).SyncRoot)
            {
                try
                {
                    taskInfo.ClientName = clientName;
                    string dataObject = GetSerializedData(taskInfo);
                    ProcessStartInfo processInfo = CreateProcessInfoObject(SCHEDULED_PROCESS_EXECUTABLE_PATH, dataObject + " " + taskInfo.FlowID + " " + clientName);
                    CreateLog(debugFilename, "starting process", this);
                    Process newProcess = Process.Start(processInfo);
                    CreateLog(debugFilename, "starting process", this);
                    newProcess.EnableRaisingEvents = true;

                    newProcess.Exited += new EventHandler(newProcess_Exited);
                    taskInfo.ProcessId = newProcess.Id;
                }
                catch (Exception ex)
                {
                    taskInfo.Status.Status = com.ivp.rad.RCTMUtils.TaskStatus.FAILED;
                    taskInfo.Status.TaskLog = "Failed while trying to start the process with error => " + ex.ToString();
                    CTMService.TaskCompleted(taskInfo, guid,clientName);
                    CreateLog(debugFilename, "RCTMClient:triggerTask>>Error while triggering task with task info:" + taskInfo.ToString() + " and guid" + guid, this);
                    CreateLog(debugFilename, ex.ToString(), this);
                }
            }
        }

        public void UpdateJobCompletionStatus(TaskInfo info, string clientName)
        {
            SRMMTConfig.SetClientName(clientName);
            CreateLog(debugFilename, "RCTMClient:UpdateJobCompletionStatus>>Job completed with taskinfo :" + info.ToString(), this);
            try
            {
                lock (((ICollection)processCollection).SyncRoot)
                {
                    string key = string.Empty;
                    CTMService.TaskCompleted(info, info.Status.chain_guid,clientName);// key);
                    CreateLog(debugFilename, "RCTMClient:UpdateJobCompletionStatus>>Calling TaskCompleted() on CTMService with taskInfo:" + info + " and guid:" + key, this);
                }
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:UpdateJobCompletionStatus>>Error while updating job completion status with task info" + info.ToString(), this);
                CreateLog(debugFilename, ex.ToString(), this);
            }
        }

        private ProcessStartInfo CreateProcessInfoObject(string fileName, string jobInfo)
        {
            try
            {
                CreateLog(debugFilename, "RCTMClient:CreateProcessInfoObject>>create process info object with file name:" + fileName + " and jobInfo:" + jobInfo, this);
                ProcessStartInfo processInfo = new ProcessStartInfo(fileName, jobInfo);

                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;
                CreateLog(debugFilename, "location:" + AppDomain.CurrentDomain.BaseDirectory, this);
                processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardInput = true;
                processInfo.RedirectStandardOutput = true;
                return processInfo;
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:CreateProcessInfoObject>>Error while trying to create process info object with file name:" + fileName + " and jobInfo:" + jobInfo, this);
                CreateLog(debugFilename, ex.ToString(), this);
                return null;
            }
        }

        private string GetSerializedData(TaskInfo info)
        {
            try
            {
                SRMMTConfig.SetClientName(info.ClientName);
                CreateLog(debugFilename, "fetching serialized data : ", this);
                MemoryStream stringWriter = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stringWriter, info);
                stringWriter.Position = 0;
                return Convert.ToBase64String(stringWriter.ToArray());
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "Error while fetching serialized data : " + ex.ToString(), this);
                return null;
            }
        }

        void newProcess_Exited(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(((Process)sender).StandardError.ReadToEnd().Trim()))
                {
                    CreateLog(debugFilename, "RTaskManagerService=>" + ((Process)sender).StandardError.ReadToEnd().Trim(), this);
                }
                CreateLog(debugFilename, "Error in new process exited :", this);
                TaskInfo info = null;
                string key = string.Empty;
                lock (((ICollection)processCollection).SyncRoot)
                {
                    //if (processCollection.Any(q => q.Value.ProcessId == ((Process)sender).Id))
                    //{
                    //    foreach (KeyValuePair<string, TaskInfo> valuePair in processCollection)
                    //    {
                    //        if (valuePair.Value.ProcessId == ((Process)sender).Id)
                    //        {
                    //            key = valuePair.Key;
                    //            break;
                    //        }
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(key))
                    //    {
                    //        info = processCollection[key];
                    //        processCollection.Remove(key);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "Error in new process exited :" + ex.ToString(), this);
            }

        }

        //not used
        public string DoEncrypt(string dataToEncrypt)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = null;
            DataInfo encryptionInfo = null;
            BinaryFormatter binFormatter = null;
            MemoryStream mStream = null;

            byte[] tempBytes = null;
            byte[] encryptedBytes = null;
            int keySize;
            int maxLength;
            int dataLength;
            int iterations;
            byte[] inputBytes = null;
            string outputString = null;

            try
            {
                rsaCryptoServiceProvider = new RSACryptoServiceProvider(KEYSIZE);
                encryptionInfo = new DataInfo();
                binFormatter = new BinaryFormatter();
                mStream = new MemoryStream();

                keySize = KEYSIZE / 8;
                inputBytes = Encoding.UTF32.GetBytes(dataToEncrypt);

                maxLength = keySize - 42;
                dataLength = inputBytes.Length;
                iterations = dataLength / maxLength;
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i <= iterations; i++)
                {
                    tempBytes = new byte[(dataLength - maxLength * i > maxLength) ?
                                                            maxLength : dataLength - maxLength * i];
                    Buffer.BlockCopy(inputBytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                    encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);

                    // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes 
                    // after encryption and before decryption.
                    // If you do not require compatibility with Microsoft Cryptographic API (CAPI)
                    // and/or other vendors.
                    // Comment out the next line and the corresponding one in the DecryptString 
                    // function.
                    Array.Reverse(encryptedBytes);

                    stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
                }
                encryptionInfo.EncryptedData = stringBuilder.ToString();
                encryptionInfo.KEY = Encoding.Unicode.GetBytes(rsaCryptoServiceProvider.ToXmlString(true));

                binFormatter.Serialize(mStream, encryptionInfo);
                outputString = Convert.ToBase64String(mStream.ToArray());

            }
            catch (Exception rEx)
            {
                // CreateLog(debugFilename,rEx.ToString());
                // throw new RCryptographyException(rEx.Message);
                CreateLog(debugFilename, rEx.ToString(), this);
                throw rEx;

            }
            finally
            {
                if (rsaCryptoServiceProvider != null)
                {
                    rsaCryptoServiceProvider.Clear();
                    rsaCryptoServiceProvider = null;
                }
                if (mStream != null)
                {
                    mStream.Close();
                    mStream = null;
                }
                encryptionInfo = null;
                binFormatter = null;
                inputBytes = null;
                encryptedBytes = null;
                //CreateLog(debugFilename,"RRSAEncrDecr : DoEncrypt Start-> Doing Encryption");

            }
            return outputString;

        }

        public static void CreateLog(string fullFileName, string message, Object ob)
        {
            try
            {
                lock (objLock)
                {
                    if (writer == null)
                        writer = File.AppendText(fullFileName);

                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,ffffff") + " [] INFO LoggerException::" + ob.GetType().FullName + " [] <> -  " + message);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("CosmosScheduler", ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public void StartCosmosScheduledJobProcessListner()
        {
            CreateLog(debugFilename, "Scheduler-> Staring the Cosmos Scheduled Job Process service to read messages", this);
            ServiceHost host = new ServiceHost(typeof(RCTMServiceClient));
            host.Open();
            CreateLog(debugFilename, "Scheduler->Host is open to read messages from Cosmos Scheduled Job Process service.", this);
        }

        RCTMTaskManager objSchedulable;
        public List<string> getSubscriberList(string assemblyLocation, string className,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling getSubscriberList", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);

                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.getSubscribeMailIds(clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling getSubscriberList", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw;
            }
        }
        public List<string> getCalendarList(string assemblyLocation, string className,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling getCalendarList", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);

                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.getCalendarNames(clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling getCalendarList", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw;
            }
        }
        public bool deleteStatusFromClient(string assemblyLocation, string className, int clientTaskStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling deleteStatusFromClient", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                objSchedulable.DeleteTaskStatusFromClient(clientTaskStatusId, clientName);
                CreateLog(debugFilename, "deleting task status at client successful", this);
                return true;
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling deleteStatusFromClient", this);
                CreateLog(debugFilename, ex.ToString(), this);
                return false;
            }

        }
        public bool UndoTask(string assemblyLocation, string className, int clientTaskStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling UndoTask on client", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                objSchedulable.UndoTask(clientTaskStatusId, clientName);
                CreateLog(debugFilename, "undo task at client successful", this);
                return true;
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling undo task in client", this);
                CreateLog(debugFilename, ex.ToString(), this);
                return false;
            }

        }

        public void flowsAdded(string assemblyLocation, string className, List<int> clientTaskStatusId, string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling flowsAdded on client " + assemblyLocation + ">>" + className, this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                objSchedulable.flowAdded(clientTaskStatusId, clientName);
                CreateLog(debugFilename, "flowsAdded at client successful", this);

            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling flowsAdded  in client", this);
                CreateLog(debugFilename, ex.ToString(), this);

            }

        }

        public void flowsDeleted(string assemblyLocation, string className, List<int> clientTaskStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling flowsDeleted on client", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                objSchedulable.flowDeleted(clientTaskStatusId, clientName);
                CreateLog(debugFilename, "flowsDeleted at client successful", this);

            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling flowsDeleted in client", this);
                CreateLog(debugFilename, ex.ToString(), this);

            }

        }

        public List<int> getUnsyncdTasksClientTaskStatusIds(string assemblyLocation, string className, List<int> list,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);
                CreateLog(debugFilename, "RCTMClient:calling getUnsyncdTasks with client task master ids >>" + list, this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);

                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.getUnsyncdTasksClientTaskStatusIds(list, clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling getUnsyncdTasks", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw;
            }
        }

        public List<string> getPrivilegeList(string assemblyLocation, string className, string pageId, string username,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                CreateLog(debugFilename, "RCTMClient:calling getPrivilegeList ", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.getPrivilegeList(pageId, username, clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in calling getPrivilegeList", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw;
            }
        }

        private object GetObjectFromAssembly(string assemblyPath, string className, object[] args)
        {
            try
            {
                CreateLog(debugFilename, "get object from assembly assembly path: " + assemblyPath + " classname :" + className + " args: " + args, this);
                assemblyPath = assemblyPath.Trim();
                className = className.Trim();
                Assembly objAssembly = null;
                CreateLog(debugFilename, "assembly=> " + assemblyPath, this);
                objAssembly = Assembly.LoadFrom(assemblyPath);
                CreateLog(debugFilename, "class=> " + className, this);
                return objAssembly.CreateInstance(className, true, BindingFlags.Instance | BindingFlags.Public, null, args, null, null);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "error in get object from assembly" + ex.ToString(), this);
                throw ex;
            }
        }

        public List<int> isSecureToTrigger(string assemblyLocation, string className, int taskMasterId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                CreateLog(debugFilename, "RCTMClient:starting isSecureToTrigger in client", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.isSecureToTrigger(taskMasterId, clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in isSecureToTrigger in client", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw ex;
            }
        }

        public DataTable SyncStatus(string assemblyLocation, string className, List<int> ctmStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                CreateLog(debugFilename, "RCTMClient:starting SyncStatus in client", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                return objSchedulable.SyncStatus(ctmStatusId, clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in SyncStatus in client", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw ex;
            }
        }

        public void KillInprogressTask(string assemblyLocation, string className, List<int> ctmStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                CreateLog(debugFilename, "RCTMClient:starting KillInprogressTask in client", this);
                assemblyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(assemblyLocation, className, null);
                objSchedulable.KillInprogressTask(ctmStatusId, clientName);
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:error in KillInprogressTask in client", this);
                CreateLog(debugFilename, ex.ToString(), this);
                throw ex;
            }
        }
    }
}

