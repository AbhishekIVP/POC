using com.ivp.rad.baselicensemanager;
using com.ivp.rad.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.SRMCommonModules
{
    public static class SRMCommonLicenseHandler
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMCommonLicenseHandler");

        private static DateTime _lastFetchDate;

        private static RBaseLicenseManager _objRBaseLicenseManager;

        public static RBaseLicenseManager objRBaseLicenseManager
        {
            get
            {
                if (_objRBaseLicenseManager == null || _lastFetchDate.Date != DateTime.Today)
                {
                    mLogger.Error("Before => new RBaseLicenseManager()");

                    _objRBaseLicenseManager = new RBaseLicenseManager();

                    _objRBaseLicenseManager.LoadFactory();

                    mLogger.Error("After => new RBaseLicenseManager()");
                }

                _lastFetchDate = DateTime.Today;

                return _objRBaseLicenseManager;
            }
        }
    }
}
