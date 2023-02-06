using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace com.ivp.common
{
    [ServiceContract]
    public interface ICommonExceptionManagerService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMCountResponse> SMGetExceptionCountData(SMExceptionFilterInfo objSMExceptionFilterInfo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMTags> SMGetExceptionTagsCountData(SMExceptionFilterInfo objSMExceptionFilterInfo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMTags> RMGetExceptionTagsCountData(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMCountResponse> RMGetExceptionCountData(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMCommonData> GetCommonEMData(SMExceptionFilterInfo objSMExceptionFilterInfo, RMExceptionFilterInfo objRMExceptionFilterInfo, string ModuleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<EMTags> GetCommonEMTags(SMExceptionFilterInfo objSMExceptionFilterInfo, RMExceptionFilterInfo objRMExceptionFilterInfo,string ModuleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllSectypes(HashSet<string> selectedSectypes, string username);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllEntityTypes(HashSet<string> selectedEntityTypes,string userName, string ModuleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllAttributesBasedOnSectypeSelection(string secTypeIds, string userName, HashSet<string> selectedAttributes);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllAttributesBasedOnEntityTypeSelection(string entityTypeIds, HashSet<string> selectedAttributes);
    }

    [DataContract]
    public class EMCommonData
    { 
        [DataMember]
        public string ExceptionType { get; set; }
        [DataMember]
        public string ExceptionTypeID { get; set; }
        [DataMember]
        public string SecmasterCount { get; set; }
        [DataMember]
        public string RefmasterCount { get; set; }
        [DataMember]
        public string PartyCount { get; set; }
        [DataMember]
        public string FundCount { get; set; }
    }

    [DataContract]
    public class EMCountResponse
    {
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public string ExceptionsCount { get; set; }
        [DataMember]
        public int TypeId { get; set; }
        [DataMember]
        public List<EMExceptions> Exceptions { get; set; }
    }

    [DataContract]
    public class EMExceptions
    { 
        [DataMember]
        public string ExceptionType { get; set; }
        [DataMember]
        public string ExceptionCount { get; set; }
        [DataMember]
        public string ExceptionTypeID { get; set; }
    }

    [DataContract]
    public class EMTags
    {
        [DataMember]
        public string TagName { get; set; }
        [DataMember]
        public string TagCount { get; set; }
    }

    [DataContract]
    public class SMExceptionFilterInfo
    {
        [DataMember]
        public List<int> ListSecurityType { get; set; }
        [DataMember]
        public List<int> ListAttribute { get; set; }
        [DataMember]
        public List<string> ListExceptionType { get; set; }
        [DataMember]
        public List<int> ListSectypeTableId { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public List<string> ListSecurityId { get; set; }
        [DataMember]
        public int Resolved { get; set; }
        [DataMember]
        public int Suppressed { get; set; }
        [DataMember]
        public string TagName { get; set; }
    }

    [DataContract]
    public class RMExceptionFilterInfo
    {
        [DataMember]
        public List<int> ListEntityType { get; set; }
        [DataMember]
        public List<int> ListAttribute { get; set; }
        [DataMember]
        public List<string> ListExceptionType { get; set; }
        [DataMember]
        public List<int> ListLegs { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public List<string> ListEntityCode { get; set; }
        [DataMember]
        public int Resolved { get; set; }
        [DataMember]
        public int Suppressed { get; set; }
        [DataMember]
        public string TagName { get; set; }
    }

    [DataContract]
    public class ListItems
    {
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string isSelected { get; set; }
    }
}
