using System.Configuration;
using System.Reflection;
using System;
using System.Linq;

namespace RCTMService
{
    partial class RCTMInstaller
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
            this.CTMProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CTMInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // CTMProcessInstaller
            // 
            SetServicePropertiesFromCommandLine();
            this.CTMProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CTMProcessInstaller.Password = null;
            this.CTMProcessInstaller.Username = null;
            // 
            // CTMInstaller
            // 
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(RCTMInstaller)).Location);
            

            this.CTMInstaller.Description = "Runs Common Jobs for the Cosmos Modules";
            //this.CTMInstaller.DisplayName = ServiceName;
            //this.CTMInstaller.ServiceName = ServiceName;
            this.CTMInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // RCTMInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CTMProcessInstaller,
            this.CTMInstaller});

        }

        private void SetServicePropertiesFromCommandLine()
        {
            string[] commandlineArgs = Environment.GetCommandLineArgs();

            string servicename;
            string servicedisplayname;
            ParseServiceNameSwitches(commandlineArgs, out servicename, out servicedisplayname);

            this.CTMInstaller.ServiceName = servicename;
            this.CTMInstaller.DisplayName = servicedisplayname;
        }

        private void ParseServiceNameSwitches(string[] commandlineArgs, out string serviceName, out string serviceDisplayName)
        {
            if (commandlineArgs != null && commandlineArgs.Length > 0)
            {
                var servicenameswitch = (from s in commandlineArgs where s.StartsWith("/servicename") select s).FirstOrDefault();
                var servicedisplaynameswitch = (from s in commandlineArgs where s.StartsWith("/servicedisplayname") select s).FirstOrDefault();

                if (servicenameswitch == null)
                    serviceName = "IVPSRMTaskManager";
                else
                {
                    if (!(servicenameswitch.Contains('=') || servicenameswitch.Split('=').Length < 2))
                        throw new ArgumentNullException("The /servicename switch is malformed");

                    serviceName = servicenameswitch.Split('=')[1];
                }

                if (servicedisplaynameswitch == null)
                    serviceDisplayName = "IVPSRMTaskManager";
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
                serviceName = "IVPSRMTaskManager";
                serviceDisplayName = "IVP SRMTaskManager";
            }
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller CTMProcessInstaller;
        private System.ServiceProcess.ServiceInstaller CTMInstaller;
    }
}