using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class RMPreferenceController
    {
        public ObjectSet GetRealTimePreferences(int moduleId, List<int> entityTypes,bool requireIdColumns)
        {            
            ObjectSet dbData = new RMPreferenceDBManager(null).GetRealTimePreferences(moduleId, string.Join(",", entityTypes));
            if (dbData != null && dbData.Tables.Count > 0)
            {
                ObjectSet preferenceData = new ObjectSet();
                ObjectTable preferenceTable = new ObjectTable();
                preferenceTable.TableName = RMPreferenceConstants.TABLE_PREFERENCE_SETUP;

                List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Entity_Type_Name, DataType = typeof(String), DefaultValue = string.Empty }); 
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Preference_Name , DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Preference_Description, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Vendor_Type, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Request_Type, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Market_Sector, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Data_Source_Name, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Vendor_Identifier, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Transport, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Data_Request_Type, DataType = typeof(String), DefaultValue = string.Empty });
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.Asset_Type, DataType = typeof(String), DefaultValue = string.Empty });

                if(requireIdColumns)
                {
                    //columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.entity_type_id, DataType = typeof(Int32), DefaultValue = string.Empty });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.preference_id, DataType = typeof(Int32), DefaultValue = 0 });
                    //columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.data_source_id, DataType = typeof(Int32), DefaultValue = string.Empty });
                }

                new RMCommonController().AddColumnsToObjectTable(preferenceTable, columnsToAdd);

                Dictionary<string, string> dictControlPropertyMapping = new Dictionary<string, string>();
                PopulateControlPropMapping(dictControlPropertyMapping);

                List<RMLicenseKeyPropertyMapping> mappings = new List<RMLicenseKeyPropertyMapping>();
                foreach (KeyValuePair<string, string> kvp in dictControlPropertyMapping)
                {
                    RMLicenseKeyPropertyMapping map = new RMLicenseKeyPropertyMapping();
                    map.KeyName = kvp.Key;
                    map.PropertyName = kvp.Value;
                    mappings.Add(map);
                }

                XmlDocument doc = new XmlDocument();
                dbData.Tables[0].AsEnumerable().ToList().ForEach(row => {

                    ObjectRow newRow = preferenceTable.NewRow();

                    if(requireIdColumns)
                    {
                        //newRow[RMPreferenceConstants.entity_type_id] = Convert.ToInt32(row[RMPreferenceConstants.entity_type_id]);
                        newRow[RMPreferenceConstants.preference_id] = Convert.ToInt32(row[RMPreferenceConstants.preference_id]);
                        //newRow[RMPreferenceConstants.data_source_id] = Convert.ToInt32(row[RMPreferenceConstants.data_source_id]);
                    }

                    newRow[RMPreferenceConstants.Entity_Type_Name] = Convert.ToString(row[RMPreferenceConstants.Entity_Type_Name]);
                    doc = new XmlDocument();
                    doc.LoadXml(Convert.ToString(row["vendor_details"]));
                    XmlNodeList nodes = doc.GetElementsByTagName("control");

                    foreach (XmlNode node in nodes)
                    {
                        XmlAttribute xmlAttr = node.Attributes["key"];
                        string value = node.Attributes["text"].Value;
                        string columnName = mappings.Where(m => m.KeyName.Trim().Equals(xmlAttr.Value.Trim(), StringComparison.OrdinalIgnoreCase)).Select(m => m.PropertyName).FirstOrDefault();
                        newRow[columnName] = value;
                    }
                    preferenceTable.Rows.Add(newRow);
                });
                preferenceData.Tables.Add(preferenceTable);

                return preferenceData;
            }
            return null;
        }

        private void PopulateControlPropMapping(Dictionary<string, string> dict)
        {
            dict.Add("name", RMPreferenceConstants.Preference_Name);
            dict.Add("description", RMPreferenceConstants.Preference_Description);
            dict.Add("vendor", RMPreferenceConstants.Vendor_Type);
            dict.Add("apitype", RMPreferenceConstants.Request_Type);
            dict.Add("marketsector", RMPreferenceConstants.Market_Sector);
            dict.Add("datasource", RMPreferenceConstants.Data_Source_Name);
            dict.Add("vendoridentifier", RMPreferenceConstants.Vendor_Identifier);
            dict.Add("transport", RMPreferenceConstants.Transport);
            dict.Add("datarequesttype", RMPreferenceConstants.Data_Request_Type);
            dict.Add("assettype", RMPreferenceConstants.Asset_Type);
        }

        internal string PopulateRealTimePreferenceXML(string preferenceName, string preferenceDescription, string vendorType, string requestType, string marketSector, string dataSource, string vendorIdentifier, string transport, string dataRequestType, string assetType, Dictionary<int, string> dataSourceDict)
        {            
            int dataRequestId = 0;
            if (dataRequestType.SRMEqualWithIgnoreCase("Company Data"))
                dataRequestId = 2;

            XElement realTimePreference = new XElement("realtimepreference");

            // Preference Name
            XElement controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", preferenceName));
            controlElement.Add(new XAttribute("text", preferenceName));
            controlElement.Add(new XAttribute("key", "name"));
            controlElement.Add(new XAttribute("name", "Task Name"));
            controlElement.Add(new XAttribute("vendorpropertyname", ""));
            realTimePreference.Add(controlElement);

            // Preference Description
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", preferenceDescription));
            controlElement.Add(new XAttribute("text", preferenceDescription));
            controlElement.Add(new XAttribute("key", "description"));
            controlElement.Add(new XAttribute("name", "Task Description"));
            controlElement.Add(new XAttribute("vendorpropertyname", ""));
            realTimePreference.Add(controlElement);

            // Vendor Type
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", vendorType));
            controlElement.Add(new XAttribute("text", vendorType));
            controlElement.Add(new XAttribute("key", "vendor"));
            controlElement.Add(new XAttribute("name", "Vendor Type"));
            controlElement.Add(new XAttribute("vendorpropertyname", ""));
            realTimePreference.Add(controlElement);

            // Request Type
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", requestType));
            controlElement.Add(new XAttribute("text", requestType));
            controlElement.Add(new XAttribute("key", "apitype"));
            controlElement.Add(new XAttribute("name", "API Type"));
            controlElement.Add(new XAttribute("vendorpropertyname", "RequestType"));
            realTimePreference.Add(controlElement);

            if(vendorType.SRMEqualWithIgnoreCase(RMPreferenceConstants.Bloomberg))
            {
                // Market Sector
                controlElement = new XElement("control");
                controlElement.Add(new XAttribute("value", marketSector));
                controlElement.Add(new XAttribute("text", marketSector));
                controlElement.Add(new XAttribute("key", "marketsector"));
                controlElement.Add(new XAttribute("name", "Market Sector"));
                controlElement.Add(new XAttribute("vendorpropertyname", "MarketSector"));
                realTimePreference.Add(controlElement);
            }
            else
            {
                // Asset Type
                controlElement = new XElement("control");
                controlElement.Add(new XAttribute("value", assetType));
                controlElement.Add(new XAttribute("text", assetType));
                controlElement.Add(new XAttribute("key", "assettype"));
                controlElement.Add(new XAttribute("name", "Asset Type"));
                controlElement.Add(new XAttribute("vendorpropertyname", "AssetTypes"));
                realTimePreference.Add(controlElement);
            }

            // DataSource
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", dataSourceDict.AsEnumerable().Where(x => x.Value.SRMEqualWithIgnoreCase(dataSource)).Select(x => x.Key).FirstOrDefault()));
            controlElement.Add(new XAttribute("text", dataSource));
            controlElement.Add(new XAttribute("key", "datasource"));
            controlElement.Add(new XAttribute("name", "DataSource"));
            controlElement.Add(new XAttribute("vendorpropertyname", ""));
            realTimePreference.Add(controlElement);


            // Vendor Identifier
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", vendorIdentifier));
            controlElement.Add(new XAttribute("text", vendorIdentifier));
            controlElement.Add(new XAttribute("key", "vendoridentifier"));
            controlElement.Add(new XAttribute("name", "Vendor Identifier"));
            controlElement.Add(new XAttribute("vendorpropertyname", "InstrumentIdentifierType"));
            realTimePreference.Add(controlElement);

            // Transport
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", transport));
            controlElement.Add(new XAttribute("text", transport));
            controlElement.Add(new XAttribute("key", "transport"));
            controlElement.Add(new XAttribute("name", "Transport"));
            controlElement.Add(new XAttribute("vendorpropertyname", "TransportName"));
            realTimePreference.Add(controlElement);

            // Data Request Type
            controlElement = new XElement("control");
            controlElement.Add(new XAttribute("value", dataRequestId));
            controlElement.Add(new XAttribute("text", dataRequestType));
            controlElement.Add(new XAttribute("key", "datarequesttype"));
            controlElement.Add(new XAttribute("name", "Data Request Type"));
            controlElement.Add(new XAttribute("vendorpropertyname", "datarequesttype"));
            realTimePreference.Add(controlElement);

            return realTimePreference.ToString();
        }
    }
}
