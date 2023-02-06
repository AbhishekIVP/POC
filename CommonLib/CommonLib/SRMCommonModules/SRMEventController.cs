using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.transport;
using com.ivp.rad.utils;
using com.ivp.secm.exceptionsmanager;
using com.ivp.srmcommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace com.ivp.SRMCommonModules
{
    [Serializable]
    public class SRMEventInfo
    {
        //public SRMEventInfo()
        //{
        //    this.Attributes = new List<AttributeDetails>();
        //    this.Legs = new List<string>();
        //}
        [JsonConverter(typeof(StringEnumConverter))]
        public SRMEventActionType Action;

        [JsonConverter(typeof(StringEnumConverter))]
        public SRMEventModule Module;

        public string Type;
        public string ID;
        public string Key;

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? TimeStamp;

        public string User;

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? EffectiveStartDate;

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? EffectiveEndDate;

        public List<SRMEventAttributeDetails> Attributes = new List<SRMEventAttributeDetails>();
        public List<string> Legs = new List<string>();
        public bool IsCreate;
        public string Initiator;
        public List<string> PendingAt = new List<string>();

        [JsonConverter(typeof(SRMExceptionTypeConverter))]
        public SMExceptionsType ExceptionType;
        //public SRMEventLevel EventLevel;
        internal JsonSerializerSettings GetSerializer(SRMEventLevel EventLevel)
        {
            return DynamicContractResolver.DictSerializer[EventLevel][this.Action];
        }

    }

    public class SRMEventAttributeDetails
    {
        //public int AttributeId { get; set; }
        //public int LegId { get; set; }
        public string Name { get; set; }
        public string RealName { get; set; }
    }

    class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

    class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffffff";
        }
    }

    public class SRMExceptionTypeConverter : JsonConverter
    {
        static Dictionary<SMExceptionsType, string> dictExceptions = new Dictionary<SMExceptionsType, string>();
        static SRMExceptionTypeConverter()
        {
            dictExceptions.Add(SMExceptionsType.SECURITY_NOT_FOUND, "Validation");
            dictExceptions.Add(SMExceptionsType.COMPARE_AND_SHOW, "Vendor Mismatch");
            dictExceptions.Add(SMExceptionsType.REFERENCE_DATA, "Ref Data Missing");
            dictExceptions.Add(SMExceptionsType.NO_VENDOR_VALUE_FOUND, "No Vendor Value");
            dictExceptions.Add(SMExceptionsType.VALUE_CHANGED, "Value Changed");
            dictExceptions.Add(SMExceptionsType.SHOW_AS_EXCEPTION, "Show As Exception");
            dictExceptions.Add(SMExceptionsType.FIRST_VENDOR_VALUE_MISSING, "1st Vendor Value Missing");
            dictExceptions.Add(SMExceptionsType.CUSTOM_EXCEPTION, "Validations");
            dictExceptions.Add(SMExceptionsType.DATATYPE_MISMATCH, "Invalid Data");
            dictExceptions.Add(SMExceptionsType.UNDERLYING_DATA, "Underlier Missing");
            dictExceptions.Add(SMExceptionsType.ALERTS, "Alerts");
            dictExceptions.Add(SMExceptionsType.BASKET_CUSTOM_EXCEPTION, "Validations");
            dictExceptions.Add(SMExceptionsType.BASKET_ALERTS, "Alerts");
            dictExceptions.Add(SMExceptionsType.CONDITIONAL_FILTER_EXCEPTION, "Validations");
            dictExceptions.Add(SMExceptionsType.UNIQUENESS, "Duplicates");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(dictExceptions[(SMExceptionsType)value]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SMExceptionsType);
        }
    }

    public enum SRMEventActionType
    {
        Create = 1,
        Update = 2,
        Attribute_Update = 3,
        Delete = 4,
        Draft = 5,
        Workflow_Request_Initiate = 6,
        Workflow_Request_Approve = 7,
        Workflow_Request_Reject = 8,
        Workflow_Request_Edit = 9,
        Workflow_Request_Cancel = 10,
        Workflow_Request_Delete = 11,
        Exception_Raised = 12,
        Exception_Resolved = 13,
        Exception_Delete = 14,
        Exception_Suppress = 15,
        Exception_Unsuppress = 16
    }

    public enum SRMEventLevel
    {
        INSTRUMENT = 1,
        ATTRIBUTE = 2,
        LEG = 3
    }

    public enum SRMEventModule
    {
        [EnumMember(Value = "Securities")]
        Securities = 3,
        [EnumMember(Value = "Ref Data")]
        Ref_Data = 6,
        [EnumMember(Value = "Fund Data")]
        Fund_Data = 18,
        [EnumMember(Value = "Party Data")]
        Party_Data = 20

    }
    internal class DynamicContractResolver : DefaultContractResolver
    {
        IList<JsonProperty> _properties;
        IList<JsonProperty> _propertiesAttribute;
        internal static Dictionary<SRMEventLevel,Dictionary<SRMEventActionType, JsonSerializerSettings>> DictSerializer = new Dictionary<SRMEventLevel, Dictionary<SRMEventActionType, JsonSerializerSettings>>();

        static DynamicContractResolver()
        {
            var lstEventLevels = Enum.GetNames(typeof(SRMEventLevel)).Select(x => (SRMEventLevel)Enum.Parse(typeof(SRMEventLevel), x)).ToList();
            var lstEventActionTypes = Enum.GetNames(typeof(SRMEventActionType)).Select(x => (SRMEventActionType)Enum.Parse(typeof(SRMEventActionType), x)).ToList();

            foreach (var eventLevel in lstEventLevels)
            {
                if (!DynamicContractResolver.DictSerializer.ContainsKey(eventLevel))
                    DynamicContractResolver.DictSerializer.Add(eventLevel, new Dictionary<SRMEventActionType, JsonSerializerSettings>());
                foreach (var actionType in lstEventActionTypes)
                {
                    var resolver = new DynamicContractResolver(eventLevel, actionType);
                    DynamicContractResolver.DictSerializer[eventLevel].Add(actionType, new JsonSerializerSettings { ContractResolver = resolver });
                }
            }
        }

        public DynamicContractResolver(SRMEventLevel eventLevel, SRMEventActionType actionType)
        {
            this._properties = base.CreateProperties(typeof(SRMEventInfo), MemberSerialization.OptOut);
            this._propertiesAttribute = base.CreateProperties(typeof(SRMEventAttributeDetails), MemberSerialization.OptOut);
            if (actionType.Equals(SRMEventActionType.Delete))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs") && !x.PropertyName.Equals("EffectiveStartDate") && !x.PropertyName.Equals("EffectiveEndDate") && !x.PropertyName.Equals("IsCreate") && !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
            }
            else if (actionType.Equals(SRMEventActionType.Draft))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs") && !x.PropertyName.Equals("EffectiveEndDate") && !x.PropertyName.Equals("IsCreate") && !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();

            }
            else if (actionType.Equals(SRMEventActionType.Create))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs") && !x.PropertyName.Equals("EffectiveEndDate") && !x.PropertyName.Equals("IsCreate") && !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
            }
            else if (actionType.Equals(SRMEventActionType.Update))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs") && !x.PropertyName.Equals("IsCreate") && !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
            }
            else if (actionType.Equals(SRMEventActionType.Attribute_Update))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
            }
            else if (actionType.Equals(SRMEventActionType.Workflow_Request_Approve) || actionType.Equals(SRMEventActionType.Workflow_Request_Cancel) || actionType.Equals(SRMEventActionType.Workflow_Request_Delete) || actionType.Equals(SRMEventActionType.Workflow_Request_Edit) || actionType.Equals(SRMEventActionType.Workflow_Request_Initiate) || actionType.Equals(SRMEventActionType.Workflow_Request_Reject))
            {
                if (eventLevel == SRMEventLevel.INSTRUMENT || actionType.Equals(SRMEventActionType.Workflow_Request_Approve) || actionType.Equals(SRMEventActionType.Workflow_Request_Cancel) || actionType.Equals(SRMEventActionType.Workflow_Request_Delete) || actionType.Equals(SRMEventActionType.Workflow_Request_Reject))
                {
                    if (actionType.Equals(SRMEventActionType.Workflow_Request_Initiate))
                        this._properties = this._properties.Where(x => !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel") && !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs")).ToList();
                    else
                        this._properties = this._properties.Where(x => !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel") && !x.PropertyName.Equals("Attributes") && !x.PropertyName.Equals("Legs")).ToList();
                }
                else
                {
                    if (actionType.Equals(SRMEventActionType.Workflow_Request_Initiate))
                        this._properties = this._properties.Where(x => !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
                    else
                        this._properties = this._properties.Where(x => !x.PropertyName.Equals("ExceptionType") && !x.PropertyName.Equals("EventLevel")).ToList();
                }
            }
            else if (actionType.Equals(SRMEventActionType.Exception_Delete) || actionType.Equals(SRMEventActionType.Exception_Raised) || actionType.Equals(SRMEventActionType.Exception_Resolved) || actionType.Equals(SRMEventActionType.Exception_Suppress) || actionType.Equals(SRMEventActionType.Exception_Unsuppress))
            {
                this._properties = this._properties.Where(x => !x.PropertyName.Equals("EffectiveStartDate") && !x.PropertyName.Equals("PendingAt") && !x.PropertyName.Equals("Initiator") && !x.PropertyName.Equals("EffectiveEndDate") && !x.PropertyName.Equals("IsCreate") && !x.PropertyName.Equals("EventLevel")).ToList();
            }

        }

        static Type EventTypeMeta = typeof(SRMEventInfo);
        static Type AttributeTypeMeta = typeof(SRMEventAttributeDetails);
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (type == EventTypeMeta)
                return this._properties;
            else if (type == AttributeTypeMeta)
                return this._propertiesAttribute;
            return null;
        }

    }

    public class SRMEventController
    {
        private static void RaiseEventThread(Object objEventdetails)
        {
            SRMRaiseEventsInputInfo eventsDetails = (SRMRaiseEventsInputInfo)objEventdetails;
            if(!string.IsNullOrEmpty(eventsDetails.clientName))
                RMultiTanentUtil.ClientName = eventsDetails.clientName;
            Dictionary<int, string> dictInstrumentActionVsInstrumentName = new Dictionary<int, string>();
            HashSet<string> lstInstrumentTransport = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, IRTransport> dictTransportVsTansportObj = new Dictionary<string, IRTransport>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> dictSecIdVsSecurityKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<SRMEventLogs> commitSuccessInfoToLogs = new List<SRMEventLogs>();
            List<SRMEventLogs> commitFailureInfoToLogs = new List<SRMEventLogs>();
            Dictionary<int, Dictionary<string, AttrLegDetails>> dictActionVsTransportVsDetails = new Dictionary<int, Dictionary<string, AttrLegDetails>>();
            Dictionary<string, string> dictUserNameVsFullName = GetAllUserLoginNames();
            IRLogger mLogger = RLogFactory.CreateLogger("SRMEventController");
            bool throwException = false;

            try
            {
                if (eventsDetails != null)
                {
                    mLogger.Debug("RaiseEventThread -> Start");
                    ObjectSet ds = GetEventConfigurationDB(eventsDetails.instrumentTypeId, eventsDetails.moduleId);
                    var processInfo = GetProcessThreadInfo();

                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                    {
                        mLogger.Debug("RaiseEventThread : Event info from db returned.");
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            dictInstrumentActionVsInstrumentName = ds.Tables[0].AsEnumerable().ToDictionary(x => x.Field<int>("action_type_id"), y => y.Field<string>("instrument_type_name"));
                        }
                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            dictActionVsTransportVsDetails = ds.Tables[1].AsEnumerable().GroupBy(x => x.Field<int>("action_type_id")).ToDictionary(x => x.Key, x => x.GroupBy(y => y.Field<string>("transport_name")).ToDictionary(y => y.Key, y =>
                            {
                                var objDetails = new AttrLegDetails();
                                foreach (var objRow in y)
                                {
                                    if (objRow.Field<bool>("is_leg"))
                                        objDetails.legDets.Add(objRow.Field<string>("leg_name"));
                                    else
                                        objDetails.attrDets.Add(objRow.Field<string>("attribute_name"), objRow.Field<string>("display_name"));
                                }
                                return objDetails;
                            }));
                        }
                        //if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        //{
                        //    dictLegLevelActionVsLegName = ds.Tables[2].AsEnumerable().GroupBy(x => x.Field<int>("action_type_id")).ToDictionary(x => x.Key, x => new HashSet<string>(x.Select(y => y.Field<string>("leg_name")), StringComparer.OrdinalIgnoreCase));
                        //}
                        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        {
                            lstInstrumentTransport = new HashSet<string>(ds.Tables[3].AsEnumerable().Select(x => x.Field<string>("transport_name")), StringComparer.OrdinalIgnoreCase);
                        }
                        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                        {
                            mLogger.Debug("RaiseEventThread : GetTransport -> Start");
                            dictTransportVsTansportObj = ds.Tables[3].AsEnumerable().ToDictionary(x => x.Field<string>("transport_name"), x => new RTransportManager().GetTransport(x.Field<string>("transport_name")));
                            mLogger.Debug("RaiseEventThread : GetTransport -> End");
                        }

                        if (ds.Tables.Count > 4 && ds.Tables[4] != null && ds.Tables[4].Rows.Count > 0)
                        {
                            dictSecIdVsSecurityKey = ds.Tables[4].AsEnumerable().ToDictionary(x => x.Field<string>("sec_id"), y => y.Field<string>("security_key"));
                        }

                        if (eventsDetails.eventInfo != null && eventsDetails.eventInfo.Count > 0)
                        {
                            //mLogger.Debug("RaiseEventThread : Entered events loop.");
                            foreach (var objEvent in eventsDetails.eventInfo)
                            {
                                string json = string.Empty;
                                //DateTime currentdate = DateTime.Now.Date;

                                if (string.IsNullOrEmpty(objEvent.Key))
                                {
                                    if (dictSecIdVsSecurityKey.ContainsKey(objEvent.ID))
                                        objEvent.Key = dictSecIdVsSecurityKey[objEvent.ID];
                                    else
                                        objEvent.Key = objEvent.ID;
                                }
                                if (objEvent.TimeStamp == null)
                                    objEvent.TimeStamp = DateTime.Now;

                                if (objEvent.EffectiveStartDate != null)
                                {
                                    if (objEvent.EffectiveStartDate.Value.ToString("yyyy-MM-dd").Equals(DateTime.Now.Date.ToString("yyyy-MM-dd")))
                                    {
                                        objEvent.EffectiveStartDate = null;
                                    }

                                }
                                if (objEvent.EffectiveEndDate != null)
                                {
                                    if (objEvent.EffectiveEndDate.Value.ToString("yyyy-MM-dd").Equals(DateTime.Now.Date.ToString("yyyy-MM-dd")))
                                    {
                                        objEvent.EffectiveEndDate = null;
                                    }
                                }

                                objEvent.User = (dictUserNameVsFullName != null && dictUserNameVsFullName.Count > 0 && dictUserNameVsFullName.ContainsKey(objEvent.User)) ? dictUserNameVsFullName[objEvent.User] : objEvent.User;

                                if (objEvent.Action.Equals(SRMEventActionType.Attribute_Update))
                                {
                                    int ActionTypeId = -1;
                                    if (objEvent.IsCreate && dictInstrumentActionVsInstrumentName.ContainsKey((int)SRMEventActionType.Create))
                                    {
                                        objEvent.Action = SRMEventActionType.Create;
                                        ActionTypeId = (int)objEvent.Action;
                                        json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.INSTRUMENT));

                                    }
                                    if (!objEvent.IsCreate && dictInstrumentActionVsInstrumentName.ContainsKey((int)SRMEventActionType.Update))
                                    {
                                        objEvent.Action = SRMEventActionType.Update;
                                        ActionTypeId = (int)objEvent.Action;
                                        json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.INSTRUMENT));
                                    }

                                    objEvent.Action = SRMEventActionType.Attribute_Update;

                                    if (lstInstrumentTransport.Count > 0 && !string.IsNullOrEmpty(json))
                                    {
                                        if (json.Contains("undefined"))
                                            json = json.Replace("undefined", string.Empty);
                                        foreach (var instrumentTrans in lstInstrumentTransport)
                                        {
                                            try
                                            {
                                                if (dictTransportVsTansportObj.ContainsKey(instrumentTrans))
                                                    dictTransportVsTansportObj[instrumentTrans].SendMessage(json);

                                                commitSuccessInfoToLogs.Add(SRMCreateEventSuccessLogInfo(objEvent, ActionTypeId));
                                            }
                                            catch (Exception ex)
                                            {
                                                commitFailureInfoToLogs.Add(new SRMEventLogs() { timeStamp = objEvent.TimeStamp, transportName = instrumentTrans, failureReason = ex.Message, jsonData = json });

                                                if (throwException)
                                                    throw;
                                            }
                                        }

                                    }
                                }

                                if (dictInstrumentActionVsInstrumentName.ContainsKey((int)objEvent.Action))
                                {
                                    json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.INSTRUMENT));

                                    if (lstInstrumentTransport.Count > 0)
                                    {
                                        if (json.Contains("undefined"))
                                            json = json.Replace("undefined", string.Empty);
                                        foreach (var instrumentTrans in lstInstrumentTransport)
                                        {
                                            try
                                            {
                                                if (dictTransportVsTansportObj.ContainsKey(instrumentTrans))
                                                    dictTransportVsTansportObj[instrumentTrans].SendMessage(json);
                                            }
                                            catch (Exception ex)
                                            {
                                                //commitToSuccessLog = false;
                                                commitFailureInfoToLogs.Add(new SRMEventLogs() { timeStamp = objEvent.TimeStamp, transportName = instrumentTrans, failureReason = ex.Message, jsonData = json });

                                                if (throwException)
                                                    throw;
                                            }
                                        }
                                    }
                                }

                                if ((((objEvent.Attributes != null && objEvent.Attributes.Count > 0) || (objEvent.Legs != null && objEvent.Legs.Count > 0)) || (objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Approve) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Cancel) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Delete) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Reject))) && dictActionVsTransportVsDetails.ContainsKey((int)objEvent.Action))
                                {
                                    var lstAttr = objEvent.Attributes;
                                    var lstLegs = objEvent.Legs;

                                    foreach (var transport in dictActionVsTransportVsDetails[(int)objEvent.Action].Keys)
                                    {
                                        var lstSubscribedAttr = dictActionVsTransportVsDetails[(int)objEvent.Action][transport].attrDets;
                                        var lstSubscribedLegs = dictActionVsTransportVsDetails[(int)objEvent.Action][transport].legDets;

                                        if (objEvent.Attributes != null && objEvent.Attributes.Count > 0)
                                        {
                                            var lstAttrsToConsider = new List<SRMEventAttributeDetails>();
                                            foreach (var attr in objEvent.Attributes)
                                            {
                                                if (lstSubscribedAttr.ContainsKey(attr.RealName))
                                                {
                                                    if (string.IsNullOrEmpty(attr.Name))
                                                        attr.Name = lstSubscribedAttr[attr.RealName];
                                                    lstAttrsToConsider.Add(attr);
                                                }

                                            }
                                            objEvent.Attributes = lstAttrsToConsider;
                                        }
                                        if (objEvent.Legs != null && objEvent.Legs.Count > 0)
                                        {
                                            var lstLegsToConsider = new List<string>();
                                            foreach (var legs in objEvent.Legs)
                                            {
                                                if (lstSubscribedLegs.Contains(legs))
                                                {
                                                    lstLegsToConsider.Add(legs);
                                                }
                                            }
                                            objEvent.Legs = lstLegsToConsider;
                                        }

                                        if (((objEvent.Attributes != null && objEvent.Attributes.Count > 0) || (objEvent.Legs != null && objEvent.Legs.Count > 0)) || (objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Approve) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Cancel) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Delete) || objEvent.Action.Equals(SRMEventActionType.Workflow_Request_Reject)))
                                        {
                                            if (objEvent.Action.Equals(SRMEventActionType.Attribute_Update) && objEvent.IsCreate)
                                            {
                                                objEvent.EffectiveEndDate = null;
                                            }
                                            json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.ATTRIBUTE));



                                            if (objEvent.Action.Equals(SRMEventActionType.Attribute_Update) && objEvent.IsCreate)
                                            {
                                                if (json.Contains("EffectiveEndDate"))
                                                    json = json.Replace("\"EffectiveEndDate\": null,", string.Empty);
                                            }
                                            if (json.Contains("undefined"))
                                                json = json.Replace("undefined", string.Empty);

                                            try
                                            {
                                                if (dictTransportVsTansportObj.ContainsKey(transport))
                                                    dictTransportVsTansportObj[transport].SendMessage(json);
                                            }
                                            catch (Exception ex)
                                            {
                                                commitFailureInfoToLogs.Add(new SRMEventLogs() { timeStamp = objEvent.TimeStamp, transportName = transport, failureReason = ex.Message, jsonData = json });

                                                if (throwException)
                                                    throw;

                                            }

                                        }
                                        objEvent.Attributes = lstAttr;
                                        objEvent.Legs = lstLegs;
                                    }
                                }
                                //    if (lstAttr.Count > 0)
                                //    {                                      
                                //        //var lstSubscribedAttr = dictAttrLevelActionVsAttributes[(int)objEvent.Action];
                                //        foreach (string transportName in dictTransportVsAttr.Keys)
                                //        {
                                //            objEvent.Attributes = new List<SRMEventAttributeDetails>();
                                //            var lstSubscribedAttrPerTransport = dictTransportVsAttr[transportName];
                                //            foreach (var attr in lstSubscribedAttrPerTransport)
                                //            {
                                //                if (lstSubscribedAttrPerTransport.Contains(attr.RealName) && lstSubscribedAttr.ContainsKey(attr.RealName))
                                //                {
                                //                    SRMEventAttributeDetails adet = new SRMEventAttributeDetails();
                                //                    adet.RealName = attr.RealName;
                                //                    adet.Name = lstSubscribedAttr[attr.RealName];
                                //                    objEvent.Attributes.Add(adet);
                                //                }
                                //            }

                                //            if (objEvent.Attributes.Count > 0)
                                //            {
                                //                json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.ATTRIBUTE));

                                //                try
                                //                {
                                //                    if (dictTransportVsTansportObj.ContainsKey(transportName))
                                //                        dictTransportVsTansportObj[transportName].SendMessage(json);
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    commitFailureInfoToLogs.Add(new SRMEventLogs() { timeStamp = objEvent.TimeStamp, transportName = transportName, failureReason = ex.Message, jsonData = json });

                                //                }
                                //            }
                                //        }
                                //        objEvent.Attributes = lstAttr;
                                //    }
                                //}

                                //if (dictLegLevelActionVsLegName.ContainsKey((int)objEvent.Action))
                                //{
                                //    if (objEvent.Legs.Count > 0)
                                //    {
                                //        var lstLegs = objEvent.Legs;

                                //        var lstSubscribedLegs = dictLegLevelActionVsLegName[(int)objEvent.Action];
                                //        foreach (string transportName in dictTransportVsLeg.Keys)
                                //        {
                                //            objEvent.Legs = new List<string>();
                                //            var lstSubscribedLegsPerTransport = dictTransportVsLeg[transportName];
                                //            foreach (var leg in lstSubscribedLegs)
                                //            {
                                //                if (lstSubscribedLegsPerTransport.Contains(leg))
                                //                {
                                //                    objEvent.Legs.Add(leg);
                                //                }
                                //            }

                                //            if (objEvent.Legs.Count > 0)
                                //            {
                                //                json = JsonConvert.SerializeObject(objEvent, Formatting.Indented, objEvent.GetSerializer(SRMEventLevel.LEG));

                                //                try
                                //                {
                                //                    if (dictTransportVsTansportObj.ContainsKey(transportName))
                                //                        dictTransportVsTansportObj[transportName].SendMessage(json);
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    commitFailureInfoToLogs.Add(new SRMEventLogs() { timeStamp = objEvent.TimeStamp, transportName = transportName, failureReason = ex.Message, jsonData = json });
                                //                }
                                //            }
                                //        }
                                //        objEvent.Legs = lstLegs;
                                //    }
                                //}
                            }

                            if (commitSuccessInfoToLogs != null && commitSuccessInfoToLogs.Count > 0)
                            {
                                int result = new SRMEventController().CommitEventInfoToSuccessLogs(commitSuccessInfoToLogs, processInfo);
                            }
                            if (commitFailureInfoToLogs != null && commitFailureInfoToLogs.Count > 0)
                            {
                                int result = new SRMEventController().CommitEventInfoToFailureLogs(commitFailureInfoToLogs);
                            }
                        }
                    }
                    mLogger.Debug("RaiseEventThread -> End.");
                }


            }
            catch (Exception ex)
            {
                if (throwException)
                    throw;
            }

        }
        public static void RaiseEvent(SRMRaiseEventsInputInfo eventsDetails)
        {
            //method to fetch configuration
            try
            {
                if (eventsDetails != null)
                {
                    if (eventsDetails.moduleId == 0)
                        throw new Exception("invalid moduleId");

                    if (eventsDetails.instrumentTypeId == 0)
                        throw new Exception("invalid instrumentTypeId");

                    eventsDetails.clientName = SRMMTConfig.GetClientName();
                    if (eventsDetails.callInThread)
                    {
                        Thread taskThread = new Thread(new ParameterizedThreadStart(RaiseEventThread));
                        taskThread.Start(eventsDetails);
                    }
                    else
                    {
                        RaiseEventThread(eventsDetails);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private static ObjectSet GetEventConfigurationDB(int instrumentTypeId, int moduleId = 3)
        {
            DataSet dsResult = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC [dbo].[REFM_GetEventsConfiguration] '{0}', '{1}'", instrumentTypeId, moduleId), ConnectionConstants.RefMaster_Connection);
            return srmcommon.SRMCommon.ConvertDataSetToObjectSet(dsResult);
        }
        private static ProcessThreadInfo GetProcessThreadInfo()
        {
            ProcessThreadInfo pInfo = new ProcessThreadInfo();
            var stacktrace = new StackTrace().ToString();
            Process currentProcess = new Process();
            currentProcess = Process.GetCurrentProcess();
            pInfo.machineName = currentProcess.MachineName;
            pInfo.processId = currentProcess.Id;
            pInfo.processName = currentProcess.ProcessName;
            pInfo.stackTrace = stacktrace;
            pInfo.threadId = Thread.CurrentThread.ManagedThreadId;
            return pInfo;
        }
        private static SRMEventLogs SRMCreateEventSuccessLogInfo(SRMEventInfo objEventsInfo, int actionTypeId)
        {
            SRMEventLogs successLogs = new SRMEventLogs();
            successLogs.actionTypeId = actionTypeId;
            successLogs.userName = objEventsInfo.User;
            successLogs.instrumentId = objEventsInfo.ID;
            successLogs.moduleId = (int)objEventsInfo.Module;
            successLogs.timeStamp = objEventsInfo.TimeStamp;
            return successLogs;
        }

        private int CommitEventInfoToSuccessLogs(List<SRMEventLogs> successLogs, ProcessThreadInfo processInfo)
        {
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("action_type_id", typeof(Int32));
                dt.Columns.Add("user_name", typeof(string));
                dt.Columns.Add("instrument_id", typeof(string));
                dt.Columns.Add("module_id", typeof(Int32));
                dt.Columns.Add("timestamp", typeof(DateTime));

                foreach (var info in successLogs)
                {
                    DataRow dr = dt.NewRow();
                    dr["action_type_id"] = info.actionTypeId;
                    dr["user_name"] = info.userName;
                    dr["instrument_id"] = info.instrumentId;
                    dr["module_id"] = info.moduleId;
                    dr["timestamp"] = info.timeStamp;
                    dt.Rows.Add(dr);
                }

                CommonDALWrapper.ExecuteQuery(@"IF(OBJECT_ID('tempdb..#tempSuceessEventLog') IS NOT NULL)
                DROP TABLE #tempSuceessEventLog
                CREATE TABLE #tempSuceessEventLog(action_type_id INT,user_name VARCHAR(200),instrument_id VARCHAR(100),module_id INT,timestamp DATETIME);", CommonQueryType.Insert, con);

                CommonDALWrapper.ExecuteBulkUpload("#tempSuceessEventLog", dt, con);
                var query = string.Format(@"DECLARE @stack_trace VARCHAR(MAX) = '{0}' ,@machine_name VARCHAR(200) = '{1}' ,@process_id INT = {2}, @process_name VARCHAR(200)='{3}', @thread_id INT = {4} ;
    
                INSERT INTO IVPRefMaster.dbo.ivp_refm_event_log
                SELECT action_type_id,user_name,instrument_id,module_id,timestamp,@stack_trace,@machine_name,@process_id,@process_name,@thread_id
                FROM #tempSuceessEventLog ", processInfo.stackTrace, processInfo.machineName, processInfo.processId, processInfo.processName, processInfo.threadId);

                CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, con);
                CommonDALWrapper.ExecuteQuery("DROP TABLE #tempSuceessEventLog", CommonQueryType.Insert, con);

                con.CommitTransaction();
            }
            catch (Exception ex)
            {
                con.RollbackTransaction();
                return 0;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
            return 1;

        }

        private int CommitEventInfoToFailureLogs(List<SRMEventLogs> failureLogs)
        {
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("timestamp", typeof(DateTime));
                dt.Columns.Add("transport_name", typeof(string));
                dt.Columns.Add("failure_reason", typeof(string));
                dt.Columns.Add("json_data", typeof(string));

                foreach (var info in failureLogs)
                {
                    DataRow dr = dt.NewRow();
                    dr["timestamp"] = info.timeStamp;
                    dr["transport_name"] = info.transportName;
                    dr["failure_reason"] = info.failureReason;
                    dr["json_data"] = info.jsonData;
                    dt.Rows.Add(dr);
                }

                CommonDALWrapper.ExecuteQuery(@"IF(OBJECT_ID('tempdb..#tempFailureEventLog') IS NOT NULL)
                DROP TABLE #tempFailureEventLog
                CREATE TABLE #tempFailureEventLog(timestamp DATETIME,transport_name VARCHAR(100),failure_reason VARCHAR(MAX),json_data VARCHAR(MAX));", CommonQueryType.Insert, con);

                CommonDALWrapper.ExecuteBulkUpload("#tempFailureEventLog", dt, con);

                var query = string.Format(@"    
                INSERT INTO IVPRefMaster.dbo.ivp_refm_event_status_log
                SELECT timestamp,transport_name,failure_reason,json_data
                FROM #tempFailureEventLog ");

                CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, con);
                CommonDALWrapper.ExecuteQuery("DROP TABLE #tempFailureEventLog", CommonQueryType.Insert, con);

                con.CommitTransaction();
            }
            catch (Exception ex)
            {
                con.RollbackTransaction();
                return 0;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
            return 1;

        }

        private static Dictionary<string, string> GetAllUserLoginNames()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();
            if (lstUserInfo != null)
            {
                lstUserInfo.ForEach(user =>
                {
                    if (!dict.ContainsKey(user.UserLoginName))
                    {
                        dict.Add(user.UserLoginName, (user.UserName.ToLower() == "admin" ? user.FirstName.Trim() : user.FullName.Trim()) + " (" + user.UserName.Trim() + ")");
                    }
                });
            }

            return dict;
        }

        public static SRMEventConfigurationOutputInfo GetEventConfiguration(SRMEventConfigurationInputInfo inputInfo)
        {
            if (inputInfo == null)
                throw new Exception("inputInfo cannot be null");

            if (inputInfo.moduleId == 0)
                throw new Exception("invalid moduleId");

            if (inputInfo.instrumentTypeId == 0)
                throw new Exception("invalid instrumentTypeId");

            string query = @"SELECT ed.action_type_id
FROM IVPRefMaster.dbo.ivp_refm_event_config_master em
INNER JOIN IVPRefMaster.dbo.ivp_refm_event_config_details ed
ON em.config_master_id = ed.config_master_id AND em.level_type_id = 1
WHERE em.module_id = " + inputInfo.moduleId + @" AND em.instrument_type_id = " + inputInfo.instrumentTypeId + @"

SELECT ed.action_type_id,em.dependent_id AS attribute_id
FROM IVPRefMaster.dbo.ivp_refm_event_config_master em
INNER JOIN IVPRefMaster.dbo.ivp_refm_event_config_details ed
ON em.config_master_id = ed.config_master_id AND em.level_type_id = 2
WHERE em.module_id = " + inputInfo.moduleId + @" AND em.instrument_type_id = " + inputInfo.instrumentTypeId;

            if (inputInfo.moduleId == 3)
            {
                query += @"
SELECT ed.action_type_id,st.table_display_name AS leg_name
FROM IVPRefMaster.dbo.ivp_refm_event_config_master em
INNER JOIN IVPRefMaster.dbo.ivp_refm_event_config_details ed
ON em.config_master_id = ed.config_master_id AND em.level_type_id = 3
INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table st
ON st.sectype_table_id = em.dependent_id AND em.is_additional_leg = 0
WHERE em.module_id = " + inputInfo.moduleId + @" AND em.instrument_type_id = " + inputInfo.instrumentTypeId + @"
UNION ALL
SELECT ed.action_type_id,st.additional_legs_name AS leg_name
FROM IVPRefMaster.dbo.ivp_refm_event_config_master em
INNER JOIN IVPRefMaster.dbo.ivp_refm_event_config_details ed
ON em.config_master_id = ed.config_master_id AND em.level_type_id = 3
INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs st
ON st.additional_legs_id = em.dependent_id AND em.is_additional_leg = 1
WHERE em.module_id = " + inputInfo.moduleId + @" AND em.instrument_type_id = " + inputInfo.instrumentTypeId;
            }
            else
            {
                query += @"
                SELECT ed.action_type_id,leg.entity_display_name AS leg_name
                FROM IVPRefMaster.dbo.ivp_refm_event_config_master em
                INNER JOIN IVPRefMaster.dbo.ivp_refm_event_config_details ed
                ON em.config_master_id = ed.config_master_id AND em.level_type_id = 3
                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type leg
                ON(leg.entity_type_id = em.dependent_id)
                WHERE em.module_id = " + inputInfo.moduleId + @" AND em.instrument_type_id = " + inputInfo.instrumentTypeId;
            }

            DataSet dsResult = commom.commondal.CommonDALWrapper.ExecuteSelectQuery(query, commom.commondal.ConnectionConstants.RefMaster_Connection);

            var resultInfo = Enum.GetNames(typeof(SRMEventActionType)).ToDictionary(x => (SRMEventActionType)Enum.Parse(typeof(SRMEventActionType), x), x => new SRMEventPreferenceInfo());

            foreach (DataRow dr in dsResult.Tables[0].Rows)
            {
                resultInfo[(SRMEventActionType)dr.Field<int>("action_type_id")].IsSecurityLevel = true;
            }

            foreach (DataRow dr in dsResult.Tables[1].Rows)
            {
                resultInfo[(SRMEventActionType)dr.Field<int>("action_type_id")].LstAttributeLevel.Add(dr.Field<int>("attribute_id"));
            }

            foreach (DataRow dr in dsResult.Tables[2].Rows)
            {
                resultInfo[(SRMEventActionType)dr.Field<int>("action_type_id")].LstLegLevel.Add(dr.Field<string>("leg_name"));
            }

            return new SRMEventConfigurationOutputInfo { DictConfiguration = resultInfo };
        }

        public static List<SRMEventInstrumentTypesConfigList> GetEventInstrumentTypesConfigList(List<int> moduleIds)
        {
            if (moduleIds.Count == 0)
                return null;
            List<SRMEventInstrumentTypesConfigList> result = new List<SRMEventInstrumentTypesConfigList>();
            SRMEventInstrumentTypesConfigList moduleEntry;
            SRMEventInstrumentTypesConfig instrumentTypeEntry;
            foreach (var moduleId in moduleIds)
            {
                DataTable dt = CommonDALWrapper.ExecuteSelectQuery(String.Format(@"exec IVPRefMaster.dbo.REFM_GetInstrumentTypesWithEventConfigFlag {0}", moduleId),
                    ConnectionConstants.RefMaster_Connection).Tables[0];
                moduleEntry = new SRMEventInstrumentTypesConfigList();
                moduleEntry.moduleId = moduleId;
                moduleEntry.instrumentTypes = new List<SRMEventInstrumentTypesConfig>();

                foreach (DataRow row in dt.Rows)
                {
                    instrumentTypeEntry = new SRMEventInstrumentTypesConfig();
                    instrumentTypeEntry.instrumentId = Int32.Parse(row["instrument_id"].ToString());
                    instrumentTypeEntry.instrumentName = row["display_name"].ToString();
                    instrumentTypeEntry.isConfigured = Boolean.Parse(row["is_configured"].ToString());
                    moduleEntry.instrumentTypes.Add(instrumentTypeEntry);
                }
                result.Add(moduleEntry);
            }

            return result;
        }

        public static void DeleteEventSettingForInstrumentType(int moduleId, int instrumentTypeId)
        {
            try
            {
                CommonDALWrapper.ExecuteSelectQuery(String.Format(@"exec IVPRefMaster.dbo.REFM_deleteEventConfigForInstrumentType {0},{1}", moduleId, instrumentTypeId),
                    ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static SRMEventsAllLevelActions GetEventConfigForInstrumentType(int moduleId, int instrumentTypeId)
        {
            DataTable table;
            var result = new SRMEventsAllLevelActions();
            result.SelectedInstrumentActions = new List<int>();
            result.SelectedInstrumentQueue = new List<string>();
            result.AttributeLevel = new List<SRMEventsAttributeLegActions>();
            result.LegLevel = new List<SRMEventsAttributeLegActions>();
            SRMEventsAttributeLegActions attributeLegLevelAction;
            table = CommonDALWrapper.ExecuteSelectQuery(String.Format(@"exec IVPRefMaster.dbo.REFM_GetInstrumentLevelDataForEvents {0},{1}", moduleId, instrumentTypeId),
                ConnectionConstants.RefMaster_Connection).Tables[0];

            foreach (DataRow row in table.Rows)
            {
                result.SelectedInstrumentActions = Array.ConvertAll(row["action_type_id"].ToString().Split(','), int.Parse).ToList<int>();
                result.SelectedInstrumentQueue = row["transport_name"].ToString().Split(',').ToList<string>();
                result.ConfigId = Int32.Parse(row["config_master_id"].ToString());
            }

            table = CommonDALWrapper.ExecuteSelectQuery(String.Format(@"exec IVPRefMaster.dbo.REFM_GetAttributeLevelDataForEvents {0},{1}", moduleId, instrumentTypeId),
                ConnectionConstants.RefMaster_Connection).Tables[0];
            foreach (DataRow row in table.Rows)
            {
                attributeLegLevelAction = new SRMEventsAttributeLegActions();
                attributeLegLevelAction.DependentId = Int32.Parse(row["attribute_id"].ToString());
                attributeLegLevelAction.DisplayName = row["display_name"].ToString();
                if (!row.IsNull("config_master_id"))
                {
                    attributeLegLevelAction.ConfigId = Int32.Parse(row["config_master_id"].ToString());
                    attributeLegLevelAction.SelectedAttributeLegQueue = row["transport_name"].ToString().Split(',').ToList<string>();
                    attributeLegLevelAction.SelectedAttributeLegActions = Array.ConvertAll(row["action_type_id"].ToString().Split(','), int.Parse).ToList<int>();
                }
                else
                {
                    attributeLegLevelAction.SelectedAttributeLegQueue = new List<string>();
                    attributeLegLevelAction.SelectedAttributeLegActions = new List<int>();
                }
                result.AttributeLevel.Add(attributeLegLevelAction);
            }

            table = CommonDALWrapper.ExecuteSelectQuery(String.Format(@"exec IVPRefMaster.dbo.[REFM_GetLegLevelDataForEvents] {0},{1}", moduleId, instrumentTypeId),
                ConnectionConstants.RefMaster_Connection).Tables[0];
            foreach (DataRow row in table.Rows)
            {
                attributeLegLevelAction = new SRMEventsAttributeLegActions();
                attributeLegLevelAction.DependentId = Int32.Parse(row["leg_id"].ToString());
                attributeLegLevelAction.DisplayName = row["display_name"].ToString();
                if (!row.IsNull("config_master_id"))
                {
                    attributeLegLevelAction.ConfigId = Int32.Parse(row["config_master_id"].ToString());
                    attributeLegLevelAction.SelectedAttributeLegQueue = row["transport_name"].ToString().Split(',').ToList<string>();
                    attributeLegLevelAction.SelectedAttributeLegActions = Array.ConvertAll(row["action_type_id"].ToString().Split(','), int.Parse).ToList<int>();
                }
                else
                {
                    attributeLegLevelAction.SelectedAttributeLegQueue = new List<string>();
                    attributeLegLevelAction.SelectedAttributeLegActions = new List<int>();
                }
                attributeLegLevelAction.IsAdditional = Boolean.Parse(row["is_additional"].ToString());
                result.LegLevel.Add(attributeLegLevelAction);
            }
            return result;
        }

        public static List<string> getTransportNames()
        {
            return new RTransportService().GetAllQueueTransportName();
        }

        public static void SaveEventConfigForInstrumentType(int moduleId, int instrumentTypeId, SRMEventsAllLevelActions config, string username)
        {
            if (config == null)
                return;
            DataTable table = new DataTable();
            table.Columns.Add("config_master_id", typeof(Int32));
            table.Columns.Add("module_id", typeof(Int32));
            table.Columns.Add("instrument_type_id", typeof(Int32));
            table.Columns.Add("level_type_id", typeof(Int32));
            table.Columns.Add("dependent_id", typeof(Int32));
            table.Columns.Add("is_additional_leg", typeof(bool));
            table.Columns.Add("action_type_id", typeof(Int32));
            table.Columns.Add("transport_name", typeof(string));
            table.Columns.Add("performAction", typeof(Int32));


            DataRow row;
            #region Add Instrument Level
            if (config.ConfigId > 0)
            {
                if (config.SelectedInstrumentQueue.Count > 0 && config.SelectedInstrumentActions.Count > 0)
                    foreach (var queue in config.SelectedInstrumentQueue)
                        foreach (var actionId in config.SelectedInstrumentActions)
                        {
                            row = table.NewRow();
                            row["module_id"] = moduleId;
                            row["instrument_type_id"] = instrumentTypeId;
                            row["dependent_id"] = instrumentTypeId;
                            row["level_type_id"] = 1;
                            row["is_additional_leg"] = false;
                            row["transport_name"] = queue;
                            row["action_type_id"] = actionId;
                            row["performAction"] = SaveActionUpdate;
                            row["config_master_id"] = config.ConfigId;
                            table.Rows.Add(row);
                        }
                else
                {
                    row = table.NewRow();
                    row["module_id"] = moduleId;
                    row["instrument_type_id"] = instrumentTypeId;
                    row["dependent_id"] = instrumentTypeId;
                    row["level_type_id"] = 1;
                    row["is_additional_leg"] = false;
                    row["performAction"] = SaveActionDelete;
                    row["config_master_id"] = config.ConfigId;
                    table.Rows.Add(row);
                }
            }
            else if (config.ConfigId < 0)
            {
                if (config.SelectedInstrumentQueue.Count > 0 && config.SelectedInstrumentActions.Count > 0)
                    foreach (var queue in config.SelectedInstrumentQueue)
                        foreach (var actionId in config.SelectedInstrumentActions)
                        {
                            row = table.NewRow();
                            row["module_id"] = moduleId;
                            row["instrument_type_id"] = instrumentTypeId;
                            row["dependent_id"] = instrumentTypeId;
                            row["level_type_id"] = 1;
                            row["is_additional_leg"] = false;
                            row["transport_name"] = queue;
                            row["action_type_id"] = actionId;
                            row["performAction"] = SaveActionInsert;
                            table.Rows.Add(row);
                        }
            }
            #endregion

            #region Add Attribute Level
            foreach (var attribute in config.AttributeLevel)
            {
                if (attribute.ConfigId > 0)
                {
                    if (attribute.SelectedAttributeLegQueue.Count > 0 && attribute.SelectedAttributeLegActions.Count > 0)
                        foreach (var queue in attribute.SelectedAttributeLegQueue)
                            foreach (var actionId in attribute.SelectedAttributeLegActions)
                            {
                                row = table.NewRow();
                                row["module_id"] = moduleId;
                                row["instrument_type_id"] = instrumentTypeId;
                                row["dependent_id"] = attribute.DependentId;
                                row["level_type_id"] = 2;
                                row["is_additional_leg"] = attribute.IsAdditional;
                                row["transport_name"] = queue;
                                row["action_type_id"] = actionId;
                                row["performAction"] = SaveActionUpdate;
                                row["config_master_id"] = attribute.ConfigId;
                                table.Rows.Add(row);
                            }
                    else
                    {
                        row = table.NewRow();
                        row["module_id"] = moduleId;
                        row["instrument_type_id"] = instrumentTypeId;
                        row["dependent_id"] = attribute.DependentId;
                        row["level_type_id"] = 2;
                        row["is_additional_leg"] = attribute.IsAdditional;
                        row["performAction"] = SaveActionDelete;
                        row["config_master_id"] = attribute.ConfigId;
                        table.Rows.Add(row);
                    }
                }
                else if (attribute.ConfigId < 0)
                {
                    if (attribute.SelectedAttributeLegQueue.Count > 0 && attribute.SelectedAttributeLegActions.Count > 0)
                        foreach (var queue in attribute.SelectedAttributeLegQueue)
                            foreach (var actionId in attribute.SelectedAttributeLegActions)
                            {
                                row = table.NewRow();
                                row["module_id"] = moduleId;
                                row["instrument_type_id"] = instrumentTypeId;
                                row["dependent_id"] = attribute.DependentId;
                                row["level_type_id"] = 2;
                                row["is_additional_leg"] = attribute.IsAdditional;
                                row["transport_name"] = queue;
                                row["action_type_id"] = actionId;
                                row["performAction"] = SaveActionInsert;
                                table.Rows.Add(row);
                            }
                }
            }
            #endregion

            #region Add Leg Level
            foreach (var leg in config.LegLevel)
            {
                if (leg.ConfigId > 0)
                {
                    if (leg.SelectedAttributeLegQueue.Count > 0 && leg.SelectedAttributeLegActions.Count > 0)
                        foreach (var queue in leg.SelectedAttributeLegQueue)
                            foreach (var actionId in leg.SelectedAttributeLegActions)
                            {
                                row = table.NewRow();
                                row["module_id"] = moduleId;
                                row["instrument_type_id"] = instrumentTypeId;
                                row["dependent_id"] = leg.DependentId;
                                row["level_type_id"] = 3;
                                row["is_additional_leg"] = leg.IsAdditional;
                                row["transport_name"] = queue;
                                row["action_type_id"] = actionId;
                                row["performAction"] = SaveActionUpdate;
                                row["config_master_id"] = leg.ConfigId;
                                table.Rows.Add(row);
                            }
                    else
                    {
                        row = table.NewRow();
                        row["module_id"] = moduleId;
                        row["instrument_type_id"] = instrumentTypeId;
                        row["dependent_id"] = leg.DependentId;
                        row["level_type_id"] = 3;
                        row["is_additional_leg"] = leg.IsAdditional;
                        row["performAction"] = SaveActionDelete;
                        row["config_master_id"] = leg.ConfigId;
                        table.Rows.Add(row);
                    }
                }
                else if (leg.ConfigId < 0)
                {
                    if (leg.SelectedAttributeLegQueue.Count > 0 && leg.SelectedAttributeLegActions.Count > 0)
                        foreach (var queue in leg.SelectedAttributeLegQueue)
                            foreach (var actionId in leg.SelectedAttributeLegActions)
                            {
                                row = table.NewRow();
                                row["module_id"] = moduleId;
                                row["instrument_type_id"] = instrumentTypeId;
                                row["dependent_id"] = leg.DependentId;
                                row["level_type_id"] = 3;
                                row["is_additional_leg"] = leg.IsAdditional;
                                row["transport_name"] = queue;
                                row["action_type_id"] = actionId;
                                row["performAction"] = SaveActionInsert;
                                table.Rows.Add(row);
                            }
                }
            }
            #endregion


            string stagingTableName = "[IVPRefMaster].[dbo].[event_config_temp" + Guid.NewGuid().ToString() + "]";
            string createStagingTable = "create table " + stagingTableName + "(config_master_id int,module_id int,instrument_type_id int,dependent_id int,level_type_id int,is_additional_leg bit,action_type_id int,transport_name varchar(200),performAction int)";
            try
            {
                CommonDALWrapper.ExecuteQuery(createStagingTable, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteBulkUpload(stagingTableName, table, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteSelectQuery(String.Format("exec IVPRefMaster.dbo.REFM_InsertEventConfig '{0}','{1}'", stagingTableName, username), ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteQuery("IF(OBJECT_ID('" + stagingTableName + "') IS NOT NULL) DROP TABLE " + stagingTableName, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public const int SaveActionNone = 0;
        public const int SaveActionUpdate = 1;
        public const int SaveActionDelete = 2;
        public const int SaveActionInsert = 3;
    }


    public class SRMEventPreferenceInfo
    {
        public SRMEventActionType ActionType;
        public bool IsSecurityLevel;
        public HashSet<int> LstAttributeLevel = new HashSet<int>();
        public HashSet<string> LstLegLevel = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
    [DataContract]
    public class SRMEventInstrumentTypesConfigList
    {
        [DataMember]
        public int moduleId { get; set; }
        [DataMember]
        public List<SRMEventInstrumentTypesConfig> instrumentTypes { get; set; }

    }
    [DataContract]
    public class SRMEventInstrumentTypesConfig
    {
        [DataMember]
        public int instrumentId { get; set; }
        [DataMember]
        public string instrumentName { get; set; }
        [DataMember]
        public bool isConfigured { get; set; }
    }

    [DataContract]
    public class SRMEventsAllLevelActions
    {
        [DataMember]
        public List<int> SelectedInstrumentActions { get; set; }
        [DataMember]
        public List<string> SelectedInstrumentQueue { get; set; }
        [DataMember]
        public int ConfigId { get; set; }
        [DataMember]
        public List<SRMEventsAttributeLegActions> AttributeLevel { get; set; }
        [DataMember]
        public List<SRMEventsAttributeLegActions> LegLevel { get; set; }
    }
    [DataContract]
    public class SRMEventsAttributeLegActions
    {
        [DataMember]
        public int DependentId { get; set; }
        [DataMember]
        public bool IsAdditional { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public int ConfigId { get; set; }
        [DataMember]
        public List<int> SelectedAttributeLegActions { get; set; }
        [DataMember]
        public List<string> SelectedAttributeLegQueue { get; set; }
    }

    public class SRMDeletedExceptionEventInputInfo
    {
        public int secTypeID;
        public string userName;
        public ObjectSet dsDeleted;
        public Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>> dictEvents = new Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>>();
        public DateTime? dtModifiedOn;
    }

    public class SRMDeletedExceptionEventOutputInfo
    {
        public Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>> dictEvents = new Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>>();
    }

    public class SRMEventConfigurationInputInfo
    {
        public int instrumentTypeId;
        public int moduleId = 3;
    }

    public class SRMEventConfigurationOutputInfo
    {
        public Dictionary<SRMEventActionType, SRMEventPreferenceInfo> DictConfiguration = new Dictionary<SRMEventActionType, SRMEventPreferenceInfo>();
    }

    public class SRMRaiseEventsInputInfo
    {
        public int instrumentTypeId;
        public List<SRMEventInfo> eventInfo = new List<SRMEventInfo>();
        public int moduleId = 3;
        public bool callInThread = true;
        public string clientName;
    }

    public class SRMEventLogs
    {
        public int actionTypeId;
        public string userName;
        public string instrumentId;
        public int moduleId;
        public DateTime? timeStamp;
        public string jsonData;
        public string failureReason;
        public string transportName;
    }
    public class ProcessThreadInfo
    {
        public string stackTrace;
        public string machineName;
        public int processId;
        public string processName;
        public int threadId;
    }

    public class AttrLegDetails
    {
        public Dictionary<string, string> attrDets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> legDets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //public string LegName;
        //public string processName;
        //public bool isLeg;
    }

}
