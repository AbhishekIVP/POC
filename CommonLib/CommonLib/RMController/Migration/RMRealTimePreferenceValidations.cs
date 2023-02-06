using com.ivp.srm.vendorapi;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.srm.vendorapi.reuters;
using com.ivp.srmcommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public enum DataRequestTypes
    {
        [Description("Current Data")]
        Current_Data = 1,

        [Description("Company Data")]
        Company_Data = 2
    }
    public partial class RMRealTimePreferenceSync
    {
        private List<string> ValidateEntityType(string entityTypeName,Dictionary<string,RMEntityDetailsInfo> entityDetails)
        {
            List<string> errorMsg = new List<string>();
            if (!entityDetails.ContainsKey(entityTypeName))
                errorMsg.Add(RMCommonConstants.ENTITY_TYPE_NOT_FOUND);
            return errorMsg;
        }

        private List<string> ValidateDataSource(string dataSourceName,Dictionary<int,string> dataSources)
        {
            List<string> errorMsg = new List<string>();
            if(!dataSources.ContainsValue(dataSourceName))
                errorMsg.Add(RMCommonConstants.DATA_SOURCE_NOT_FOUND);
            return errorMsg;
        }

        private List<string> ValidateRequestTypes(string vendorType, ref string requestType)
        {
            RBbgRequestType bloombergRequest = RBbgRequestType.BULK;
            RReuterRequestType reutersRequest = RReuterRequestType.BULK;
            List<string> errorMsg = new List<string>();

            if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg))
            {
                if (Enum.TryParse(requestType, true, out bloombergRequest))
                    requestType = bloombergRequest.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Request_Type);
            }
               
            else if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Reuters))
            {
                if (Enum.TryParse(requestType, true, out reutersRequest))
                    requestType = reutersRequest.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Request_Type);
            }                

            return errorMsg;
        }

        private List<string> ValidateMarketSector(string vendorType,ref string marketSector)
        {
            RBbgMarketSector mrktSector = RBbgMarketSector.Comdty;
            List<string> errorMsg = new List<string>();
            if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg) && string.IsNullOrEmpty(marketSector))
                errorMsg.Add(RMPreferenceConstants.Empty_Market_Sector);
            else if(vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg) && !string.IsNullOrEmpty(marketSector))
            {
                if (Enum.TryParse(marketSector, true, out mrktSector))
                    marketSector = mrktSector.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Market_Sector);
            }                
            return errorMsg;
        }

        private List<string> ValidateAssetType(string vendorType, ref string assetType)
        {
            List<string> errorMsg = new List<string>();
            if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Reuters) && string.IsNullOrEmpty(assetType))
                errorMsg.Add(RMPreferenceConstants.Empty_Asset_Type);
            else if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Reuters) && !string.IsNullOrEmpty(assetType))
            {
                RReuterAssetTypes reuterTypes = RReuterAssetTypes.CONV;
                if (Enum.TryParse(assetType, true, out reuterTypes))
                    assetType = reuterTypes.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Asset_Type);
            }            
            return errorMsg;
        }

        private List<string> ValidateDataRequestType(string vendorType, ref string dataRequestType)
        {
            List<string> errorMsg = new List<string>();
            if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg) && string.IsNullOrEmpty(dataRequestType))
                errorMsg.Add(RMPreferenceConstants.Empty_Data_Request_Type);
            else if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg) && !string.IsNullOrEmpty(dataRequestType))            
            {
                DataRequestTypes requestTypes = DataRequestTypes.Company_Data;
                if (Enum<DataRequestTypes>.TryParse(dataRequestType, true, out requestTypes))
                    dataRequestType = requestTypes.GetDescription();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Data_Request_Type);
            }                
            return errorMsg;
        }

        private List<string> ValidateTransport(ref string transport, ArrayList allFTPTransports)
        {
            string localTranportName = transport;
            List<string> errorMsg = new List<string>();
            if(!allFTPTransports.SRMContainsWithIgnoreCase(transport))
                errorMsg.Add(RMPreferenceConstants.Invalid_Transport);
            else
            {
                int index = Array.FindIndex(allFTPTransports.ToArray(), p => ((string)p).Equals(localTranportName, StringComparison.CurrentCultureIgnoreCase));
                transport = (string)allFTPTransports[index];
            }
                
            return errorMsg;
        }

        private List<string> ValidateVendorIdentifiers(string vendorType, ref string vendorIdentifier)
        {
            RReuterInstrumentIdType reuterVendorIdentifier = RReuterInstrumentIdType.BDG;
            RBbgInstrumentIdType bloombergVendorIdentifier = RBbgInstrumentIdType.AUSTRIAN;
            List<string> errorMsg = new List<string>();

            if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg))
            { 
                if (Enum.TryParse(vendorIdentifier, true, out bloombergVendorIdentifier))
                    vendorIdentifier = bloombergVendorIdentifier.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Vendor_Identifier);
            }

            else if (vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Reuters))
            {
                if (Enum.TryParse(vendorIdentifier, true, out reuterVendorIdentifier))
                    vendorIdentifier = reuterVendorIdentifier.ToString();
                else
                    errorMsg.Add(RMPreferenceConstants.Invalid_Vendor_Identifier);
            }
            
            return errorMsg;
        }

        private List<string> ValidateVendorType(List<string> vendorTypes, ref string vendorType)
        {
            RVendorType vendorTypeEnum = RVendorType.Bloomberg;
            List<string> errorMsg = new List<string>();

            if(!vendorTypes.SRMContainsWithIgnoreCase(vendorType))
                errorMsg.Add(RMPreferenceConstants.Invalid_Vendor_Type);
            else
            {
                if (Enum.TryParse(vendorType, true, out vendorTypeEnum))
                    vendorType = vendorTypeEnum.ToString();
            }

            return errorMsg;
        }

    }
}
