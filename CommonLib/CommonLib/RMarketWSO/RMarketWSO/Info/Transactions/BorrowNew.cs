using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class BorrowNew
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

        [CustomAttributes(HeaderName = "BorrowNewData", HasInnerProperty = true)]
        public BorrowNewData BorrowNewData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
        [CustomAttributes(HeaderName = "ContractExisting", HasInnerProperty = true)]
        public List<ContractExisting> ContractExisting { get; set; }
        [CustomAttributes(HeaderName = "ContractNew", HasInnerProperty = true)]
        public ContractNew ContractNew { get; set; }
    }

    class BorrowNewData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
        [CustomAttributes(HeaderName = "globamt", HasInnerProperty = false)]
        public double GlobalAmount { get; set; }
        [CustomAttributes(HeaderName = "name", HasInnerProperty = false)]
        public string GlobalContractName { get; set; }
    }
}
