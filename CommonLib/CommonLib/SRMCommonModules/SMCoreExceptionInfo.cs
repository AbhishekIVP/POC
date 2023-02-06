using com.ivp.SRMCommonModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace com.ivp.secm.exceptionsmanager
{
    [Serializable]
    public class SMCoreExceptionInfo
    {
        public SMExceptionsType ExceptionType { get; set; }
        public int TaskMasterId { get; set; }
        public int SecTypeId { get; set; }
        public int AttributeId { get; set; }
        public int SecTypeTableId { get; set; }
        public string SecurityId { get; set; }
        public int PrimaryVendorId { get; set; }
        public int FeedId { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceValue { get; set; }
        public int EntityTypeId { get; set; }
        public string UserName { get; set; }
        public string Remarks { get; set; }
        public string DBValue { get; set; }
        public List<string> GroupName { get; set; }
        public List<int> UserId { get; set; }
        public List<SMDependentInfo> VendorValues { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdditionalLeg { get; set; }

        public SMCoreExceptionInfo()
        {
            GroupName = new List<string>();
            UserId = new List<int>();
            VendorValues = new List<SMDependentInfo>();
            IsActive = true;
            AttributeId = -1;
            SecTypeTableId = -1;
            DBValue = string.Empty;
        }
        public override int GetHashCode() { return 0; }

        public bool Equals(SMCoreExceptionInfo obj)
        {
            if (this.AttributeId == obj.AttributeId &&
                this.SecTypeId == obj.SecTypeId &&
                this.SecurityId == obj.SecurityId &&
                this.ExceptionType == obj.ExceptionType &&
                this.VendorValues.Count == obj.VendorValues.Count &&
                this.TaskMasterId == obj.TaskMasterId &&
                this.VendorValues.All(x => obj.VendorValues.Contains(x)))
            {
                if (this.ExceptionType == SMExceptionsType.VALUE_CHANGED)
                {
                    return Convert.ToString(this.DBValue).Equals(Convert.ToString(obj.DBValue), StringComparison.OrdinalIgnoreCase);
                }
                else
                    return true;
            }

            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj is SMCoreExceptionInfo && Equals((SMCoreExceptionInfo)obj))
                return true;

            return false;
        }
    }

    public enum SMExceptionsType
    {
        SECURITY_NOT_FOUND = -1,
        COMPARE_AND_SHOW = 1,
        REFERENCE_DATA = 2,
        NO_VENDOR_VALUE_FOUND = 3,
        VALUE_CHANGED = 4,
        SHOW_AS_EXCEPTION = 5,
        FIRST_VENDOR_VALUE_MISSING = 6,
        CUSTOM_EXCEPTION = 7,
        DATATYPE_MISMATCH = 8,
        UNDERLYING_DATA = 9,
        ALERTS = 10,
        BASKET_CUSTOM_EXCEPTION = 11,
        BASKET_ALERTS = 12,
        CONDITIONAL_FILTER_EXCEPTION = 13,
        UNIQUENESS = 14
    }

    public enum SMExceptionDependentType
    {
        VENDOR = 1,
        ATTRIBUTE = 2
    }
    [Serializable]
    public struct SMDependentInfo
    {
        int dependentTypeId;
        int dependenteId;
        string dependentValue;

        public int DependentTypeId
        {
            get { return dependentTypeId; }
            set { dependentTypeId = value; }
        }
        public int DependenteId
        {
            get { return dependenteId; }
            set { dependenteId = value; }
        }
        public string DependentValue
        {
            get { return dependentValue; }
            set { dependentValue = value; }
        }
        public SMDependentInfo(int dependentTypeId, int dependenteId, string dependentValue)
        {
            this.dependentTypeId = dependentTypeId;
            this.dependenteId = dependenteId;
            if (string.IsNullOrEmpty(dependentValue) || dependentValue.Equals(DBNull.Value))
                this.dependentValue = null;
            else
                this.dependentValue = dependentValue;
        }

        public override int GetHashCode() { return 0; }

        public override bool Equals(object obj)
        {
            string objDepedentValue = ((SMDependentInfo)obj).dependentValue;
            string thisDepedentValue = this.dependentValue;

            if (string.IsNullOrWhiteSpace(this.dependentValue) || string.IsNullOrEmpty(this.dependentValue) || this.dependentValue.Equals(DBNull.Value))
            { 
                this.dependentValue = null;
                thisDepedentValue = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(((SMDependentInfo)obj).dependentValue) || string.IsNullOrEmpty(((SMDependentInfo)obj).dependentValue) || ((SMDependentInfo)obj).dependentValue.Equals(DBNull.Value))
            { 
                objDepedentValue = string.Empty;
            }

            if (string.Equals(thisDepedentValue.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim(), objDepedentValue.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                    this.DependentTypeId == ((SMDependentInfo)obj).DependentTypeId &&
                    this.DependenteId == ((SMDependentInfo)obj).DependenteId)
                return true;

            //if (!string.IsNullOrEmpty(thisDepedentValue) && !string.IsNullOrEmpty(objDepedentValue))
            //{
                
            //}
            //else
            //{
            //    if (this.dependentValue == ((SMDependentInfo)obj).DependentValue &&
            //      this.DependentTypeId == ((SMDependentInfo)obj).DependentTypeId &&
            //      this.DependenteId == ((SMDependentInfo)obj).DependenteId)
            //        return true;
            //}

            return false;
        }


    }

    public enum SMExceptionTaskType
    {
        Manual,
        FlowTask,
        ValidationTask,
        RuleRunnerTask
    }

    public class SMManageExceptionAcrossInputInfo
    {
        public int secTypeId;
        public string userName;
        public Dictionary<string, SMManageExceptionAcrossInfo> lstExceptionInfo;
        public SMExceptionTaskType taskType;
        public Dictionary<SRMEventActionType, SRMEventPreferenceInfo> dictEventConfiguration;
        public DateTime? dtModifiedOn;
    }

    /// <summary>
    /// Info for inserting and deleting exceptions across modules. Exceptions to be inserted or deleted is to be collected per security id
    /// </summary>
    public class SMManageExceptionAcrossInfo
    {
        public string SecId;

        /// <summary>
        /// List of master and single info attribute ids for which value has changed and if exception preference is set true then exceptions for these attributes will be deleted
        /// </summary>
        public HashSet<int> LstImpactedAttributes = new HashSet<int>();

        /// <summary>
        /// List of multi info/common leg names for which data has changed and if exception preference is set true then exceptions for these legs will be deleted
        /// </summary>
        public HashSet<string> LstImpactedLegs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// List of new exceptions raised which will be compared with suppressed exceptions and then inserted in exception tables
        /// </summary>
        public List<SMCoreExceptionInfo> LstCoreExceptions = new List<SMCoreExceptionInfo>();
    }

    public class SMExceptionEventsAcross
    {
        public Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>> DeletedExceptionsEvents = new Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>> InsertedExceptionsEvents = new Dictionary<string, Dictionary<SMExceptionsType, SRMEventInfo>>(StringComparer.OrdinalIgnoreCase);
        public int ExceptionCount;
    }
}
