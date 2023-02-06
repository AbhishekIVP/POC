using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Reflection;
using System.Timers;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Collections.Specialized;
using com.ivp.rad.RCTMUtils;
using System.Xml;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.common;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using com.ivp.rad.dal;
using System.Xml.Linq;
using com.ivp.srmcommon;

namespace com.ivp.rad.CTMProcess
{
    class RCTMProcssStart
    {
        const int KEYSIZE = 1024;
        private string p;
        string fileName = string.Empty;
        static readonly object objLock = new object();
        static StreamWriter writer = null;
        TaskInfo data = null;
        private ChannelFactory<IInterProcessConnector> channelFactory = null;
        private IInterProcessConnector ServiceContract;
        RCTMTaskManager objSchedulable = null;
        TimeSpan startTime;

        public static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public RCTMProcssStart(string encryptedData, string flowId)
        {
            // TODO: Complete member initialization
            try
            {
                string decryptedData = encryptedData;// DoDecrypt(encryptedData);
                TaskInfo data = GetObjectData(new MemoryStream(Convert.FromBase64String(decryptedData)));
                string directoryName = GetFullFilePath(data.ClientName);
                fileName = directoryName + "\\" + DateTime.Now.ToString("HH-mm-ss") + " " + flowId + " PID " + Process.GetCurrentProcess().Id.ToString() + ".log";
                CreateLog(fileName, "Start Process");
                CreateLog(fileName, "Start Decrypting the object. " + encryptedData);
                
                CreateLog(fileName, "Finished decryption of object and begin deserilize the object.");
                


                CreateLog(fileName, "Finished deserilizatin of object.");
                CreateLog(fileName, string.Format("Job ID - {0}, NoOfRetries - {1}, RetryInterval - {2},Job Start Time - {3}: ", data.FlowID, data.RetryCount, data.RetryInterval, DateTime.Now));
                this.data = data;
                CreateLog(fileName, data.ToString());
                CreateLog(fileName, "Instantiate Logger.");
                CreateJobLogger(data);
                CreateLog(fileName, "Logger Intantiated and invoke Process constructor.");
                //this.fileName = fileName;
                CreateLog(fileName, "Opening WCF channel");
                InitializeWCFConnection();
                CreateLog(fileName, "WCF channel opened");
                // RTransportManager.LoadTransports();
            }
            catch (Exception ex)
            {
                data.Status = new TaskStatusInfo() { Status = TaskStatus.FAILED, TaskLog = ex.ToString() };
                CreateLog(fileName, "Process failed due to following exception " + ex.Message);
                UpdateJobStatus(data);
            }
        }

        private void InitializeWCFConnection()
        {
            channelFactory = new ChannelFactory<IInterProcessConnector>("CommonTaskManagerClient.RCTMServiceClient");
            channelFactory.Open();
            ServiceContract = channelFactory.CreateChannel();
        }
        static string xmlFileName = string.Empty;
        private string GetFullFilePath(string clientName)
        {
            string directoryName = "";
            try
            {
                List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
                foreach (XmlDocument loggerDoc in lstConfig)
                {
                    XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                            xmlFileName = node.Attributes["value"].Value;
                    }
                }
                directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
                //directoryName = directoryName + "\\CTM Process Log\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!string.IsNullOrEmpty(clientName))
                    directoryName = directoryName + "\\" + clientName + "\\CTM Process Log\\" + DateTime.Now.ToString("yyyyMMdd");
                else
                    directoryName = directoryName + "\\CTM Process Log\\" + DateTime.Now.ToString("yyyyMMdd");

                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                return directoryName;
            }
            catch (Exception ex)
            {

                System.Diagnostics.EventLog.WriteEntry("CosmosScheduler", "In getFullFilePath()" + ex.ToString() + "Filename:" + directoryName, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }
        }

