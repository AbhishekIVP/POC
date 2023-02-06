using com.ivp.rad.RRadWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.srmcommon;
using com.ivp.rad.RUserManagement;
using System.Web.Script.Serialization;
using com.ivp.rad.common;

namespace com.ivp.common
{
    public class SRMRADWorkflow
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowController");
        public bool TriggerWorkFlow(List<TriggerInfo> lstTriggerInfo, Action<List<TriggerInfo>> callback)
        {
            RWorkFlowService rwService = new RWorkFlowService();
            MassageUserNamesTriggerInfo(lstTriggerInfo);
            bool output = rwService.TriggerWorkFlow(lstTriggerInfo, callback);
            if (lstTriggerInfo != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    mLogger.Debug("TriggerWorkFlow -> Output From Rad: " + serializer.Serialize(lstTriggerInfo));
                }
                catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("TriggerWorkFlow -> Output From Rad: NULL");
            }
            return output;
        }

        public bool ReTriggerWorkFlow(List<TriggerInfo> triggerInfo, Action<List<TriggerInfo>> callback)
        {
            RWorkFlowService rwService = new RWorkFlowService();
            MassageUserNamesTriggerInfo(triggerInfo);
            bool output = rwService.ReTriggerWorkFlow(triggerInfo, callback);
            if (triggerInfo != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    mLogger.Debug("ReTriggerWorkFlow -> Output From Rad: " + serializer.Serialize(triggerInfo));
                }
                catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("ReTriggerWorkFlow -> Output From Rad: NULL");
            }
            return output;
        }

        public bool CancelWorkflowRequest(List<TriggerInfo> triggerInfo)
        {
            RWorkFlowService rwService = new RWorkFlowService();
            MassageUserNamesTriggerInfo(triggerInfo);
            bool output = rwService.FailWorkflowInstance(triggerInfo);
            if (triggerInfo != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    mLogger.Debug("CancelWorkflowRequest -> Output From Rad: " + serializer.Serialize(triggerInfo));
                }
                catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("CancelWorkflowRequest -> Output From Rad: NULL");
            }
            return output;
        }

        public List<RWorkFlowInstanceInfo> GetWorkflowInstanceAudit(int instance_id)
        {
            RWorkFlowService rwService = new RWorkFlowService();
            List<RWorkFlowInstanceInfo> output = rwService.GetWorkflowInstanceAudit(instance_id);
            if (output != null)
            {
                //try
                //{
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    serializer.MaxJsonLength = Int32.MaxValue;
                //    mLogger.Debug("GetWorkflowInstanceAudit -> Output From Rad: " + serializer.Serialize(output));
                //}
                //catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("GetWorkflowInstanceAudit -> Output From Rad: NULL");
            }
            return output;
        }

        public List<RWorkFlowInstanceInfo> GetAllWOrkFlowInstances(WorkFlowStatus status, string user, List<int> instances)
        {
            user = MassageUserName(user);
            List<RWorkFlowInstanceInfo> output = new RWorkFlowService().GetAllWOrkFlowInstances(status, user, instances);
            if (output != null)
            {
                //try
                //{
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    serializer.MaxJsonLength = Int32.MaxValue;
                //    mLogger.Debug("GetAllWorkflowInstances -> Output From Rad: " + serializer.Serialize(output));
                //}
                //catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("GetAllWorkflowInstances -> Output From Rad: NULL");
            }
            return output;
        }

        public List<int> GetFinalWorkflowInstanceStateStatedByUser(string user, string state, int moduleId)
        {
            user = MassageUserName(user);
            List<int> output = new RWorkFlowService().GetFinalWorkflowInstanceStateStatedByUser(user, state, moduleId);
            if (output != null)
            {
                output = output.Distinct().ToList();
                //try
                //{
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    serializer.MaxJsonLength = Int32.MaxValue;
                //    mLogger.Debug("GetFinalWorkflowInstanceStateStatedByUser -> Output From Rad: " + serializer.Serialize(output));
                //}
                //catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("GetFinalWorkflowInstanceStateStatedByUser -> Output From Rad: NULL");
            }
            return output;
        }

        public bool MoveWorkflow(List<TriggerInfo> triggerInfo, Action<List<TriggerInfo>> callback)
        {
            MassageUserNamesTriggerInfo(triggerInfo);
            bool result = new RWorkFlowService().MoveWorkflow(triggerInfo, callback);
            if (triggerInfo != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    mLogger.Debug("MoveWorkflow -> Output From Rad: " + serializer.Serialize(triggerInfo));
                }
                catch (Exception ex) { }
            }
            else
            {
                mLogger.Debug("MoveWorkflow -> Output From Rad: NULL");
            }
            return result;
        }

        public List<RWorkFlowInstanceInfo> GetActionOfInstanceForUser(List<int> instanceID, string user)
        {
            user = MassageUserName(user);
            List<RWorkFlowInstanceInfo> output = new RWorkFlowService().GetActionOfInstanceForUser(instanceID, user);
            if (output != null)
            {
                //try
                //{
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    serializer.MaxJsonLength = Int32.MaxValue;
                //    mLogger.Debug("GetActionOfInstanceForUser -> Output From Rad: " + serializer.Serialize(output));
                //}
                //catch (Exception ex) { }

            }
            else
            {
                mLogger.Debug("GetActionOfInstanceForUser -> Output From Rad: NULL");
            }
            return output;
        }

        public List<RWorkFlowInstanceInfo> GetPendingInfoForInstances(List<int> instanceID, string user)
        {
            user = MassageUserName(user);
            List<RWorkFlowInstanceInfo> output = new RWorkFlowService().GetActionOfInstanceForUser(instanceID, null);
            if (output != null)
            {
                //try
                //{
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    serializer.MaxJsonLength = Int32.MaxValue;
                //    mLogger.Debug("GetPendingInfoForInstances -> Output From Rad: " + serializer.Serialize(output));
                //}
                //catch (Exception ex) { }

            }
            else
            {
                mLogger.Debug("GetPendingInfoForInstances -> Output From Rad: NULL");
            }
            return output;
        }

        public string MassageUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;
            return userName.ToLower() == "admin" ? "admin" : SRMCommonRAD.GetRADLoginNameFromUserName(userName);
        }

        public void MassageUserNamesTriggerInfo(List<TriggerInfo> triggerInfo)
        {
            List<string> allUsers = triggerInfo.Select(t => t.user).Distinct().ToList();
            Dictionary<string, string> dictNames = null;

            if (allUsers != null && allUsers.Count > 0)
            {
                dictNames = SRMCommonRAD.GetRADLoginNameFromUserNameMultiple(allUsers);

                foreach (KeyValuePair<string, string> kvp in dictNames)
                {
                    triggerInfo.Where(t => t.user == kvp.Key && t.user.ToLower() != "admin").ToList().ForEach(ti => { ti.user = kvp.Value; });
                }

            }

        }

        public Dictionary<string, string> GetAllUserNames()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();
            if (lstUserInfo != null)
            {
                lstUserInfo.ForEach(user =>
                {
                    if (!dict.ContainsKey(user.UserName))
                    {
                        dict.Add(user.UserName, (user.UserName.ToLower() == "admin" ? user.FirstName.Trim() : user.FullName.Trim()) + " (" + user.UserName.Trim() + ")");
                    }
                });
            }

            return dict;
        }

        public Dictionary<string, string> GetAllUserLoginNames()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();
            if (lstUserInfo != null)
            {
                lstUserInfo.ForEach(user =>
                {
                    if (!dict.ContainsKey(user.UserLoginName))
                    {
                        dict.Add(user.UserLoginName, (user.UserName.ToLower() == "admin" ? user.FirstName.Trim() : user.FullName.Trim()) + " (" + user.UserName.Trim() + ")");
                    }
                });
            }

            return dict;
        }

        private static string GetUserDisplayNameWithUserNameFromInfo(RUserInfo userInfo)
        {
            return ((userInfo.UserName.ToLower().Equals("admin") ? userInfo.FirstName.Trim() : userInfo.FullName.Trim()) + " (" + userInfo.UserName.Trim() + ")").Trim();
        }
    }
}
