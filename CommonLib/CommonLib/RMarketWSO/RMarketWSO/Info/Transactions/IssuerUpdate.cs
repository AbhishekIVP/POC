using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class IssuerUpdate
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

        [CustomAttributes(HeaderName = "IssuerUpdateData", HasInnerProperty = true)]
        public IssuerUpdateData IssuerUpdateData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public IssuerUpdateIssuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "IssuerExisting", HasInnerProperty = true)]
        public IssuerUpdateIssuer IssuerExisting { get; set; }
    }
    class IssuerUpdateData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
    }

    class IssuerUpdateIssuer
    {
        [CustomAttributes(HeaderName = "name", HasInnerProperty = false)]
        public string Name { get; set; }
        [CustomAttributes(HeaderName = "globissuerid", HasInnerProperty = false)]
        public string GlobalIssuerId { get; set; }
        [CustomAttributes(HeaderName = "abbrev", HasInnerProperty = false)]
        public string IssuerAbbreviation { get; set; }
    }


}