        private void CreateJobLogger(TaskInfo data)
        {
            IRLogger logger = null;
            string processId = Process.GetCurrentProcess().Id.ToString();
            data.ProcessId = Convert.ToInt32(processId);
            string clientName = data.ClientName;

            CreateLog(fileName, string.Format("Started created job specific logger for job id -{0}.", data.FlowID));
            //string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
            string jobLoggingFolder = System.IO.Directory.GetParent(xmlFileName).FullName + "\\" + clientName + "\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(jobLoggingFolder))
                Directory.CreateDirectory(jobLoggingFolder);
            string jobLoggingPath = data.FlowID + "_" + System.Text.RegularExpressions.Regex.Replace(data.TaskName, @"[^\w\-.\ ]", "") + "_" + DateTime.Now.ToString("HH-mm-ss") + " PID " + processId + ".log";

            logger= SRMMTCommon.CreateJobLogger(SRMMTConfig.isMultiTenantEnabled, jobLoggingPath, "CTMProcess",  jobLoggingFolder, out jobLoggingPath, clientName);
            SRMMTConfig.SetClientName(clientName);
            //string routingKey = string.Empty;
            //string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
            //if (File.Exists(customFilePath))
            //{
            //    XDocument rKeys = XDocument.Load(customFilePath);
            //    if (rKeys != null)
            //    {
            //        foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
            //        {
            //            if (ele.Attribute("name").Value.Equals("CTMProcess"))
            //            {
            //                routingKey = ele.Attribute("key").Value;
            //                break;
            //            }
            //        }
            //    }
            //}

            //List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            //foreach (XmlDocument loggerDoc in lstConfig)
            //{
            //    XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
            //    foreach (XmlNode node in nodeList)
            //    {
            //        if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
            //            node.Attributes["value"].Value = jobLoggingPath;
            //    }
            //    if (!string.IsNullOrEmpty(routingKey))
            //    {
            //        XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
            //        if (RoutingKeyList != null)
            //        {
            //            foreach (XmlNode node in RoutingKeyList)
            //            {
            //                node.Attributes["value"].Value = routingKey;
            //            }
            //        }
            //    }
            //}

            //CreateLog(fileName, "LOGGER : " + lstConfig[0].InnerXml);

            //IRLogger logger = RLogFactory.CreateLogger("ProcessStartUp", lstConfig[0].InnerXml);

            //TODO:Set log file path
            RDBConnectionManager dbConnection = null;
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            dbConnection.ExecuteQuery(string.Format(@"UPDATE dbo.ivp_rad_task_status 
                SET task_log_full_path = '{1}', process_log_full_path = '{2}', task_time_out = '{3}', process_id = {4}
                WHERE task_status_id = {0};", data.Status.StatusId, jobLoggingPath, fileName, data.TaskEndTime, processId), RQueryType.Update);
            
            CreateLog(fileName, "log_file_path updated in db");

            logger.Error("Log File Created for RCommonTaskManaget.RCTMProcessStart");
            CreateLog(fileName, string.Format("logger created for job id -{0}.", data.FlowID));
        }

        public static TaskInfo GetObjectData(Stream infoStream)
        {
            BinaryFormatter binFormatter = null;
            TaskInfo processInfo = null;
            try
            {
                binFormatter = new BinaryFormatter();
                if (infoStream.Length == 0)
                    return null;
                //Stream stream = File.Open(infoStream, FileMode.Open);
                processInfo = (TaskInfo)binFormatter.Deserialize(infoStream, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                infoStream.Close();
                infoStream.Dispose();
                binFormatter = null;
            }
            return processInfo;
        }

        internal void ExecuteJob()
        {
            try
            {
                CreateLog(fileName, "Process constructor invoked and execute the job.");
                int noOfRetries = data.RetryCount;
                CreateLog(fileName, "Getting schedulable Task class for the Job.");
                objSchedulable = (RCTMTaskManager)GetObjectFromAssembly(data.AssemblyLocation, data.ClassName, null);
                CreateLog(fileName, "Schedulable Task class for the Job achieved.");
                do
                {
                    CreateLog(fileName, "Calling run job method for the job.");
                    objSchedulable.RunJob(data,data.ClientName);
                    CreateLog(fileName, "ran the job.");
                    if (data.TimeOut > 0)
                    {
                        startTime = DateTime.Now.TimeOfDay;
                        System.Timers.Timer mTimer = new System.Timers.Timer(double.Parse(ConfigurationManager.AppSettings["TimerInterval"]));
                        mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                        mTimer.Start();
                    }
                    CreateLog(fileName, "Job execution completed.Got the status - " + data.Status.Status.ToString());
                    if (data.Status.Status.Equals(Enum.Parse(typeof(TaskStatus), TaskStatus.PASSED.ToString())))
                    {

                        if (string.IsNullOrEmpty(data.CustomClassAssembly) == false && string.IsNullOrEmpty(data.CustomClassClassName))
                        {
                            CreateLog(fileName, "calling custom class");
                            CallCustomClass();
                            CreateLog(fileName, "custom class executed");
                        }
                        break;
                    }
                    else
                    {
                        if (noOfRetries > 0)
                        {
                            CreateLog(fileName, "applying retry for job.");
                            Thread.Sleep(new TimeSpan(0, data.RetryInterval, 0));
                            //UpdateForRetryOrError(RScheduledProcessConstants.SCHEDULED_PROCESS_RETRY_SCHEDULED);
                        }
                        noOfRetries--;
                    }
                } while (noOfRetries > 0);
                // NotifyMail();
                UpdateJobStatus(data);
                CreateLog(fileName, "task completed");

            }
            catch (Exception ex)
            {
                CreateLog(fileName, "Update Database through WCF - the job was failed." + ex.ToString());
                if (data.Status == null || string.IsNullOrEmpty(data.Status.TaskLog))
                    data.Status = new TaskStatusInfo() { Status = TaskStatus.FAILED, TaskLog = ex.Message };
                UpdateJobStatus(data);
            }
        }

        private void CallCustomClass()
        {
            Assembly objAssembly = null;

            objAssembly = Assembly.LoadFrom(data.CustomClassAssembly);
            object obj = objAssembly.CreateInstance(data.CustomClassClassName,
                                         true,
                                         BindingFlags.Instance | BindingFlags.Public,
                                         null,
                                         null,
                                         null,
                                         null);
            ((RCTMCustomClass)obj).ExecuteTask(data);
        }

        private void NotifyMail()
        {
            CreateLog(fileName, "begin sending notify mail");
            string mailTo = string.Empty;

            switch (data.Status.Status)
            {
                case TaskStatus.PASSED:
                    if (data.MailSubscribeId.ContainsKey(TaskStatus.PASSED) == true)
                    {
                        SendMail(data.MailSubscribeId[TaskStatus.PASSED]);
                    }
                    break;
                case TaskStatus.FAILED:
                    if (data.MailSubscribeId.ContainsKey(TaskStatus.FAILED) == true)
                    {
                        SendMail(data.MailSubscribeId[TaskStatus.FAILED]);
                    }
                    break;
            }
            CreateLog(fileName, "end sending notify mail");
        }

        private void SendMail(MailDetails mailDetail)
        {
            try
            {
                if (string.IsNullOrEmpty(mailDetail.mailBody) == false && string.IsNullOrEmpty(mailDetail.MailSubject) == false)
                {
                    string userName = string.Empty;
                    string password = string.Empty;
                    bool enableSsl = false;
                    string Host = string.Empty;
                    int port = -1;
                    NameValueCollection mailDetails = ConfigurationManager.GetSection("CustomAppSettingsGroup/MailDetails") as NameValueCollection;
                    //var mailDetails = ConfigurationManager.GetSection("MailDetails") as NameValueCollection;
                    if (mailDetails != null)
                    {
                        userName = mailDetails["UserName"].ToString();
                        password = mailDetails["Password"].ToString();
                        enableSsl = Convert.ToBoolean(mailDetails["EnableSsl"]);
                        Host = mailDetails["Server"].ToString();
                        port = Convert.ToInt32(mailDetails["Port"]);
                    }

                    NetworkCredential mNetworkCredential = new NetworkCredential(userName, password);
                    SmtpClient client = new SmtpClient();
                    client.Credentials = mNetworkCredential;
                    client.EnableSsl = enableSsl;
                    client.Host = Host;
                    client.Port = port;
                    MailMessage mMailMsg = new MailMessage();
                    mMailMsg.From = new MailAddress(userName);
                    mMailMsg.Subject = mailDetail.MailSubject;
                    mMailMsg.Body = mailDetail.mailBody;
                    mMailMsg.To.Add(string.Join(";", mailDetail.MailIds));
                    mMailMsg.IsBodyHtml = true;
                    mMailMsg.Priority = MailPriority.Normal;
                    client.Send(mMailMsg);
                }
            }
            catch (Exception ex)
            {
                CreateLog(fileName, "Error unable to send mail>>" + ex.Message);
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CreateLog(fileName, "Time out");
                TimeSpan timeLapsed = DateTime.Now.TimeOfDay.Subtract(startTime);
                if (timeLapsed.Minutes >= data.TimeOut)
                {
                    objSchedulable.Cancel(data, data.ClientName);
                    UpdateJobStatus(data);
                    DisposeAll();
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                DisposeAll();
                Process.GetCurrentProcess().Kill();
            }
        }

        private object GetObjectFromAssembly(string assemblyPath, string className,
                                                                       object[] args)
        {
            try
            {
                assemblyPath = assemblyPath.Trim();
                className = className.Trim();
                Assembly objAssembly = null;
                CreateLog(fileName, "assembly=> " + assemblyPath);
                objAssembly = Assembly.LoadFrom(assemblyPath);
                CreateLog(fileName, "class=> " + className);
                return objAssembly.CreateInstance(className,
                                             true,
                                             BindingFlags.Instance | BindingFlags.Public,
                                             null,
                                             args,
                                             null,
                                             null);
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                throw ex;
            }
        }

        internal void UpdateJobStatus(TaskInfo info)
        {
            try
            {
                info.ProcessId = Process.GetCurrentProcess().Id;
                CreateLog(fileName, "Update Database through WCF for the job status." + info);
                ServiceContract.UpdateJobCompletionStatus(info, info.ClientName);
            }
            catch (Exception ex)
            {
                CreateLog(fileName, "RCTMProcessStart>>Error while trying to update job");
                CreateLog(fileName, ex.ToString());
            }
        }

        internal void DisposeAll()
        {
            CreateLog(fileName, "process execution completed.Clearing all resources.");
            if (channelFactory.State != CommunicationState.Closed)
            {
                channelFactory.Close();
            }
            channelFactory = null;
            CreateLog(fileName, "process execution completed.all resources cleared.");
            CreateLog(fileName, "Channel factory and other resources disposed and exiting.");
            CreateLog(fileName, "Bye Bye.");
        }

        public void CreateLog(string fullFileName, string message)
        {
            try
            {
                lock (objLock)
                {
                    if (writer == null)
                        writer = File.AppendText(fullFileName);

                    writer.WriteLine("[TimeStamp:->" + DateTime.Now.ToString("MMddyyyyHH:mm:ss") + "] Message:->" + message);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("CosmosScheduler", ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        //public string DoDecrypt(string encryptedData)
        //{
        //    RSACryptoServiceProvider rsaCryptoServiceProvider = null;
        //    DataInfo decryptionInfo = null;
        //    BinaryFormatter binFormatter = null;
        //    MemoryStream mStream = null;

        //    string xmlString = null;
        //    string inputString = null;
        //    ArrayList arrayList = null;
        //    string outputString = null;
        //    //CspParameters rsaParams = null;
        //    try
        //    {
        //        mStream = new MemoryStream(Convert.FromBase64String(encryptedData));
        //        binFormatter = new BinaryFormatter();
        //        decryptionInfo = (DataInfo)binFormatter.Deserialize((Stream)mStream, null);
        //        inputString = decryptionInfo.EncryptedData;
        //        xmlString = Encoding.Unicode.GetString(decryptionInfo.KEY);
        //        //rsaParams = new CspParameters();
        //        //rsaParams.Flags = CspProviderFlags.UseExistingKey;


        //        rsaCryptoServiceProvider = new RSACryptoServiceProvider(KEYSIZE);//, rsaParams);
        //        rsaCryptoServiceProvider.FromXmlString(xmlString);


        //        //to be used if keysize is changed from 1024
        //        int base64BlockSize = ((((KEYSIZE / 8) / 3) * 4) + 4);
        //        //int base64BlockSize = ((KEYSIZE / 8) % 3 != 0) ?
        //        //                        ((((KEYSIZE / 8) / 3) * 4) + 4) : (((KEYSIZE / 8) / 3) * 4);
        //        int iterations = inputString.Length / base64BlockSize;
        //        arrayList = new ArrayList();

        //        for (int i = 0; i < iterations; i++)
        //        {
        //            byte[] encryptedBytes = Convert.FromBase64String(
        //                               inputString.Substring(base64BlockSize * i, base64BlockSize));
        //            // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes
        //            // after encryption and before decryption.
        //            // If you do not require compatibility with Microsoft Cryptographic API (CAPI) 
        //            // and/or other vendors.
        //            // Comment out the next line and the corresponding one in the EncryptString 
        //            // function.
        //            Array.Reverse(encryptedBytes);
        //            arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
        //        }
        //        outputString = Encoding.UTF32.GetString
        //                                 (arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        //    }
        //    catch (Exception rEx)
        //    {
        //        throw rEx;
        //    }
        //    finally
        //    {
        //        if (rsaCryptoServiceProvider != null)
        //        {
        //            rsaCryptoServiceProvider.Clear();
        //            rsaCryptoServiceProvider = null;
        //        }
        //        if (mStream != null)
        //        {
        //            mStream.Close();
        //            mStream = null;
        //        }
        //        decryptionInfo = null;
        //        binFormatter = null;
        //        arrayList = null;


        //    }
        //    return outputString;
        //}
    }


}
