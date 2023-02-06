using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class BankDealUpdate
    {
        [CustomAttributes(HeaderName = "globtranid", HasInnerProperty = false)]
        public string GlobTranId { get; set; }
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

        [CustomAttributes(HeaderName = "BankDealUpdateData", HasInnerProperty = true)]
        public BankDealUpdateData BankDealUpdateData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public List<Facility> Facility { get; set; }
        [CustomAttributes(HeaderName = "BankDeal", HasInnerProperty = true)]
        public BankDeal BankDeal { get; set; }
        [CustomAttributes(HeaderName = "BankDealExisting", HasInnerProperty = true)]
        public BankDeal BankDealExisting { get; set; }
    }

    class BankDealUpdateData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
    }

    class BankDeal
    {
        [CustomAttributes(HeaderName = "globbankdealid", HasInnerProperty = false)]
        public int GlobalbankDealId { get; set; }
        [CustomAttributes(HeaderName = "name", HasInnerProperty = false)]
        public string bankDealName { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "creditdt", HasInnerProperty = false)]
        public DateTime CreditDate { get; set; }
        [CustomAttributes(HeaderName = "cusip", HasInnerProperty = false)]
        public string Cusip { get; set; }
    }
}
