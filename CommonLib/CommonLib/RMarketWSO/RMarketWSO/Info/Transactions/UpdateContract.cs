using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class UpdateContract
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

        [CustomAttributes(HeaderName = "UpdateContractData", HasInnerProperty = true)]
        public UpdateContractData UpdateContractData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
        [CustomAttributes(HeaderName = "ContractExisting", HasInnerProperty = true)]
        public ContractExisting ContractExisting { get; set; }
        [CustomAttributes(HeaderName = "ContractUpdating", HasInnerProperty = true)]
        public ContractExisting ContractUpdating { get; set; }
    }

    class UpdateContractData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
    }
}
