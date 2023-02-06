using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace com.ivp.rad.RCTMService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new RCTMService() 
			};
            ServiceBase.Run(ServicesToRun);

            if (!EventLog.SourceExists("CTMService"))
            {
                EventLog.CreateEventSource("CTMService", "NewLog");
            }
        }
    }
}
