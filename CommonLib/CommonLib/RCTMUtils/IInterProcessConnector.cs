using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace com.ivp.rad.RCTMUtils
{
    [ServiceContract]
    public interface IInterProcessConnector
    {
        [OperationContract]
        void UpdateJobCompletionStatus(TaskInfo status,string clientName);
    }
}
