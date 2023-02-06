using RCTMExecutorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCTMTaskExecutorProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RCTMExecutorServiceClient client = new RCTMExecutorServiceClient();
                TaskExecutorResponse res = client.TriggerTask(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));

                if (res.ExitCode == 0)
                    Environment.ExitCode = 0;
                else
                    Environment.ExitCode = 1;
                Console.WriteLine(res.Message);
            }
            catch (Exception er)
            {
                Environment.ExitCode = 1;
                Console.WriteLine(er.ToString());
            }
        }
    }
}
