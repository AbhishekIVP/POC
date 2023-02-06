using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SRMCommonModules
{
    //Vendor Management Data
    [DataContract]
    public class HeadersRequestTypeDataType
    {
        public HeadersRequestTypeDataType(string name, string data)
        {
            this.headerName = name;
            this.headerValue = data;
        }
        [DataMember]
        public string headerName { get; set; }
        [DataMember]
        public string headerValue { get; set; }
    }

    [DataContract]
    public class HeadersRequestType
    {
        public HeadersRequestType()
        {
            value = new List<HeadersRequestTypeDataType>();
        }
        [DataMember]
        public string requestType { get; set; }
        [DataMember]
        public List<HeadersRequestTypeDataType> value { get; set; }
    }

    [DataContract]
    public class ConfigurationsDataType
    {

        public ConfigurationsDataType(string labelName, string value)
        {
            this.labelName = labelName;
            this.value = value;
        }
        [DataMember]
        public string labelName { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class ConfigurationsType
    {
        public ConfigurationsType()
        {
            FTP = new List<ConfigurationsDataType>();
            SAPI = new List<ConfigurationsDataType>();
            HeavyLite = new List<ConfigurationsDataType>();
            GlobalApi = new List<ConfigurationsDataType>();
        }
        [DataMember]
        public int VendorId { get; set; }
        [DataMember]
        public List<ConfigurationsDataType> FTP { get; set; }
        [DataMember]
        public List<ConfigurationsDataType> SAPI { get; set; }
        [DataMember]
        public List<ConfigurationsDataType> HeavyLite { get; set; }
        [DataMember]
        public List<ConfigurationsDataType> GlobalApi { get; set; }
    }

    [DataContract]
    public class HeadersType
    {
        public HeadersType()
        {
            FTP = new List<HeadersRequestType>();
            HeavyLite = new List<HeadersRequestType>();
        }
        [DataMember]
        public int VendorId { get; set; }
        [DataMember]
        public List<HeadersRequestType> FTP { get; set; }
        [DataMember]
        public List<HeadersRequestType> HeavyLite { get; set; }
    }

    [DataContract]
    public class VendorManagementInputInfo
    {
        [DataMember]
        public int VendorManagementId { get; set; }
        [DataMember]
        public string VendorManagementName { get; set; }
        [DataMember]
        public int VendorManagementToCloneFromID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public List<ConfigurationsType> Configurations { get; set; }

        [DataMember]
        public List<HeadersType> Headers { get; set; }
    }

    [DataContract]
    public class VendorManagementDataSourceType
    {
        public VendorManagementDataSourceType()
        {
            Configurations = new ConfigurationsType();
            Headers = new HeadersType();
            VendorPreferences = new List<VendorManagementPreferences>();
        }
        [DataMember]
        public int VendorManagementId { get; set; }
        [DataMember]
        public ConfigurationsType Configurations { get; set; }

        [DataMember]
        public HeadersType Headers { get; set; }

        [DataMember]
        public List<VendorManagementPreferences> VendorPreferences { get; set; }

        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public string Message { get; set; }

    }

    [DataContract]
    public class VendorManagementPreferences
    {

        public VendorManagementPreferences(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }



    }

    [DataContract]
    public class VendorManagementDeleteData
    {
        //public VendorManagementDeleteData()
        //{
        //    VendorPreferences = new List<VendorManagementPreferences>();
        //}
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int VendorManagementId { get; set; }

        //[DataMember]
        //public bool Status { get; set; }

        //[DataMember]
        //public string Message { get; set; }

        //[DataMember]
        //public List<VendorManagementPreferences> VendorPreferences { get; set; }

    }
    enum VendorManagementRequestType
    {
        SAPI = 0,
        FTP = 1,
        GlobalAPI = 2,
        Heavy = 3
    }
}
