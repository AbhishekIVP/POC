using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace com.ivp.common
{
    [ServiceContract(Namespace = "com.ivp.common", Name = "DeDuplicationService")]
    public interface IDeDuplicationService
    {
        [OperationContract]
        List<DeDupeListItem> GetAllSectypes(string userName);

        [OperationContract]
        List<DeDupeListItem> GetAllEntityTypes(string userName, int ModuleId);

        [OperationContract]
        string[] GetEntityTypeAttributesForDeDupe(string entityTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SavePreset(DeDupeConfig configValues);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SaveRMPreset(DeDupeConfig configValues);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DeDupeListItem> GetDeDuplicateModuleList();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DeDupeListItem> GetDeDuplicateMatchTypeList();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DeDuplicationResponseData> FindDupesData(int configId, DeDupeConfig configDetails, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DeDuplicationResponseData> FindEntityDupesData(int configId, DeDupeConfig configDetails, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool CheckEntityHasDependency(List<string> entityCodes);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        string SetMergedEntityDetails(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS, int deletionOption);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<int, DeDupeConfig> GetDeDuplicateFilterList();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<int, DeDupeConfig> GetRMDeDuplicateFilterList(string userName, int ModuleId = 0);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<string, Dictionary<string, string>> GetSecurityAttributeValues(List<string> sectypeNames, List<string> secIds, string username);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DeDupeConfig GetPreset(int dupes_master_id);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        MergeResponse GetSecurityData(string[] secIds);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        MergeResponse GetDupeEntityData(string[] secIds);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string MergeSecurities(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS);


        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<string, List<DeDupeListItem>> GetToleranceOptions();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void CopyTimeSeriesAndDeleteSecuritiesCallback(string unique, string secId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<int, string> GetLegNames(string sectypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Dictionary<int, string> GetEntityTypeLegNames(string sectypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string[] GetAttributeBasedOnSecTypeSelectionForDeDupe(string secTypeIds, string userName);
    }

    [DataContract]
    public class DeDupeListItem
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class DeDupeConfig
    {
        [DataMember]
        public string configName { get; set; }
        [DataMember]
        public int moduleId { get; set; }
        [DataMember]
        public int[] sectypeId { get; set; }
        [DataMember]
        public int matchConfidence { get; set; }
        [DataMember]
        public string entityTypeName { get; set; }
        [DataMember]
        public List<Attribute> attrList { get; set; }
        [DataMember]
        public List<string> entityCodes { get; set; }
    }

    [DataContract]
    public class Attribute
    {
        [DataMember]
        public int attributeId { get; set; }
        [DataMember]
        public int matchTypeId { get; set; }
        [DataMember]
        public int matchPercentage { get; set; }
        [DataMember]
        public int toleranceValue { get; set; }
        [DataMember]
        public string toleranceOptionSelected { get; set; }
    }

    [DataContract]
    public class DeDuplicationResponseData
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public List<string> colHeaders { get; set; }
        [DataMember]
        public bool checkboxRequired { get; set; }
        [DataMember]
        public Dictionary<string, Dictionary<string, MergeAttributeInfo>> mergeSecurities { get; set; }
    }
}
