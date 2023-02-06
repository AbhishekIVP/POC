using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.ivp.srm.vendorapi.Bloomberg.Fundamentals;

namespace com.ivp.srm.vendorapi
{
    public interface IVendorForFundamentals
    {
        void GetFundamentals(RBbgFundamentalInfo info);
        DataSet GetResponse(string RequestId, string transportName, int VendorPreferenceId);
    }
}
