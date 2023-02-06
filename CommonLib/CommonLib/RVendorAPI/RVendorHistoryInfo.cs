using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;
using System.Threading;

namespace com.ivp.srm.vendorapi
{
    internal class RVendorHistoryInfo
    {
        public int VendorHistoryId { get; set; }
        public string VendorInstruments { get; set; }
        public string VendorFields { get; set; }
        public string UserLoginName { get; set; }
        public DateTime RequestedOn { get; set; }
        public string requestIdentifier { get; set; }
        public int ProcessedInstrumentCount { get; set; }
        public int ProcessedFieldCount { get; set; }
        public string RequestStatus { get; set; }
        public string TimeStamp { get; set; }
        public string RequestType { get; set; }
        public string VendorName { get; set; }
        public string ExceptionMessage { get; set; }
        public bool IsBulkList { get; set; }
        public int ModuleId{ get; set; }
        internal string TransportName { get; set; }
        internal RBloomberg CurrentObject { get; set; }
        internal ManualResetEvent ManualReset{ get; set; }
        internal bool IsMarketSectorAppended { get; set; }
        public int VendorPricingId { get; set; }
        //internal string MarketSector { get; set; }
        public int VendorPreferenceId { get; set; }

        public string ClientName { get; set; }

    }
}
