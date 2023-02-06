using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using com.ivp.rad.dal;
using com.ivp.common;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System.Reflection;
using com.ivp.commom.commondal;
using com.ivp.rad.common;

namespace com.ivp.common
{
    public class SMCommonController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SMCommonController");

        private void SetSecTypeAndAttributeDisplayNames(List<SRMSecurityAttributeDetailsInfo> lstInput, SRMSecTypeMassageInputInfo inputInfo, IEnumerable<ObjectRow> dtInput, DataTable table, RDBConnectionManager mDBCon = null)
        {
            if (table != null && table.Rows.Count > 0)
            {
                var assign = from lst in lstInput.AsEnumerable().Where(l => !string.IsNullOrEmpty(l.SecurityAttributeId))
                             join tab in table.AsEnumerable()
                             on lst.SecurityTypeId equals Convert.ToInt32(tab["sectype_id"])
                             where
                             (
                                Convert.ToString(lst.SecurityAttributeId).Trim().ToLower() == Convert.ToString(tab["attribute_id"]).Trim().ToLower()
                             )
                             select AssignSecTypeAttributeName(lst, Convert.ToString(tab["sectype_name"]), lst.SecurityAttributeId, Convert.ToString(tab["attribute_name"]), Convert.ToString(tab["attribute_display_name"]));

                assign.Count();

                assign = from lst in lstInput.AsEnumerable().Where(l => !string.IsNullOrEmpty(l.SecurityAttributeRealName))
                         join tab in table.AsEnumerable()
                         on lst.SecurityTypeId equals Convert.ToInt32(tab["sectype_id"])
                         where
                         (
                            lst.SecurityAttributeRealName.Trim().ToLower() == Convert.ToString(tab["attribute_name"]).Trim().ToLower()
                         )
                         select AssignSecTypeAttributeName(lst, Convert.ToString(tab["sectype_name"]), Convert.ToString(tab["attribute_id"]), lst.SecurityAttributeRealName, Convert.ToString(tab["attribute_display_name"]));

                assign.Count();


                if (!string.IsNullOrEmpty(inputInfo.SecTypeAttributeRealNameColumn))
                {
                    assign = from lst in lstInput
                             join row in dtInput.Where(r => !string.IsNullOrEmpty(Convert.ToString(r[inputInfo.SecTypeIDColumn])))
                             on lst.SecurityTypeId equals Convert.ToInt32(row[inputInfo.SecTypeIDColumn])
                             where lst.SecurityAttributeRealName.Trim().ToLower() == Convert.ToString(row[inputInfo.SecTypeAttributeRealNameColumn]).Trim().ToLower()
                             select SetSecurityTypeDetails(row, inputInfo.SecTypeDisplayNameColumn, inputInfo.SecTypeAttributeDisplayNameColumn, lst.SecurityTypeName, lst.SecurityAttributeDisplayName);

                    assign.Count();
                }
                else
                {
                    assign = from lst in lstInput
                             join row in dtInput.Where(r => !string.IsNullOrEmpty(Convert.ToString(r[inputInfo.SecTypeIDColumn])))
                             on lst.SecurityTypeId equals Convert.ToInt32(row[inputInfo.SecTypeIDColumn])
                             where lst.SecurityAttributeId.Trim().ToLower() == Convert.ToString(row[inputInfo.SecTypeAttributeIDColumn]).Trim().ToLower()
                             select SetSecurityTypeDetails(row, inputInfo.SecTypeDisplayNameColumn, inputInfo.SecTypeAttributeDisplayNameColumn, lst.SecurityTypeName, lst.SecurityAttributeDisplayName);

                    assign.Count();

                }
            }

        }

        private bool SetSecurityTypeDetails(ObjectRow row, string secTypeColumnName, string secAttributeColumnName, string secTypeName, string secAttributeName)
        {
            row[secTypeColumnName] = secTypeName;
            row[secAttributeColumnName] = secAttributeName;
            return true;
        }

