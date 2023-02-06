namespace com.ivp.rad.RCTMService
{
    partial class RCTMService
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
            this.mLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.mLog)).BeginInit();
            // 
            // mLog
            // 
            this.mLog.Log = "Application";
            this.mLog.Source = "CTMService";
            // 
            // SchedulerService
            // 
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.ServiceName = "CTMService";
            ((System.ComponentModel.ISupportInitialize)(this.mLog)).EndInit();
        }

        #endregion
    }
}
