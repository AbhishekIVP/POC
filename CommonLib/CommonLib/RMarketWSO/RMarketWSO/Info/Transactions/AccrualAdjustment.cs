using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class AccrualAdjustment
    {
        [CustomAttributes(HeaderName = "globtranid", HasInnerProperty = false)]
        public string GLOBALTRANID { get; set; }
        [CustomAttributes(HeaderName = "messagetype", HasInnerProperty = false)]
        public string MessageType { get; set; }
        [CustomAttributes(HeaderName = "effdt", HasInnerProperty = false)]
        public DateTime EffectiveDate { get; set; }
        [CustomAttributes(HeaderName = "notes", HasInnerProperty = false)]
        public string Notes { get; set; }
        [CustomAttributes(HeaderName = "longdescription", HasInnerProperty = false)]
        public string LongDescription { get; set; }
        [CustomAttributes(HeaderName = "shortdescription", HasInnerProperty = false)]
        public string ShortDescription { get; set; }
        [CustomAttributes(HeaderName = "trangroupid", HasInnerProperty = false)]
        public int TransactionGroupID { get; set; }

        [CustomAttributes(HeaderName = "AccrualAdjustmentData", HasInnerProperty = true)]
        public AccrualAdjustmentData AccrualAdjustmentData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
    }

    class AccrualAdjustmentData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double AccrualGlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "startdt", HasInnerProperty = false)]
        public DateTime AccrualStartDate { get; set; }
        [CustomAttributes(HeaderName = "enddt", HasInnerProperty = false)]
        public DateTime AccrualEndDate { get; set; }

        [CustomAttributes(HeaderName = "allinrate", HasInnerProperty = false)]
        public double AllInRate { get; set; }
        [CustomAttributes(HeaderName = "accrualdesc", HasInnerProperty = false)]
        public string AccrualDescription { get; set; }
        [CustomAttributes(HeaderName = "receivedt", HasInnerProperty = false)]
        public DateTime AccrualReceiveDate { get; set; }
        [CustomAttributes(HeaderName = "daycount", HasInnerProperty = false)]
        public string DateCount { get; set; }
    }
}
