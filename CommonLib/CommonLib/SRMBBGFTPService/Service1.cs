using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.transport;
using com.ivp.srm.bbgftpservice;

namespace SRMBBGFTPService
{
    public partial class Service1 : ServiceBase
    {
        private string _EmailIds = string.Empty;
        private string _MailTransportName = string.Empty;
        private string _FromEmailIDForApp = string.Empty;
        private string _ClientName = string.Empty;
        //private string _SecMasterServiceAPIDllName = "SecMasterServiceAPI.dll";
        public List<ServiceHost> serviceHost = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                CultureInfo culture = null;
                culture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                string routingKey = string.Empty;
                string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
                if (File.Exists(customFilePath))
                {
                    XDocument rKeys = XDocument.Load(customFilePath);
                    if (rKeys != null)
                    {
                        foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
                        {
                            if (ele.Attribute("name").Value.Equals("SRMBBGFTPService"))
                            {
                                routingKey = ele.Attribute("key").Value;
                                break;
                            }
                        }
                    }
                }

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
                            newLogFileName = fileName.Substring(0, fileName.LastIndexOf("\\")) + "\\SRMBBGFTPService" + DateTime.Now.ToString("yyyyddMM") + ".txt";
                            node.Attributes["value"].Value = newLogFileName;
                        }
                    }
                    if (!string.IsNullOrEmpty(routingKey))
                    {
                        XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
                        if (RoutingKeyList != null)
                        {
                            foreach (XmlNode node in RoutingKeyList)
                            {
                                node.Attributes["value"].Value = routingKey;
                            }
                        }
                    }
                }
                IRLogger logger = RLogFactory.CreateLogger("SRMBBGFTPService", lstConfig[0].InnerXml);

                AppSettingsReader configReader = new AppSettingsReader();
                _EmailIds = configReader.GetValue("NotificationToEmailIds", typeof(string)).ToString();
                _MailTransportName = configReader.GetValue("MailTransportName", typeof(string)).ToString();
                _FromEmailIDForApp = configReader.GetValue("FromEmailIDForApp", typeof(string)).ToString();
                _ClientName = configReader.GetValue("ClientName", typeof(string)).ToString();

                //try
                //{
                //    _SecMasterServiceAPIDllName = configReader.GetValue("SecMasterServiceAPIDllName", typeof(string)).ToString();
                //}
                //catch (Exception ex)
                //{
                //}

                if (serviceHost != null && serviceHost.Count > 0)
                {
                    serviceHost.ForEach(x => x.Close());
                }

                if (serviceHost == null)
                    serviceHost = new List<ServiceHost>();

                //string[] dllName = ConfigurationManager.AppSettings["SecMasterServiceAPIDllName"].Split('|');
                //string[] ServiceClassesToBeHosted = ConfigurationManager.AppSettings["ServiceClassesToBeHosted"].Split('|');

                //for (int i = 0; i < dllName.Length; i++)
                //{
                //    //Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + dllName[i]);

                //    foreach (string className in ServiceClassesToBeHosted[i].Split(','))
                //    {
                //        try
                //        {
                //            ServiceHost ServiceHost = null;
                //            ServiceHost = new ServiceHost(assembly.GetType(className, true, true));
                //            ServiceHost.Open();
                //            serviceHost.Add(ServiceHost);
                //        }
                //        catch (Exception ex)
                //        {
                //            throw;
                //        }
                //    }
                //}

                try
                {
                    ServiceHost ServiceHost = null;
                    ServiceHost = new ServiceHost(typeof(SRMBbgFtpApi));
                    ServiceHost.Open();
                    serviceHost.Add(ServiceHost);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(@"E:\IVPSecMaster\CosmosSecMasterFiles\log8.0\SRMBBGLog.txt", ex.ToString());
                    throw;
                }

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Service_UnhandledException);

                SendEmailNotification("SRMBBGFTPService started on " + _ClientName + " (" + System.Environment.MachineName + ")", "SRMBBGFTPService started on " + _ClientName + " (" + System.Environment.MachineName + ")");
            }
            catch (Exception ee)
            {
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", "Exception Message: " + ee.ToString());
            }
        }

        void Service_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exp = (Exception)e.ExceptionObject;
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", "Exception Message: " + exp.ToString());
            OnStop();
        }

        private void SendEmailNotification(string logDescription, string subject)
        {
            try
            {
                if (!string.IsNullOrEmpty(_EmailIds))
                {
                    IRTransport transConfig = new RTransportManager().GetTransport(_MailTransportName);
                    RMailContent objMailContent = new RMailContent();
                    StringBuilder body = new StringBuilder();
                    string linkFile = string.Empty;

                    body.AppendLine("Hi,");
                    body.AppendLine("<br></br><br></br>");
                    body.AppendLine(logDescription);
                    body.AppendLine("<br></br><br></br>");
                    body.AppendLine("Thanks,<br></br>IVP Solutions Team");

                    if (transConfig != null)
                    {
                        objMailContent.From = _FromEmailIDForApp;
                        objMailContent.IsBodyHTML = true;
                        objMailContent.MailPriority = System.Net.Mail.MailPriority.High;
                        objMailContent.Subject = subject;
                        objMailContent.To = _EmailIds;
                        objMailContent.Body = body.ToString();

                        transConfig.SendMessage(objMailContent);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }

        protected override void OnStop()
        {
            if (serviceHost != null && serviceHost.Count > 0)
            {
                serviceHost.ForEach(x => x.Close());
            }

            serviceHost = null;

            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(Service_UnhandledException);
            SendEmailNotification("SRMBBGFTPService stopped on " + _ClientName + " (" + System.Environment.MachineName + ")", "SRMBBGFTPService stopped on " + _ClientName + " (" + System.Environment.MachineName + ")");

        }
    }
}
