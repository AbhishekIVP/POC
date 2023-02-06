using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.RUserManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    public static class SRMCommonRAD
    {
        public static List<RUserInfo> lstUserInfo;
        public static List<RUserInfo> GetRADAllUserInfo()
        {
            return new RUserManagementService().GetAllUsersGDPR();
        }

        public static RUserInfo GetRADUserInfo(string userName)
        {
            return new RUserManagementService().GetUserInfoGDPR(userName);
        }

        public static string GetRADLoginNameFromUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            string loginName = string.Empty;
            RUserInfo userInfo = null;
            //userInfo = new RUserManagementService().GetUserInfoGDPR(userName);

            List<RUserInfo> lstAllUsers = new RUserManagementService().GetAllUsersGDPR();

            userInfo = lstAllUsers.Where(u => u.UserLoginName == userName).FirstOrDefault();

            if (userInfo != null)
                loginName = userInfo.UserName;
            return loginName;
        }

        public static Dictionary<string, string> GetRADLoginNameFromUserNameMultiple(List<string> userNames)
        {
            Dictionary<string, string> dictNames = new Dictionary<string, string>();
            string loginName = string.Empty;
            userNames.Where(u => !string.IsNullOrEmpty(u)).ToList().ForEach(un =>
            {

                RUserInfo userInfo = null;
                //userInfo = new RUserManagementService().GetUserInfoGDPR(un);

                List<RUserInfo> lstAllUsers = new RUserManagementService().GetAllUsersGDPR();

                userInfo = lstAllUsers.Where(u => u.UserLoginName == un).FirstOrDefault();

                if (userInfo != null)
                    dictNames.Add(un, userInfo.UserName);
                else
                    dictNames.Add(un, string.Empty);
            });

            return dictNames;
        }

        public static string GetUserDisplayNameWithUserNameFromInfo(RUserInfo userInfo)
        {
            return ((userInfo.UserName.ToLower().Equals("admin") ? userInfo.FirstName.Trim() : userInfo.FullName.Trim()) + " (" + userInfo.UserName.Trim() + ")").Trim();
        }

        public static string GetUserDisplayNameWithEmailFromInfo(RUserInfo userInfo)
        {
            return ((userInfo.UserName.ToLower().Equals("admin") ? userInfo.FirstName.Trim() : userInfo.FullName.Trim()) + " (" + userInfo.EmailId.Trim() + ")").Trim();
        }

        public static string GetUserDisplayNameFromUserName(string userName)
        {
            string displayName = userName;
            List<RUserInfo> allUsers = GetRADAllUserInfo();
            RUserInfo userInfo = allUsers.Where(u => u.UserLoginName == userName || (userName.ToLower() == "admin" && u.UserName.ToLower() == "admin")).FirstOrDefault();

            if (userInfo != null)
            {
                displayName = ((userInfo.UserName.ToLower().Equals("admin") ? userInfo.FirstName.Trim() : userInfo.FullName.Trim()) + " (" + userInfo.UserName.Trim() + ")").Trim();
            }

            return displayName;
        }

        public static DataTable ConvertLoginNameToDisplayNameForDataTable(DataTable dtTableToUpdate, List<string> columnsToUpdate, bool convertActiveOnly, bool removeInActiveRows)
        {
            if (dtTableToUpdate != null && dtTableToUpdate.Rows.Count > 0)
                ConvertLoginNameToDisplayName(dtTableToUpdate.AsEnumerable(), columnsToUpdate, convertActiveOnly, removeInActiveRows);
            return dtTableToUpdate;
        }

        public static void ConvertLoginNameToDisplayName(IEnumerable<DataRow> rowsToUpdate, List<string> columnsToUpdate, bool convertActiveOnly, bool removeInActiveRows)
        {
            if (rowsToUpdate != null && rowsToUpdate.Count() > 0)
            {
                Dictionary<string, string> dctLoginNamevsDisplayName = new Dictionary<string, string>();
                List<DataRow> lstRowsToRemove = new List<DataRow>();
                if (convertActiveOnly)
                    dctLoginNamevsDisplayName = GetAllUsersLoginNamevsDisplayName();
                else
                    dctLoginNamevsDisplayName = GetAllDBUsersLoginNamevsDisplayName();

                foreach (DataRow row in rowsToUpdate)
                {
                    bool removeRow = false;
                    foreach (string columnName in columnsToUpdate)
                    {
                        if (dctLoginNamevsDisplayName.ContainsKey(Convert.ToString(row[columnName])))
                            row[columnName] = dctLoginNamevsDisplayName[Convert.ToString(row[columnName])];
                        else if (removeInActiveRows)
                        {
                            removeRow = true;
                        }
                    }
                    if (removeRow)
                        lstRowsToRemove.Add(row);
                }
                foreach (DataRow dr in lstRowsToRemove)
                {
                    rowsToUpdate.ToList().Remove(dr);
                }
            }
        }

        public static Dictionary<string, string> GetAllUsersLoginNamevsDisplayName()
        {
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();
            Dictionary<string, string> dctUserInfo = lstUserInfo.ToDictionary(y => y.UserName.ToLower() == "admin" ? y.UserName : y.UserLoginName, x => GetUserDisplayNameWithUserNameFromInfo(x));
            if (!dctUserInfo.ContainsKey("admin"))
            {
                var adminInfo = GetRADUserInfo("admin");
                if (adminInfo != null)
                {
                    dctUserInfo.Add(adminInfo.UserName, GetUserDisplayNameWithUserNameFromInfo(adminInfo));
                }
            }

            RUserInfo tempAdmin = lstUserInfo.Where(y => y.UserName.ToLower() == "admin").FirstOrDefault();

            if (tempAdmin != null)
            {
                dctUserInfo[tempAdmin.UserLoginName] = GetUserDisplayNameWithUserNameFromInfo(tempAdmin);
            }

            return dctUserInfo;
        }

        public static Dictionary<string, string> GetAllDBUsersLoginNamevsDisplayName()
        {
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersFromDBGDPR();
            Dictionary<string, string> dctUserInfo = lstUserInfo.ToDictionary(y => y.UserName.ToLower() == "admin" ? y.UserName : y.UserLoginName, x => GetUserDisplayNameWithUserNameFromInfo(x));
            return dctUserInfo;
        }

        public static Dictionary<string, string> GetAllDisplayNamevsUsersLoginName()
        {
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();
            Dictionary<string, string> dctUserInfo = lstUserInfo.ToDictionary(x => GetUserDisplayNameWithUserNameFromInfo(x), y => y.UserName.ToLower() == "admin" ? y.UserName : y.UserLoginName, StringComparer.OrdinalIgnoreCase);
            if (!dctUserInfo.ContainsKey("admin") && !dctUserInfo.ContainsKey("Admin (admin)"))
            {
                var adminInfo = GetRADUserInfo("admin");
                if (adminInfo != null)
                {
                    dctUserInfo.Add(GetUserDisplayNameWithUserNameFromInfo(adminInfo), adminInfo.UserName);
                }
            }
            return dctUserInfo;
        }

        public static List<string> GetAllGroups()
        {
            return new RUserManagementService().GetAllGroupsGDPR().Select(x => x.GroupName).ToList();
        }

        public static HashSet<string> GetCalendarNames()
        {
            return new HashSet<string>(new RCalendarController().GetAllCalendars().Tables[0].AsEnumerable().Select(x => Convert.ToString(x["calendar_name"])), StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, int> GetCalendarNameVsId()
        {
            return new RCalendarController().GetAllCalendars().Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["calendar_name"]), y => Convert.ToInt32(y["calendar_id"]), StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, List<string>> GetUsersEmailVsDisplayName()
        {
            var dict = new RUserManagementService().GetAllUsersGDPR().GroupBy(a => a.EmailId).ToDictionary(x => x.Key, b => b.Select(y => y.FullName + " <" + y.UserName + ">").ToList());
            return dict;
        }
        public static Dictionary<string, string> GetUsersDisplayNameVsEmail()
        {
            var dict = new RUserManagementService().GetAllUsersGDPR().ToDictionary(y => y.FullName + " <" + y.UserName + ">", x => x.EmailId);
            return dict;
        }

        public static List<string> GetUserDataPrivilege(string username, int entityTypeId, bool requireRealName, bool requireEntityCode, string tableAlias)
        {
            var response = new List<string>();
            var userManagementService = new RUserManagementService();
            var dataPrivileges = userManagementService.GetUserDataPrivilegeGDPR(username);

            foreach (var privilegeInfo in dataPrivileges)
            {
                foreach (var hierarchyInfo in privilegeInfo.HierarchyDetail.HierarchyEntitiesInfo)
                {
                    if (hierarchyInfo.EntityTypeId == entityTypeId.ToString())
                    {
                        var attributeIdVsName = hierarchyInfo.AttributesInfo.ToDictionary(x => x.AttributeId, y => y.AttributeName);
                        foreach (var entityDetail in hierarchyInfo.EntityData)
                        {
                            string attributeName;
                            if (attributeIdVsName.TryGetValue(entityDetail.Key, out attributeName) && entityDetail.Value.Count > 0)
                            {
                                if (!requireRealName)
                                {
                                    StringBuilder query = new StringBuilder("[").Append(attributeName).Append("]");
                                    if (!string.IsNullOrEmpty(tableAlias))
                                        query = new StringBuilder(tableAlias).Append(".[").Append(attributeName).Append("]");

                                    if (entityDetail.Value.Count > 1)
                                    {
                                        if (requireEntityCode)
                                            query.Append(" IN ('").Append(string.Join("','", entityDetail.Value.Select(x => x.Key))).Append("')");
                                        else
                                            query.Append(" IN ('").Append(string.Join("','", entityDetail.Value.Select(x => x.Value))).Append("')");
                                    }
                                    else
                                    {
                                        if (requireEntityCode)
                                            query.Append(" = ").Append(entityDetail.Value.First().Key);
                                        else
                                            query.Append(" = ").Append(entityDetail.Value.First().Value);
                                    }
                                    response.Add(query.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }
    }
}
