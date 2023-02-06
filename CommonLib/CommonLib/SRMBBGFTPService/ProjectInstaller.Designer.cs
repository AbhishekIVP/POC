using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Configuration.Install;

namespace SRMBBGFTPService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            SetServicePropertiesFromCommandLine();
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});

        }

        private void SetServicePropertiesFromCommandLine()
        {
            string[] commandlineArgs = Environment.GetCommandLineArgs();

            string servicename;
            string servicedisplayname;
            ParseServiceNameSwitches(commandlineArgs, out servicename, out servicedisplayname);

            serviceInstaller1.ServiceName = servicename;
            serviceInstaller1.DisplayName = servicedisplayname;
        }

        private void ParseServiceNameSwitches(string[] commandlineArgs, out string serviceName, out string serviceDisplayName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(ProjectInstaller)).Location);
            if (commandlineArgs != null && commandlineArgs.Length > 0)
            {
                var servicenameswitch = (from s in commandlineArgs where s.StartsWith("/servicename", StringComparison.OrdinalIgnoreCase) select s).FirstOrDefault();
                var servicedisplaynameswitch = (from s in commandlineArgs where s.StartsWith("/servicedisplayname", StringComparison.OrdinalIgnoreCase) select s).FirstOrDefault();
                if (servicenameswitch == null)
                {
                    if (config != null && config.AppSettings.Settings["SRMBBGFTPServiceName"] != null && !string.IsNullOrEmpty(config.AppSettings.Settings["SRMBBGFTPServiceName"].Value))
                    {
                        serviceName = config.AppSettings.Settings["SRMBBGFTPServiceName"].Value;
                    }
                    else
                    {
                        serviceName = "SRMBBGFTPService";
                    }
                }
                else
                {
                    if (!(servicenameswitch.Contains('=') || servicenameswitch.Split('=').Length < 2))
                        throw new ArgumentNullException("The /servicename switch is malformed");

                    serviceName = servicenameswitch.Split('=')[1];
                }

                if (servicedisplaynameswitch == null)
                {
                    if (config != null && config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"] != null && !string.IsNullOrEmpty(config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"].Value))
                    {
                        serviceDisplayName = config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"].Value;
                    }
                    else
                    {
                        serviceDisplayName = "SRMBBGFTPService";
                    }
                }
                else
                {
                    if (!(servicedisplaynameswitch.Contains('=') || servicedisplaynameswitch.Split('=').Length < 2))
                        throw new ArgumentNullException("The /servicedisplaynameswitch switch is malformed");

                    serviceDisplayName = servicedisplaynameswitch.Split('=')[1];
                }

                serviceName = serviceName.Trim('"');
                serviceDisplayName = serviceDisplayName.Trim('"');
            }
            else
            {
                if (config != null && config.AppSettings.Settings["SRMBBGFTPServiceName"] != null && !string.IsNullOrEmpty(config.AppSettings.Settings["SRMBBGFTPServiceName"].Value))
                {
                    serviceName = config.AppSettings.Settings["SRMBBGFTPServiceName"].Value;
                }
                else
                {
                    serviceName = "SRMBBGFTPService";
                }
                if (config != null && config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"] != null && !string.IsNullOrEmpty(config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"].Value))
                {
                    serviceDisplayName = config.AppSettings.Settings["SRMBBGFTPServiceDisplayName"].Value;
                }
                else
                {
                    serviceDisplayName = "SRMBBGFTPService";
                }
            }
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}