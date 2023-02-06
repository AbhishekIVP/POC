using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using com.ivp.rad.RCTMUtils;

namespace CommonTaskManagerClient
{
    class Program
    {
        static StreamWriter writer = null;
        static string debugFilename = "ClientErrorLog.txt";
        static readonly object objLock = new object();
        static void Main(string[] args)
        {
            CreateLog(debugFilename, "RCTMClient:Main()>>creating new instance of RCTMServiceClient in main",new object());
            try
            {
                RCTMServiceClient client = new RCTMServiceClient();
                StartCosmosScheduledJobProcessListner();
                Console.Read();
            }
            catch (Exception ex)
            {
                CreateLog(debugFilename, "RCTMClient:Main()>>error while trying to create new instance of RCTMServiceClient in main", new object());
                CreateLog(debugFilename, ex.ToString(), new object());
            }
        }
        private static void StartCosmosScheduledJobProcessListner()
        {
            // mLogger.Debug("Scheduler-> Staring the Cosmos Scheduled Job Process service to read messages");
            ServiceHost host = new ServiceHost(typeof(RCTMServiceClient));
            host.Open();
            //mLogger.Debug("Scheduler->Host is open to read messages from Cosmos Scheduled Job Process service.");
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
    }
}
