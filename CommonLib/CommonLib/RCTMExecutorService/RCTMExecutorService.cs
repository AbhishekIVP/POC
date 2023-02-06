using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RCTMExecutorService
{
    public class RCTMExecutorService : IRCTMExecutorService
    {
        private static IRLogger mLogger = null;

        public RCTMExecutorService()
        {
            CreateJobLogger();
        }

        public static void CreateJobLogger()
        {
            string fileName = string.Empty, newLogFileName = string.Empty;
            List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                    {
                        fileName = node.Attributes["value"].Value;
                        var dir = fileName.Substring(0, fileName.LastIndexOf("\\")) + "\\ExecuteCTMTaskLog";

                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        newLogFileName = dir + "\\ExecuteCTMTaskLog_" + DateTime.Now.ToString("yyyyddMM_hh_mm_ss_fff") + Process.GetCurrentProcess().Id + ".txt";

                        node.Attributes["value"].Value = newLogFileName;
                    }
                }
            }
            mLogger = RLogFactory.CreateLogger("RCTMExecutorService", lstConfig[0].InnerXml);
        }

        public TaskExecutorResponse TriggerTask(int chainId, int flowId)
        {
            mLogger.Error("TriggerTask ----> START");
            mLogger.Error("TriggerTask ----> Chain Id : " + chainId);
            mLogger.Error("TriggerTask ----> Flow Id : " + flowId);
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo("ExecuteCTMTask.exe", chainId + " " + flowId);

                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;
                processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardInput = true;
                processInfo.RedirectStandardOutput = true;

                Process p = Process.Start(processInfo);
                p.WaitForExit();

                return new TaskExecutorResponse { ExitCode = p.ExitCode, Message = p.StandardOutput.ReadToEnd() };
            }
            catch (Exception ex)
            {
                mLogger.Error("TriggerTask ----> ERROR : " + ex.ToString());
                return new TaskExecutorResponse { ExitCode = 1, Message = ex.ToString() };
            }
            finally
            {
                mLogger.Error("TriggerTask ----> END");
            }
        }
    }

    [DataContract]
    public class TaskExecutorResponse
    {
        [DataMember]
        public int ExitCode { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
