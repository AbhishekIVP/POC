using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class SRMVendorManagementDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorManagementDBManager");
        private RDBConnectionManager mDBCon = null;

        public SRMVendorManagementDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }
        public SRMVendorManagementDBManager() { }
        public ObjectSet DownloadVendorSystemConfiguration(List<int> vendorManagementIds = null,bool isVisible = true)
        {
            try
            {
                mLogger.Debug("SRMVendorManagementDBManager -> DownloadVendorSystemConfiguration -> Start");

                string vendor_management_ids = string.Empty;
                ObjectSet opSet = null;

                if (vendorManagementIds != null && vendorManagementIds.Count > 0)
                {
                    vendor_management_ids = string.Join(",", vendorManagementIds);
                }

                string queryText = "EXEC [dbo].[RAD_DownloadVendorSystemSettings] '" + vendor_management_ids + "','" + isVisible + "'";


                if (this.mDBCon != null)
                    opSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    opSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);
                
                return opSet;
            }
            catch
            {
                mLogger.Debug("SRMVendorManagementDBManager -> DownloadVendorSystemConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("SRMVendorManagementDBManager -> DownloadVendorSystemConfiguration -> End");
            }
        }

        public ObjectSet GetPossibleKeys()
        {
            try
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetPossibleKeys -> Start");

                ObjectSet possKeys = null;

                string queryText = "select distinct vendor_name,request_type_name,key_name from IVPRefMasterVendor.dbo.ivp_rad_vendor_management_vendor_type vtype inner join IVPRefMasterVendor.dbo.ivp_rad_vendor_management_details vdet on vdet.vendor_id = vtype.vendor_id inner join IVPRefMasterVendor.dbo.ivp_rad_vendor_management_request_type rtype on rtype.request_type_id = vdet.request_type_id order by vendor_name, request_type_name";

                possKeys = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);

                return possKeys;
            }
            catch
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetPossibleKeys -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetPossibleKeys -> End");
            }
        }

        public ObjectSet GetHeaderTypeValues()
        {
            try
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetHeaderTypeValues -> Start");

                ObjectSet possKeys = null;

                string queryText = "select distinct vendor_name,request_type_name,header_type_name from IVPRefMasterVendor.dbo.ivp_rad_vendor_management_vendor_type vtype inner join IVPRefMasterVendor.dbo.ivp_rad_vendor_management_header_type vhtyp on vtype.vendor_id = vhtyp.vendor_id inner join IVPRefMasterVendor.dbo.ivp_rad_vendor_management_request_type vreq on vreq.request_type_id = vhtyp.request_type_id";

                possKeys = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);

                return possKeys;
            }
            catch
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetHeaderTypeValues -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("SRMVendorManagementDBManager -> GetHeaderTypeValues -> End");
            }
        }
    }
}
