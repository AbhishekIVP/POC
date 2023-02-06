using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using System.Globalization;

namespace com.ivp.rad.CTMProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            RCTMProcssStart processStart = null;
            CultureInfo culture = null;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            try
            {
                try
                {
                    int sleepTime = 10000;
                    if (System.Configuration.ConfigurationManager.AppSettings["CTMDelay"] != null)
                    {
                        sleepTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CTMDelay"]);
                    }
                    Thread.Sleep(sleepTime);//Convert.ToInt32(new Random(Convert.ToInt32(DateTime.Now.Ticks)).NextDouble() * 10000));//10000);
                    processStart = new RCTMProcssStart(args[0],args[1]);
                    processStart.ExecuteJob();

                }
                catch(Exception ex)
                {
                    System.Diagnostics.EventLog.WriteEntry("CosmosScheduler", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                    //processStart.UpdateJobStatus("FAILED");
                    Process.GetCurrentProcess().Kill();
                }
                Process.GetCurrentProcess().Kill();
            }

            finally
            {
                processStart.DisposeAll();

            }
        }
    }
}
