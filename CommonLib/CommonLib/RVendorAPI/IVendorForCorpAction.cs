using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;

namespace com.ivp.srm.vendorapi
{
    public interface IVendorForCorpAction
    {
        void GetCorpAction(RCorpActInfo corpActInfo);
        void GetCorpActionResponse(RCorpActInfo corpActInfo);
    }
}
