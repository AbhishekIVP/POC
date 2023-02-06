using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RCTMExecutorService
{
    [ServiceContract]
    public interface IRCTMExecutorService
    {
        [OperationContract]
        TaskExecutorResponse TriggerTask(int chainId, int flowId);
    }
}
