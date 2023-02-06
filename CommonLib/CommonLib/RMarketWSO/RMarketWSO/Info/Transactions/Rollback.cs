using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class Rollback
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

        [CustomAttributes(HeaderName = "RollbackData", HasInnerProperty = true)]
        public RollbackData RollbackData { get; set; }
    }

    class RollbackData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
        [CustomAttributes(HeaderName = "rollbackglobtranid", HasInnerProperty = false)]
        public int RollbackGlobalTransactionID { get; set; }
        [CustomAttributes(HeaderName = "rollbacktrantypeid", HasInnerProperty = false)]
        public int RollbackTransactionTypeID { get; set; }
        [CustomAttributes(HeaderName = "rollbacktrangroupid", HasInnerProperty = false)]
        public int RollbackTransactionGroupID { get; set; }
        [CustomAttributes(HeaderName = "rollbackglobfacilityid", HasInnerProperty = false)]
        public int RollbackGlobalFacilityID { get; set; }
        [CustomAttributes(HeaderName = "existingglobcontractids", HasInnerProperty = false)]
        public string CommaDelimitedListOfExistingGlobalContractID { get; set; }
      
    }
}
