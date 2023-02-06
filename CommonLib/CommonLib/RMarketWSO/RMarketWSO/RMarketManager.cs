using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using System.Net;

namespace com.ivp.rad.RMarketWSO
{
    public class RMarketManager
    {
        static RMarketManager()
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
        }

        static IRLogger mLogger = RLogFactory.CreateLogger("RMarketManager");

        public Dictionary<int, bool> SubscribeFacility(List<int> facilityId, string user, int VendorPreferenceId = 1)
        {
            mLogger.Debug("being SubscribeFacility");
            RMarketController cntrl = new RMarketController() { VendorPreferenceId = VendorPreferenceId };
            Dictionary<int, bool> result = cntrl.SubscribeFacility(facilityId, user);
            mLogger.Debug("end SubscribeFacility");
            return result;
        }

        public Dictionary<int, bool> UnSubscribeFacility(List<int> facilityId, string user, int VendorPreferenceId = 1)
        {
            mLogger.Debug("being UnSubscribeFacility");
            RMarketController cntrl = new RMarketController() { VendorPreferenceId = VendorPreferenceId };
            Dictionary<int, bool> result = cntrl.UnSubscribeFacility(facilityId, user);
            mLogger.Debug("end UnSubscribeFacility");
            return result;
        }

        public List<int> GetAllSubscribedFacilities(int VendorPreferenceId = 1)
        {
            mLogger.Debug("MarkitWSO -> Entering GetAllSubscribedFacilities method ");
            List<int> subscribedFacilities = new RMarketController() { VendorPreferenceId = VendorPreferenceId }.GetAllSubscribedFacilities();
            mLogger.Debug("MarkitWSO -> Leaving GetAllSubscribedFacilities method ");
            return subscribedFacilities;
        }

        public RMarketResponseInfo FetchFacilityData(RMarketRequestInfo requestInfo)
        {
            if (requestInfo.VendorPreferenceId == 0)
                requestInfo.VendorPreferenceId = 1;
            mLogger.Debug("begin FetchFacilityData");
            RMarketController cntrl = new RMarketController() { VendorPreferenceId = requestInfo.VendorPreferenceId };
            RMarketResponseInfo response = cntrl.FetchFacilityData(requestInfo);
            mLogger.Debug("end FetchFacilityData");
            return response;
        }

        public RMarketResponseInfo FetchFacilityData(string FileName, int VendorPreferenceId = 1)
        {
            mLogger.Debug("begin FetchFacilityData");
            RMarketController cntrl = new RMarketController() { VendorPreferenceId = VendorPreferenceId };
            RMarketResponseInfo response = cntrl.FetchFacilityData(FileName);
            mLogger.Debug("end FetchFacilityData");
            return response;
        }

        public void FetchAllFacilityDetails(int VendorPreferenceId = 1)
        {
            mLogger.Debug("start FetchAllFacilityDetails");
            RMarketController cntrl = new RMarketController() { VendorPreferenceId = VendorPreferenceId };
            cntrl.FetchAllFacilityDetails();
            mLogger.Debug("end  FetchAllFacilityDetails");
        }

        public Dictionary<string, RMarketIdentifierType> GetAllAvailableFacilities()
        {
            return new RMarketController().GetAllAvailableFacilities();
        }
    }
}
