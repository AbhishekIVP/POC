using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;
using System.Data;

namespace com.ivp.srm.vendorapi
{
    public interface IVendorForGetHistory
    {
        void GetHistory(RBbgHistoryInfo historyInfo);
        DataSet GetResponse(string RequestId, int VendorPreferenceId);
        DataSet GetResponse(string RequestId);
    }
}
