﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.rad.RMarketWSO.Info.Transactions
{
    class ReceiveInterimInterest
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

        [CustomAttributes(HeaderName = "ReceiveInterimInterestData", HasInnerProperty = true)]
        public ReceiveInterimInterestData ReceiveInterimInterestData { get; set; }
        [CustomAttributes(HeaderName = "Issuer", HasInnerProperty = true)]
        public Issuer Issuer { get; set; }
        [CustomAttributes(HeaderName = "Facility", HasInnerProperty = true)]
        public Facility Facility { get; set; }
        [CustomAttributes(HeaderName = "ContractExisting", HasInnerProperty = true)]
        public ContractExisting ContractExisting { get; set; }
        [CustomAttributes(HeaderName = "Capitalization", HasInnerProperty = true)]
        public Capitalization Capitalization { get; set; }
    }

    class ReceiveInterimInterestData
    {
        [CustomAttributes(HeaderName = "desc", HasInnerProperty = false)]
        public string Description { get; set; }
        [CustomAttributes(HeaderName = "nextpaydt", HasInnerProperty = false)]
        public DateTime NextPaymentDate { get; set; }
    }
}