        public void MassageSecTypeAndAttributes(IEnumerable<ObjectRow> dtInput, SRMSecTypeMassageInputInfo inputInfo, RDBConnectionManager mDBCon = null)
        {
            try
            {
                DataTable table = null;
                if (dtInput != null && dtInput.Count() > 0)
                {
                    List<SRMSecurityAttributeDetailsInfo> secDetailsInfo = new List<SRMSecurityAttributeDetailsInfo>();
                    SRMSecurityAttributeDetailsInfo secInfo = null;
                    string attributeColumn = !string.IsNullOrEmpty(inputInfo.SecTypeAttributeIDColumn) ? inputInfo.SecTypeAttributeIDColumn : inputInfo.SecTypeAttributeRealNameColumn;
                    bool hasAttributeID = !string.IsNullOrEmpty(inputInfo.SecTypeAttributeIDColumn);

                    dtInput.Where(d => !string.IsNullOrEmpty(Convert.ToString(d[inputInfo.SecTypeIDColumn]))).ToList().ForEach(row =>
                    {
                        secInfo = new SRMSecurityAttributeDetailsInfo();
                        secInfo.SecurityTypeId = Convert.ToInt32(row[inputInfo.SecTypeIDColumn]);
                        if (hasAttributeID)
                            secInfo.SecurityAttributeId = Convert.ToString(row[inputInfo.SecTypeAttributeIDColumn]);
                        else
                            secInfo.SecurityAttributeRealName = Convert.ToString(row[inputInfo.SecTypeAttributeRealNameColumn]);

                        secDetailsInfo.Add(secInfo);
                    });


                    table = new SMDBManager().GetSecTypeAndAttributeDetails(secDetailsInfo, mDBCon);
                    SetSecTypeAndAttributeDisplayNames(secDetailsInfo, inputInfo, dtInput, table, mDBCon);
                }

            }
            catch
            {

                throw;
            }
        }


        private bool AssignSecTypeAttributeName(SRMSecurityAttributeDetailsInfo info, string secTypeName, string secAttributeID, string secAttributeRealName, string secAttributeDisplayName)
        {
            info.SecurityAttributeId = secAttributeID;
            info.SecurityTypeName = secTypeName;
            info.SecurityAttributeRealName = secAttributeRealName;
            info.SecurityAttributeDisplayName = secAttributeDisplayName;
            return true;
        }

        public Dictionary<string, SecurityTypeMasterInfo> GetSectypeAttributes(bool requireCurveAttributes)
        {
            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
            Type SMCommonMethodsController = SecMasterCoreAssembly.GetType("com.ivp.secm.api.SMCommonMethods");
            MethodInfo GetSectypeAttributes = SMCommonMethodsController.GetMethod("GetSectypeAttributes");

            object SMCommonMethodsControllerObj = Activator.CreateInstance(SMCommonMethodsController);
            var response = (Dictionary<string, SecurityTypeMasterInfo>)GetSectypeAttributes.Invoke(SMCommonMethodsControllerObj, new object[] { requireCurveAttributes });
            return response;
        }

        public Dictionary<string, AttrInfo> FetchCommonAttributes(bool requireCurveAttributes)
        {
            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
            Type SMCommonMethodsController = SecMasterCoreAssembly.GetType("com.ivp.secm.api.SMCommonMethods");
            MethodInfo FetchCommonAttributes = SMCommonMethodsController.GetMethod("FetchCommonAttributes");

            object SMCommonMethodsControllerObj = Activator.CreateInstance(SMCommonMethodsController);
            var response = (Dictionary<string, AttrInfo>)FetchCommonAttributes.Invoke(SMCommonMethodsControllerObj, new object[] { requireCurveAttributes });
            return response;
        }

        public static DataTable GetUserGroupLayoutPriority()
        {
            mLogger.Debug("GetUserGroupLayoutPriority -> Start");
            string tempTableName = "IVPSecMaster.dbo.tempUserLayoutReport_" + Guid.NewGuid().ToString().Replace("-", "_");
            try
            {
                var lstUsers = new com.ivp.rad.RUserManagement.RUserManagementService().GetAllUsersGDPR();
                DataTable otUser = new DataTable();
                otUser.Columns.Add("user_login_name");
                otUser.Columns.Add("user_name");
                foreach (var user in lstUsers)
                {
                    var orUser = otUser.NewRow();
                    orUser["user_login_name"] = user.UserLoginName;
                    orUser["user_name"] = user.UserName;
                    otUser.Rows.Add(orUser);
                }

                CommonDALWrapper.ExecuteSelectQuery("CREATE TABLE " + tempTableName + "(user_login_name VARCHAR(MAX), user_name VARCHAR(MAX))", ConnectionConstants.SecMaster_Connection);
                CommonDALWrapper.ExecuteBulkUpload(tempTableName, otUser, ConnectionConstants.SecMaster_Connection);

                var dtResult = CommonDALWrapper.ExecuteSelectQuery("EXEC IVPSecMaster.dbo.SECM_GetUserGroupLayoutPriority '" + tempTableName + "'", ConnectionConstants.SecMaster_Connection).Tables[0];
                return dtResult;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                try
                {
                    CommonDALWrapper.ExecuteSelectQuery("IF EXISTS(SELECT 1 FROM IVPSecMaster.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('" + tempTableName + "',1)) DROP TABLE " + tempTableName, ConnectionConstants.SecMaster_Connection);
                }
                catch (Exception ex)
                {

                }

                mLogger.Debug("GetUserGroupLayoutPriority -> End");
            }
        }
    }
}
