using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace com.ivp.srm.vendorapi.bloomberg
{
    public class RBbgHistoryInfo
    {
        public RBbgHistoryInfo()
        {
            SecurityIds = new List<RHistorySecurityInfo>();
            Fields = new List<string>();
            ResultantData = new DataSet();
            ProcessedInstruments = new List<string>();
            ProcessedFields = new List<string>();
            IPAddresses = new List<string>();
            RequestId = string.Empty;
        }
        public List<RHistorySecurityInfo> SecurityIds { get; set; }
        public List<string> Fields { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TransportName { get; set; }
        public string UserLogin { get; set; }
        public int ModuleID { get; set; }
        public DataSet ResultantData { get; set; }
        public List<string> ProcessedInstruments { get; set; }
        public List<string> ProcessedFields { get; set; }
        public RHistoryRequestType RequestType { get; set; }
        public string SAPIUserName { get; set; }
        public List<string> IPAddresses { get; set; }
        public PeriodicityAdjustment PeriodicAdjustment { get; set; }
        public PeriodicitySelection PeriodicSelection { get; set; }
        public bool IsImmediate { get; set; }
        public string RequestId { get; set; }
        public int VendorPreferenceId { get; set; }
    }

    public class RHistorySecurityInfo
    {
        public string SecurityName { get; set; }
        public RBbgHistoryInstrumentIdType securityType { get; set; }
        public CorpActMarketSector MarcketSector { get; set; }
        internal bool dateProvided;
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value; dateProvided = true;
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value; dateProvided = true;
            }
        }
    }
    public enum RBbgHistoryInstrumentIdType
    {
        /// <remarks/>
        NONE,

        /// <remarks/>
        AUSTRIAN,

        /// <remarks/>
        BB_UNIQUE,

        BB_GLOBAL,

        /// <remarks/>
        BELGIAN,

        /// <remarks/>
        BLOOMBERG,

        /// <remarks/>
        CATS,

        /// <remarks/>
        CEDEL,

        /// <remarks/>
        CINS,

        /// <remarks/>
        COMMON_NUMBER,

        /// <remarks/>
        COMMON_HEADER,

        /// <remarks/>
        CUSIP,

        /// <remarks/>
        CZECH,

        /// <remarks/>
        DUTCH,

        /// <remarks/>
        EUROCLEAR,

        /// <remarks/>
        FRENCH,

        /// <remarks/>
        IRISH,

        /// <remarks/>
        ISIN,

        /// <remarks/>
        ISRAELI,

        /// <remarks/>
        ITALY,

        /// <remarks/>
        JAPAN,

        /// <remarks/>
        LUXEMBOURG,

        /// <remarks/>
        SEDOL,

        /// <remarks/>
        SEDOL1,

        /// <remarks/>
        SEDOL2,

        /// <remarks/>
        SPAIN,

        /// <remarks/>
        TICKER,

        /// <remarks/>
        TSCUSIP,

        /// <remarks/>
        VALOREN,

        /// <remarks/>
        WPK,
    }
    public enum RHistoryRequestType
    {
        FTP,
        SAPI
    }
    public enum PeriodicityAdjustment
    {
        ACTUAL,
        CALENDER,
        FISCAL
    }

    public enum PeriodicitySelection
    {
        DAILY,
        WEEKLY,
        MONTHLY,
        QUATERLY,
        SEMI_ANNUALLY,
        YEARLY
    }
}
