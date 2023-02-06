using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;
using com.ivp.srmcommon;

namespace com.ivp.common
{
    public static class SRMWorkflowUtils
    {
        public static Dictionary<int, WorkflowType> workflowTypes = new Dictionary<int, WorkflowType>();

        public static string GetInstrumentXML(List<WorkflowQueueInfo> queueInfo)
        {
            string xmlString = null;

            XElement xml = new XElement("root", from item in queueInfo
                                                select new XElement("row",
                                                   new XAttribute("wi", item.WorkflowInstanceID),
                                                   new XAttribute("rb", item.RequestedBy),
                                                   new XAttribute("ro", item.RequestedOn.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                                                   item.EffectiveStartDate != null ? new XAttribute("ef", item.EffectiveStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")) : null,
                                                   item.EffectiveEndDate != null ? new XAttribute("ee", item.EffectiveEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")) : null,
                                                   new XAttribute("ti", item.TypeID),
                                                   !string.IsNullOrEmpty(item.ClonedFrom) ? new XAttribute("cf", item.ClonedFrom) : null,
                                                   new XAttribute("de", item.DeleteExisting),
                                                   new XAttribute("ts", item.CopyTimeSeries),
                                                   new XAttribute("ii", item.InstrumentID),
                                                   new XAttribute("ri", item.RadInstanceID),
                                                   new XAttribute("fa", item.IsFinalApproval),
                                                   new XAttribute("rm", item.RuleMappingID)


                                       ));

            xmlString = xml.ToString();

            return xmlString;
        }

        public static List<SRMWorkflowCountPerType> GetDefaultCount()
        {
            List<SRMWorkflowCountPerType> lst = new List<common.SRMWorkflowCountPerType>();
            SRMWorkflowCountPerType cpt = null;
            WorkflowType wfType;
            workflowTypes = SetWorkflowTypeDictionary(workflowTypes);

            for (int i = 0; i <= 5; i++)
            {
                wfType = workflowTypes[i];
            }


            return lst;
        }

        public static List<WorkflowQueueData> PrepareWorkflowQueueData(DataTable dtQueueData, string userName, int moduleID, string dateFormat,string dateTimeFormat)
        {
            List<WorkflowQueueData> queueData = new List<WorkflowQueueData>();
            WorkflowQueueData queue = null;
            Dictionary<int, List<string>> dictPossibleActions = null;
            List<int> typeIds = new List<int>();
            List<int> instancesPerTypeID = new List<int>();
            List<AttributeVsValue> attrVsValue = null;
            List<AttributeVsValue> finalAttributeValues = new List<AttributeVsValue>();

            dtQueueData.AsEnumerable().ToList().ForEach(q =>
            {
                queue = new WorkflowQueueData();
                queue.EffectiveStartDate = null;
                queue.EffectiveEndDate = null;
                queue.ClonedFrom = null;
                queue.DeleteExisting = false;
                queue.CopyTimeSeries = false;
                queue.QueueId = Convert.ToInt32(q["queue_id"]);
                queue.InstrumentId = Convert.ToString(q["instrument_id"]);
                queue.RadInstanceId = Convert.ToInt32(q["rad_workflow_instance_id"]);
                queue.WorkflowInstanceID = Convert.ToInt32(q["workflow_instance_id"]);
                queue.TypeID = Convert.ToInt32(q["type_id"]);
                queue.TypeName = Convert.ToString(q["type_name"]);
                queue.RequestedByReal = Convert.ToString(q["requested_by"]);
                queue.RequestedBy = SRMCommonRAD.GetUserDisplayNameFromUserName(Convert.ToString(q["requested_by"]));
                queue.RequestedOn = Convert.ToDateTime(q["requested_on"]);
                if (!string.IsNullOrEmpty(Convert.ToString(q["effective_from_date"])))
                    queue.EffectiveStartDate = Convert.ToDateTime(q["effective_from_date"]);
                if (!string.IsNullOrEmpty(Convert.ToString(q["effective_to_date"])))
                    queue.EffectiveEndDate = Convert.ToDateTime(q["effective_to_date"]);
                if (!string.IsNullOrEmpty(Convert.ToString(q["cloned_from"])))
                    queue.ClonedFrom = Convert.ToString(q["cloned_from"]);
                if (!string.IsNullOrEmpty(Convert.ToString(q["delete_existing"])))
                    queue.DeleteExisting = Convert.ToBoolean(q["delete_existing"]);
                if (!string.IsNullOrEmpty(Convert.ToString(q["copy_from_time_series"])))
                    queue.CopyTimeSeries = Convert.ToBoolean(q["copy_from_time_series"]);

                queueData.Add(queue);
            });

            if (queueData != null && queueData.Count > 0)
            {
                typeIds = queueData.Select(q => q.TypeID).Distinct().ToList();
                typeIds.ForEach(id =>
                {
                    attrVsValue = null;
                    instancesPerTypeID = new List<int>();
                    instancesPerTypeID.AddRange(queueData.Where(q => q.TypeID == id).Select(qq => qq.RadInstanceId).Distinct().ToList());

                    if (instancesPerTypeID != null)
                        attrVsValue = SRMWorkflowController.GetAttributeVsValue(instancesPerTypeID, moduleID, dateFormat, dateTimeFormat);

                    if (attrVsValue != null)
                    {
                        var setAttrVsValue = from que in queueData.AsEnumerable()
                                             join avs in attrVsValue.AsEnumerable()
                                             on que.RadInstanceId equals avs.InstanceID
                                             select SetAttributeVsValue(que, avs.AttributeName, avs.AttributeValue, avs.isPrimary);

                        setAttrVsValue.Count();
                    }

                });


                dictPossibleActions = SRMWorkflowController.GetPossibleActions(queueData.Select(q => q.RadInstanceId).Distinct().ToList(), userName);

            }

            if (dictPossibleActions != null && dictPossibleActions.Keys.Count > 0)
            {
                queueData.ForEach(q =>
                {
                    if (dictPossibleActions.ContainsKey(q.RadInstanceId))
                        q.PossibleActions = dictPossibleActions[q.RadInstanceId];
                });
            }



            return queueData;
        }


        private static bool SetAttributeVsValue(WorkflowQueueData queue, string attrName, string attrValue, bool isPrimary)
        {
            queue.WorkflowAttributes.Add(new AttributeVsValue() { AttributeName = attrName, AttributeValue = attrValue, isPrimary = isPrimary });
            return true;
        }

        public static List<AttributeVsValue> RPFMGetAttributeValueFromDatatable(DataTable dt)
        {
            List<AttributeVsValue> attrVsValue = new List<AttributeVsValue>();
            AttributeVsValue avs = null;
            const string PRIMARY_ATTRIBUTE = "____inboxPrimaryAttribute";
            List<string> attributes = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Distinct().ToList();
            string primaryAttributeName = string.Empty;

            if (attributes.Contains("entity_code"))
                attributes.Remove("entity_code");
            if (attributes.Contains("instanceID"))
                attributes.Remove("instanceID");
            if (attributes.Contains(PRIMARY_ATTRIBUTE))
                attributes.Remove(PRIMARY_ATTRIBUTE);

            if (dt != null && dt.Rows.Count > 0)
            {
                primaryAttributeName = Convert.ToString(dt.Rows[0][PRIMARY_ATTRIBUTE]);
                dt.AsEnumerable().ToList().ForEach(row =>
                {
                    attributes.ForEach(attr =>
                    {
                        avs = new AttributeVsValue();
                        avs.InstanceID = Convert.ToInt32(row["instanceID"]);
                        avs.AttributeName = attr == "____primaryAttribute" ? primaryAttributeName : attr;
                        avs.AttributeValue = Convert.ToString(row[attr]);
                        avs.isPrimary = avs.AttributeName == primaryAttributeName;
                        attrVsValue.Add(avs);
                    });

                });
            }

            return attrVsValue;
        }


        public static List<AttributeAudit> BreakAuditLevels(DataTable dtAudit, Dictionary<int, DateTime> auditLevels, string dateColumn, int moduleID,string dateTimeFormat)
        {
            List<AttributeAudit> attrAudit = new List<AttributeAudit>();
            AttributeAudit audit = null;
            int currentLevel = 1;
            int nextLevel = 1;
            DateTime currentDate;
            DateTime nextDate;
            int levelCount = auditLevels.Keys.Count;

            for (int i = currentLevel; i < levelCount; i++)
            {
                nextLevel = currentLevel + 1;
                currentDate = auditLevels[currentLevel];
                nextDate = auditLevels[nextLevel];

                dtAudit.AsEnumerable().Where(t => Convert.ToDateTime(t[dateColumn]) >= currentDate && Convert.ToDateTime(t[dateColumn]) <= nextDate)
                    .ToList().ForEach(d =>
                    {
                        audit = new AttributeAudit();
                        audit.AuditLevel = currentLevel;
                        audit.AttributeName = Convert.ToString(d["Attribute Name"]);
                        audit.OldValue = Convert.ToString(d["Old Value"]);
                        audit.NewValue = Convert.ToString(d["New Value"]);
                        audit.TypeName = moduleID == 3 ? Convert.ToString(d["Leg Name"]) : Convert.ToString(d["Entity Type Name"]);
                        if (moduleID == 3)
                            audit.LegID = Convert.ToString(d["Leg ID"]);
                        if (moduleID != 3)
                            audit.PrimaryAttribute = Convert.ToString(d["Primary Attribute"]);
                        audit.KnowledgeDate = !string.IsNullOrEmpty(Convert.ToString(d[dateColumn])) ? Convert.ToDateTime(d[dateColumn]).ToString(dateTimeFormat) : Convert.ToString(d[dateColumn]);
                        audit.UserName = Convert.ToString(d["Modified By"]);

                        attrAudit.Add(audit);
                    });

                currentLevel++;
            }

            return attrAudit;
        }

        public static List<AttributeAudit> ParseActionHistoryAudit(DataTable dtAudit, string dateColumn, int moduleID, DateTime startDate, DateTime endDate)
        {
            List<AttributeAudit> attrAudit = new List<AttributeAudit>();
            AttributeAudit audit = null;
            dtAudit.AsEnumerable().Where(t => Convert.ToDateTime(t[dateColumn]) >= startDate && Convert.ToDateTime(t[dateColumn]) <= endDate)
                .ToList().ForEach(d =>
                {
                    audit = new AttributeAudit();
                    audit.AttributeName = Convert.ToString(d["Attribute Name"]);
                    audit.OldValue = Convert.ToString(d["Old Value"]);
                    audit.NewValue = Convert.ToString(d["New Value"]);
                    audit.TypeName = moduleID == 3 ? Convert.ToString(d["Leg Name"]) : Convert.ToString(d["Entity Type Name"]);
                    if (moduleID == 3)
                        audit.LegID = Convert.ToString(d["Leg ID"]);
                    if (moduleID != 3)
                        audit.PrimaryAttribute = Convert.ToString(d["Primary Attribute"]);
                    audit.KnowledgeDate = Convert.ToString(d[dateColumn]);
                    audit.UserName = Convert.ToString(d["Modified By"]);

                    attrAudit.Add(audit);
                });


            return attrAudit;
        }

        public static Dictionary<string, string> PopulateActionDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("approve", "Approved");
            dict.Add("edit and approve", "Approved");
            dict.Add("reject", "Rejected");
            dict.Add("pending", "Pending");
            dict.Add("started", "Initiated");
            dict.Add("cancel", "Canceled");
            return dict;
        }

        public static Dictionary<int, WorkflowType> SetWorkflowTypeDictionary(Dictionary<int, WorkflowType> workflowTypes)
        {
            workflowTypes = new Dictionary<int, WorkflowType>();
            workflowTypes.Add(0, WorkflowType.CREATE);
            workflowTypes.Add(1, WorkflowType.UPDATE);

           // workflowTypes.Add(2, WorkflowType.ATTRIBUTE);
           // workflowTypes.Add(3, WorkflowType.LEG);
           // workflowTypes.Add(4, WorkflowType.DELETE);
            return workflowTypes;
        }

        public static List<WorkflowDivisionByModule> PopulateModuleWiseInfo(List<WorkflowDivisionByModule> moduleWiseInfo)
        {
            moduleWiseInfo = new List<WorkflowDivisionByModule>();
            WorkflowDivisionByModule obj = null;

            //Start Filling Instrument Columns
            obj = new WorkflowDivisionByModule();
            obj.ModuleID = 3;
            obj.InstrumentColumnName = "Security ID";
            moduleWiseInfo.Add(obj);

            obj = new WorkflowDivisionByModule();
            obj.ModuleID = 6;
            obj.InstrumentColumnName = "Entity Code";
            moduleWiseInfo.Add(obj);

            obj = new WorkflowDivisionByModule();
            obj.ModuleID = 18;
            obj.InstrumentColumnName = "Entity Code";
            moduleWiseInfo.Add(obj);

            obj = new WorkflowDivisionByModule();
            obj.ModuleID = 20;
            obj.InstrumentColumnName = "Entity Code";
            moduleWiseInfo.Add(obj);
            //End Filling Instrument Columns


            return moduleWiseInfo;
        }
    }
}
