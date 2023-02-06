using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using com.ivp.rad.common;
using System.ServiceModel;
using com.ivp.rad.RCommonTaskManager;
using System.ServiceModel.Description;
using com.ivp.rad.configurationmanagement;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using com.ivp.srmcommon;
using com.ivp.common.srmdwhjob;

namespace com.ivp.rad.RCTMService
{
    public partial class RCTMService : ServiceBase
    {
        private Timer mTimer = null;
        private EventLog mLog = null;
        static IRLogger mLogger = null; //RLogFactory.CreateLogger("RCTMService");        
        //private RCTMJobExecuter mJobExecutor = null;
        static int dwhProcessID;
        public RCTMService()
        {
            InitializeComponent();
            //System.Threading.Thread.Sleep(15000);
            //mJobExecutor = new RCTMJobExecuter();
        }



        //private void CreateJobLogger()
        //{
        //    string routingKey = string.Empty;
        //    string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
        //    if (File.Exists(customFilePath))
        //    {
        //        XDocument rKeys = XDocument.Load(customFilePath);
        //        if (rKeys != null)
        //        {
        //            foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
        //            {
        //                if (ele.Attribute("name").Value.Equals("CTMService"))
        //                {
        //                    routingKey = ele.Attribute("key").Value;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
        //    foreach (XmlDocument loggerDoc in lstConfig)
        //    {
        //        XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
        //        foreach (XmlNode node in nodeList)
        //        {
        //            if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
        //            {
        //                string xmlFileName = node.Attributes["value"].Value;
        //                string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
        //                node.Attributes["value"].Value = directoryName + "\\" + "CTMServiceLog.txt";
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(routingKey))
        //        {
        //            XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
        //            if (RoutingKeyList != null)
        //            {
        //                foreach (XmlNode node in RoutingKeyList)
        //                {
        //                    node.Attributes["value"].Value = routingKey;
        //                }
        //            }
        //        }
        //    }
        //    mLogger = RLogFactory.CreateLogger("CTMService", lstConfig[0].InnerXml);
        //}

        protected override void OnStart(string[] args)
        {
            string loggingPath = string.Empty;
            try
            {
                mLogger = SRMMTCommon.CreateJobLogger(SRMMTConfig.isMultiTenantEnabled, string.Empty, "CTMService", string.Empty, out loggingPath, string.Empty);

                //CreateJobLogger();
                //System.Threading.Thread.Sleep(15000);
                dwhProcessID = SRMCommon.TriggerProcess("SRMDWHService.exe", "SRMDWHService", AppDomain.CurrentDomain.BaseDirectory, SRMDWHJob.ExitCallback);
                mLogger.Debug("DWH Service Process ID: " + dwhProcessID);
                HostService();

                mLogger.Error("CTM SERVICE STARTED");
            }
            catch (Exception ex)
            {
                string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RCTMCreateLoggerError_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");

                System.IO.File.AppendAllText(fileName, string.Format("Exception Message: {0}", ex));
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Service_UnhandledException);


            //UpdateNextScheduledTime();
            //string scheduledJobInterval = ConfigurationManager.AppSettings["ScheduledJobInterval"];
            //mTimer = new Timer(double.Parse(scheduledJobInterval));
            //mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //mTimer.Start();
        }

        private void Service_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SRMMTConfig.MethodCallPerClient(DumpQueues);
            Exception exp = (Exception)e.ExceptionObject;
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "CTMServiceUnhandledLog" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", "Exception Message: " + exp.ToString());
        }

        private void DumpQueues(string clientName)
        {
            CTMService.DumpQueues(clientName);
        }

        private void HostService()
        {
            ServiceHost host = new ServiceHost(typeof(CTMService));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            host.Open();

            try
            {
                ServiceHost host1 = new ServiceHost(typeof(RCTMExecutorService.RCTMExecutorService));
                host1.Open();
            }
            catch { }
        }

        /// <summary>
        /// Handles the Elapsed event of the timer control.Looks for scheduled jobs to be executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // mJobExecutor.RunScheduledJobs();
        }


        private void UpdateNextScheduledTime()
        {
            mLogger.Debug("Scheduler-> Update Next scheduled times on scheduler restart");
            // mJobExecutor.UpdateNextScheduledTime();
            mLogger.Debug("Scheduler-> Updated Next scheduled times on scheduler restart");
        }

        protected override void OnStop()
        {
            //mTimer.Stop();
            try
            {
                mLogger.Debug("OnStop-DWH Service Process ID : " + dwhProcessID);
                if (dwhProcessID != -1 && Process.GetProcessById(dwhProcessID) != null)
                {
                    Process.GetProcessById(dwhProcessID).Kill();
                    mLogger.Debug("OnStop- Kill DWH Service Process");

                }
            }
            catch { }
            SRMMTConfig.MethodCallPerClient(DumpQueues);
        }

        protected override void OnShutdown()
        {
            //mTimer.Stop();
            // base.OnShutdown();
            SRMMTConfig.MethodCallPerClient(DumpQueues);
        }

    }
}
