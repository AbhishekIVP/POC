using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;

namespace com.ivp.srm.vendorapi.bloomberg
{
    public class RCorpActInfo
    {
        public RCorpActInfo()
        {
            SecurityIds = new List<RCorpActSecurityInfo>();
            corpActions = new List<CorpActions>();
            CorpActResultantXml = new XDocument();
            GetFutureAction = true;
            ProcessedInstruments = new Dictionary<string, List<string>>();
            CorpActResultantDataset = new DataSet();
            StartDate = DateTime.Now - new TimeSpan(7, 0, 0, 0);
        }

        public List<RCorpActSecurityInfo> SecurityIds { get; set; }
        public List<CorpActions> corpActions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool GetFutureAction { get; set; }
        public XDocument CorpActResultantXml { get; set; }
        public string TransportName { get; set; }
        public string UserLogin { get; set; }
        public int ModuleID { get; set; }
        public Dictionary<string, List<string>> ProcessedInstruments { get; set; }
        public string RequestIdentifier { get; set; }
        public bool ImmediateResponse { get; set; }
        public DataSet CorpActResultantDataset { get; set; }
        internal bool HasError { get; set; }
        public RBbgCorpActRequestType RequestType { get; set; }
        public RBbgInstrumentIdType InstrumentType { get; set; }
        public int VendorPreferenceId { get; set; }
    }
    public class RCorpActSecurityInfo
    {
        public string SecurityName { get; set; }
        public CorpActMarketSector MarcketSector { get; set; }
    }

    public enum RBbgCorpActRequestType
    {
        FTP,
        Heavy
    }

    public enum CorpActions
    {
        CHG_NAME,
        CHG_DOM,
        CHG_STATE,
        CHG_RLOT,
        CHG_TKR,
        CHG_ID,
        DELIST,
        CHG_LIST,
        LIST,
        VAR_INT_RST,
        CHG_VOTE,
        CRNCY_QT_CHNG,
        RECONVENTION,
        REDENOMINATION,
        SH_HOLDER_MEET,
        MERG,
        SPIN,
        BANCR,
        STOCK_BUY,
        EQY_OFFER,
        DBT_RDMP_CALL,
        DBT_OFFER_INC,
        DBT_OFFER_NEW,
        CHG_PAR,
        DBT_RDMP_SINK,
        ACQUIS,
        DIVEST,
        PARTIAL_PAY,
        RECLASS,
        DBT_REP,
        DBT_RDMP_PUT,
        EXCH_OFFER,
        VAR_PR_RDMP,
        PAY_IN_KIND,
        RIGHTS_OFFER,
        CONV_PX_RFIX,
        EX_PX_RFIX,
        FUNG_ISS,
        EXTEN_ISS,
        DVD_CASH,
        DVD_STOCK,
        STOCK_SPLT,
    }

    public enum CorpActMarketSector
    {
        None,
        /// <remarks/>
        Govt,

        /// <remarks/>
        Corp,

        /// <remarks/>
        Mtge,

        /// <remarks/>
        MMkt,

        /// <remarks/>
        Muni,

        /// <remarks/>
        Pfd,

        /// <remarks/>
        Equity,

        /// <remarks/>
        Comdty,

        /// <remarks/>
        Index,

        /// <remarks/>
        Curncy,
    }

    public enum CorpActReturnType
    {

    }
}
