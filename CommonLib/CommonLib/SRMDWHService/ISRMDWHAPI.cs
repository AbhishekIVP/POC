using com.ivp.common.srmdwhjob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.srm.dwhdownstream
{
    [ServiceContract(Namespace = "com.ivp.refmaster.dwhdownstream", Name = "DWHDownstreamService")]
    [ServiceKnownType(typeof(List<string>))]
    [Obfuscation(ApplyToMembers = true, Exclude = true)] 
    public interface ISRMDWHAPI
    {
        /// <summary>
        /// Method to update entities
        /// </summary>
        /// <param name="setupName"></param>
        /// <returns>Output status info</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/TriggerDWHSync", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SRMDWHSyncOutputInfo TriggerDWHSync(SRMDWHInputInfo inputInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/RollData", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SRMDWHOutputInfo RollData(SRMDWHInputInfo inputInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetWorkerResponse", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void GetWorkerResponse(SRMDWHInternalInfo inputInfo);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/CheckSyncStatus/{setupStatusId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SRMDWHSyncStatus CheckSyncStatus(string setupStatusId);
    }

    
}
