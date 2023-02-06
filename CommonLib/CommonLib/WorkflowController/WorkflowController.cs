using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using System.Net;
using com.ivp.rad.utils;
using com.ivp.rad.notificationmanager;
using com.ivp.rad.transport;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using com.ivp.refmaster.common;
using com.ivp.refmaster.refmasterwebservices;
using System.Timers;
//using com.ivp.refmaster.controller;
using Newtonsoft.Json;

namespace com.ivp.common
{
    public static class WorkflowController
    {
        private static Dictionary<string, SMWorkflowCachedInfo> dictGuidVsCachedInfo = new Dictionary<string, SMWorkflowCachedInfo>();
        private static Dictionary<string, DateTime> dictGuidVsValidUpto = new Dictionary<string, DateTime>();
        private static Timer timer = new Timer(30000);

        private static IRLogger mLogger = RLogFactory.CreateLogger("Common.WorkflowController");
        private static RHashlist mHList = null;

        static WorkflowController()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<string> keysToRemove = new List<string>();
            lock (((ICollection)dictGuidVsValidUpto).SyncRoot)
            {
                foreach (KeyValuePair<string, DateTime> kvp in dictGuidVsValidUpto)
                {
                    DateTime now = DateTime.Now;
                    if (kvp.Value < now)
                    {
                        RemoveKey(kvp.Key, false);
                        keysToRemove.Add(kvp.Key);
                    }
                }
                foreach (string key in keysToRemove)
                {
                    dictGuidVsValidUpto.Remove(key);
                }
            }
        }

        public static DataTable SMGetDownStreamSystems(string userName)
        {
            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
            Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMSearchController");
            MethodInfo GetExternalSystemDetails = objType.GetMethod("GetExternalSystemDetails");

            object obj = Activator.CreateInstance(objType);
            return (DataTable)GetExternalSystemDetails.Invoke(obj, new object[] { userName, true });
        }

        public static void SMPostToDownstream(List<int> systemIDs, List<string> secIds, string userName)
        {
            Assembly SMDownstreamSystemInterface = Assembly.Load("SecMasterDownstreamSystemInterface");
            Type objType = SMDownstreamSystemInterface.GetType("com.ivp.secm.secmasterdownstreamsysteminterface.SMDownstreamSystemInterface");
            Type objSMDownstreamSystemInterfaceInfoType = SMDownstreamSystemInterface.GetType("com.ivp.secm.secmasterdownstreamsysteminterface.SMDownstreamSystemInterfaceInfo");
            MethodInfo PushToExternalSystem = objType.GetMethod("PushToExternalSystem");

            object obj = Activator.CreateInstance(objType);
            object objSMDownstreamSystemInterfaceInfo = Activator.CreateInstance(objSMDownstreamSystemInterfaceInfoType);

            PropertyInfo propInfo = objSMDownstreamSystemInterfaceInfoType.GetProperty("SecurityIdList");
            propInfo.SetValue(objSMDownstreamSystemInterfaceInfo, secIds, null);

            propInfo = objSMDownstreamSystemInterfaceInfoType.GetProperty("SystemIds");
            propInfo.SetValue(objSMDownstreamSystemInterfaceInfo, systemIDs, null);

            propInfo = objSMDownstreamSystemInterfaceInfoType.GetProperty("userName");
            propInfo.SetValue(objSMDownstreamSystemInterfaceInfo, userName, null);

            propInfo = objSMDownstreamSystemInterfaceInfoType.GetProperty("PushIdentifier");
            propInfo.SetValue(objSMDownstreamSystemInterfaceInfo, 0, null);

            PushToExternalSystem.Invoke(obj, new object[] { objSMDownstreamSystemInterfaceInfo });

        }

        public static DataSet GetWorkflowStatusNotificationData(string username)
        {
            DataSet ds = null;
            mLogger.Debug("GetWorkflowStatusNotificationData -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.REFM_GetWorkflowStatusNotificationData '{0}'", username), ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowStatusNotificationData -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetWorkflowStatusNotificationData -> End");
            return ds;
        }

        public static DataSet GetWorkflowStatusData(WorkflowGridInput inputObject)
        {
            DataSet ds = null;
            mLogger.Debug("GetWorkflowStatusData -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetWorkflowStatusData '{0}', {1}, {2}, {3}, '{4}', '{5}', {6}", inputObject.userName, (inputObject.getMyRequests ? 1 : 0), (inputObject.getRejectedRequests ? 1 : 0), (inputObject.getRequestsPending ? 1 : 0), inputObject.sectypeIds, inputObject.securityId, (inputObject.getAllRequests ? 1 : 0)), ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowStatusData -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetWorkflowStatusData -> End");
            return ds;
        }

        public static DataSet GetRMWorkflowStatusData(WorkflowGridInput inputObject)
        {
            DataSet ds = null;
            mLogger.Debug("GetRMWorkflowStatusData -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.REFM_GetWorkflowStatusData '{0}', {1}, {2}, {3}, {4}, {5}", inputObject.userName, (inputObject.getMyRequests ? 1 : 0), (inputObject.getRejectedRequests ? 1 : 0), (inputObject.getRequestsPending ? 1 : 0), (inputObject.getAllRequests ? 1 : 0), (!string.IsNullOrEmpty(inputObject.sectypeIds) ? inputObject.sectypeIds : string.Empty)), ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error("GetRMWorkflowStatusData -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetRMWorkflowStatusData -> End");
            return ds;
        }

        public static DataSet GetWorkflowRequestLog(int queueId, bool getPending, out bool status)
        {
            DataSet ds = null;
            status = true;
            mLogger.Debug("GetWorkflowRequestLog -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @queue_id INT, @get_pending BIT, @max_workflow_sequence INT, @current_workflow_sequence INT

DECLARE @queue_statuses TABLE (status_id INT, queue_id INT, is_approved BIT, is_rejected BIT, is_deleted BIT, is_suppressed BIT, action_by VARCHAR(200), action_on DATETIME, remarks VARCHAR(MAX), workflow_sequence INT, is_active BIT, next_levels BIT)

SELECT @queue_id = {0}, @get_pending = {1}

IF(@get_pending = 1)
BEGIN
	SELECT @current_workflow_sequence = workflow_sequence + 1 FROM IVPSecMaster.dbo.ivp_secm_security_workflow_queue_status WHERE queue_id = @queue_id AND is_active = 1
	
	SELECT @max_workflow_sequence = MAX(workflow_sequence) FROM IVPSecMaster.dbo.ivp_secm_security_workflow_group_user_mapping gum
	INNER JOIN IVPSecMaster.dbo.ivp_secm_security_workflow_requests_queue rQ
	ON rQ.workflow_instance_id = gum.workflow_instance_id AND gum.is_active = 1 AND rQ.is_active = 1 AND queue_id = @queue_id
	
	WHILE(@max_workflow_sequence >= @current_workflow_sequence)
	BEGIN
		INSERT INTO @queue_statuses(status_id, queue_id, workflow_sequence, is_active, next_levels)
		VALUES(0, @queue_id, @current_workflow_sequence, 1, 1)
		
		SELECT @current_workflow_sequence += 1
	END
END

INSERT INTO @queue_statuses
SELECT status_id, queue_id, is_approved, is_rejected, is_deleted, is_suppressed, action_by, CONVERT(VARCHAR(10), action_on, 101) + ' ' + LTRIM(RIGHT(CONVERT(VARCHAR(20), action_on, 20), 8)) AS action_on, remarks, workflow_sequence, is_active, 0 AS next_levels
FROM IVPSecMaster.dbo.ivp_secm_security_workflow_queue_status
WHERE queue_id = @queue_id AND (is_approved = 1 OR is_rejected = 1)
UNION ALL
SELECT status_id, queue_id, is_approved, is_rejected, is_deleted, is_suppressed, action_by, CONVERT(VARCHAR(10), action_on, 101) + ' ' + LTRIM(RIGHT(CONVERT(VARCHAR(20), action_on, 20), 8)) AS action_on, remarks, workflow_sequence, is_active, 0 AS next_levels
FROM IVPSecMaster.dbo.ivp_secm_security_workflow_queue_status
WHERE queue_id = @queue_id AND is_active=1 AND @get_pending = 1

SELECT * FROM @queue_statuses
ORDER BY next_levels DESC, status_id DESC, workflow_sequence DESC

SELECT workflow_group_user_name, workflow_sequence
FROM IVPSecMaster.dbo.ivp_secm_security_workflow_requests_queue wrq
INNER JOIN IVPSecMaster.dbo.ivp_secm_security_workflow_group_user_mapping apprvr
ON apprvr.workflow_instance_id = wrq.workflow_instance_id AND wrq.is_active=1 AND apprvr.is_active=1 AND wrq.queue_id=@queue_id
ORDER BY workflow_sequence DESC, workflow_group_user_name", queueId, (getPending ? 1 : 0)), ConnectionConstants.SecMaster_Connection);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                mLogger.Error("GetWorkflowRequestLog -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetWorkflowRequestLog -> End");
            return ds;
        }

        public static DataSet GetRMWorkflowRequestLog(int queueId, bool getPending, out bool status)
        {
            DataSet ds = null;
            status = true;
            mLogger.Debug("GetRMWorkflowRequestLog -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @queue_id INT, @get_pending BIT, @max_workflow_sequence INT, @current_workflow_sequence INT

                DECLARE @queue_statuses TABLE (status_id INT, queue_id INT, is_approved BIT, is_rejected BIT, is_deleted BIT, is_suppressed BIT, action_by VARCHAR(200), action_on DATETIME, remarks VARCHAR(MAX), workflow_sequence INT, is_active BIT, next_levels BIT)

                SELECT @queue_id = {0}, @get_pending = {1}

                IF(@get_pending = 1)
                BEGIN
	                SELECT @current_workflow_sequence = workflow_sequence + 1 FROM IVPRefMaster.dbo.ivp_refm_workflow_queue_status WHERE queue_id = @queue_id AND is_active = 1
	
	                SELECT @max_workflow_sequence = MAX(workflow_sequence) FROM IVPRefMaster.dbo.ivp_refm_workflow_group_user_mapping gum
	                INNER JOIN IVPRefMaster.dbo.ivp_refm_workflow_requests_queue rQ
	                ON rQ.workflow_instance_id = gum.workflow_instance_id AND gum.is_active = 1 AND rQ.is_active = 1 AND queue_id = @queue_id
	
	                WHILE(@max_workflow_sequence >= @current_workflow_sequence)
	                BEGIN
		                INSERT INTO @queue_statuses(status_id, queue_id, workflow_sequence, is_active, next_levels)
		                VALUES(0, @queue_id, @current_workflow_sequence, 1, 1)
		
		                SELECT @current_workflow_sequence += 1
	                END
                END

                INSERT INTO @queue_statuses
                SELECT status_id, queue_id, is_approved, is_rejected, is_deleted, is_suppressed, action_by, CONVERT(VARCHAR(10), action_on, 101) + ' ' + LTRIM(RIGHT(CONVERT(VARCHAR(20), action_on, 20), 8)) AS action_on, remarks, workflow_sequence, is_active, 0 AS next_levels
                FROM IVPRefMaster.dbo.ivp_refm_workflow_queue_status
                WHERE queue_id = @queue_id AND (is_approved = 1 OR is_rejected = 1)
                UNION ALL
                SELECT status_id, queue_id, is_approved, is_rejected, is_deleted, is_suppressed, action_by, CONVERT(VARCHAR(10), action_on, 101) + ' ' + LTRIM(RIGHT(CONVERT(VARCHAR(20), action_on, 20), 8)) AS action_on, remarks, workflow_sequence, is_active, 0 AS next_levels
                FROM IVPRefMaster.dbo.ivp_refm_workflow_queue_status
                WHERE queue_id = @queue_id AND is_active=1 AND @get_pending = 1

                SELECT * FROM @queue_statuses
                ORDER BY next_levels DESC, status_id DESC, workflow_sequence DESC

                SELECT workflow_group_user_name, workflow_sequence
                FROM IVPRefMaster.dbo.ivp_refm_workflow_requests_queue wrq
                INNER JOIN IVPRefMaster.dbo.ivp_refm_workflow_group_user_mapping apprvr
                ON apprvr.workflow_instance_id = wrq.workflow_instance_id AND wrq.is_active=1 AND apprvr.is_active=1 AND wrq.queue_id=@queue_id
                ORDER BY workflow_sequence DESC, workflow_group_user_name", queueId, (getPending ? 1 : 0)), ConnectionConstants.RefMaster_Connection);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                mLogger.Error("GetRMWorkflowRequestLog -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetRMWorkflowRequestLog -> End");
            return ds;
        }

        public static DataSet GetWorkflowQueueIdVsAttributeId(string queueIds)
        {
            DataSet ds = null;
            mLogger.Debug("GetWorkflowQueueIdVsAttributeId -> Start");
            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT attribute_id
FROM IVPSecMaster.dbo.ivp_secm_security_workflow_requests_queue wrq
INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{0}', ',') qids
ON qids.item = wrq.queue_id", queueIds), ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowQueueIdVsAttributeId -> Exception ->" + ex.ToString());
                throw ex;
            }
            mLogger.Debug("GetWorkflowQueueIdVsAttributeId -> End");
            return ds;
        }

        private static WorkflowHandlerResponseInfo ProcessWorkflowRequsts()
        {
            return null;
        }

        public static void SendMail(WorkflowMailInfo workflowSubscriptionInfo, WorkflowMailConfigInfo mailConfigInfo, bool isRefmaster = false)
        {
            mLogger.Debug("SendMail -> Start");
            try
            {
                var mailSegment = mailConfigInfo.MailSegment;

                string updateType = string.Empty;
                switch (workflowSubscriptionInfo.UpdateType.ToUpper())
                {
                    case "CREATE":
                        updateType = "Creation";
                        break;
                    case "UPDATE":
                        updateType = "Update";
                        break;
                    case "TIMESERIES":
                        updateType = "Time series correction";
                        break;
                }

                ///SUBJECT
                mailSegment[0] = mailSegment[0].Replace("[UpdateType]", updateType);
                mailSegment[0] = mailSegment[0].Replace("[ActionBy]", workflowSubscriptionInfo.ActionBy);
                mailSegment[0] = mailSegment[0].Replace("[RequestedTime]", workflowSubscriptionInfo.RequestedTime.ToString("MM/dd/yyyy"));

                ///BODY
                var userAndGroupNames = new List<string>();
                if (workflowSubscriptionInfo.GroupNameVsUserNameVsMailId != null && workflowSubscriptionInfo.GroupNameVsUserNameVsMailId.Count > 0)
                {
                    userAndGroupNames.AddRange(workflowSubscriptionInfo.GroupNameVsUserNameVsMailId.Keys.OrderBy(x => x));
                }
                if (workflowSubscriptionInfo.UserNameVsMailId != null && workflowSubscriptionInfo.UserNameVsMailId.Count > 0)
                {
                    userAndGroupNames.AddRange(workflowSubscriptionInfo.UserNameVsMailId.Keys.OrderBy(x => x));
                }
                bool multipleSecurities = false;
                if (isRefmaster)
                    multipleSecurities = workflowSubscriptionInfo.TableInfo.AsEnumerable().Select(x => x.Field<string>("Entity Code")).Distinct().Count() > 1;
                else
                    multipleSecurities = workflowSubscriptionInfo.TableInfo.AsEnumerable().Select(x => x.Field<string>("Security Id")).Distinct().Count() > 1;

                mailSegment[1] = mailSegment[1].Replace("[UpdateType]", workflowSubscriptionInfo.UpdateType);
                if (isRefmaster)
                {
                    mailSegment[1] = mailSegment[1].Replace("[entity]", multipleSecurities ? "entities" : "entity");
                    mailSegment[1] = mailSegment[1].Replace("[entityisare]", multipleSecurities ? "are" : "is");
                }
                else
                {
                    mailSegment[1] = mailSegment[1].Replace("[security]", multipleSecurities ? "securities" : "security");
                    mailSegment[1] = mailSegment[1].Replace("[securityisare]", multipleSecurities ? "are" : "is");
                }
                mailSegment[1] = mailSegment[1].Replace("[UserNames,GroupNames]", (string.Join(", ", (userAndGroupNames.Count > 5 ? userAndGroupNames.Take(5) : userAndGroupNames)) + (userAndGroupNames.Count > 5 ? (" and " + (userAndGroupNames.Count - 5) + " others") : string.Empty)));
                mailSegment[1] = mailSegment[1].Replace("[ActionBy]", workflowSubscriptionInfo.ActionBy);
                mailSegment[1] = mailSegment[1].Replace("[ActionTime]", String.Format("{0:MMM d, h:m tt}", workflowSubscriptionInfo.ActionTime));
                mailSegment[1] = mailSegment[1].Replace("[RequestedBy]", workflowSubscriptionInfo.RequestedBy);
                mailSegment[1] = mailSegment[1].Replace("[RequestedTime]", String.Format("{0:MMM d, h:m tt}", workflowSubscriptionInfo.RequestedTime));
                mailSegment[1] = mailSegment[1].Replace("[Comments]", workflowSubscriptionInfo.Comments);

                string strAttr = string.Empty;
                if (workflowSubscriptionInfo.TableInfo != null && workflowSubscriptionInfo.TableInfo.Rows.Count > 0)
                {
                    //string tableHtml = string.Empty;
                    //StringBuilder strHtml = new StringBuilder("<html>");

                    StringBuilder tblHeader = new StringBuilder("<thead><tr>");
                    foreach (DataColumn dcCol in workflowSubscriptionInfo.TableInfo.Columns)
                    {
                        tblHeader.Append("<th style=\"border-top:1px solid grey;border-left:1px solid grey;\">" + WebUtility.HtmlEncode(dcCol.ColumnName) + "</th>");
                    }
                    tblHeader.Append("</tr></thead>");

                    StringBuilder tblBody = new StringBuilder("<tbody>");
                    foreach (DataRow drRow in workflowSubscriptionInfo.TableInfo.Rows)
                    {
                        tblBody.Append("<tr>");
                        foreach (DataColumn dcCol in workflowSubscriptionInfo.TableInfo.Columns)
                        {
                            tblBody.Append("<td style=\"border-top:1px solid grey;border-left:1px solid grey;\">" + WebUtility.HtmlEncode(Convert.ToString(drRow[dcCol])) + "</td>");
                        }
                        tblBody.Append("</tr>");
                    }
                    tblBody.Append("</tbody>");

                    StringBuilder strAttribute = new StringBuilder();
                    //StringBuilder strMain = new StringBuilder("<html>");
                    //strMain.Append("<table>");
                    //strMain.Append("<tr><td>").Append("Hi,<br><br>").Append(mailSegment[1].Replace("[TableInfo]", string.Empty)).Append("</td></tr>");
                    //strMain.Append("<tr><td>&nbsp;</td></tr>");
                    strAttribute.Append("<tr><td><table cellPadding=\"5\" cellSpacing=\"0\" width=\"100%\" style=\"border-bottom:1px solid grey;border-right:1px solid grey;\">").Append(tblHeader).Append(tblBody).Append("</table></td></tr>");
                    //strMain.Append("<tr><td>&nbsp;</td></tr><tr><td>Regards</td></tr><tr><td>&nbsp;</td></tr><tr><td>This is an automated mail. Please do not reply.</td></tr>");
                    //strMain.Append("</table>");
                    //strMain.Append("</html>");
                    //mailSegment[1] = strMain.ToString();
                    strAttr = strAttribute.ToString();
                }
                //else
                //{
                //    StringBuilder strMain = new StringBuilder("<html>");
                //    strMain.Append("<table>");
                //    strMain.Append("<tr><td>").Append("Hi,<br><br>").Append(mailSegment[1].Replace("[TableInfo]", string.Empty)).Append("</td></tr>");
                //    strMain.Append("</table>");
                //    strMain.Append("</html>");
                //    mailSegment[1] = strMain.ToString();
                //}

                StringBuilder strMain = new StringBuilder("<html>");
                strMain.Append("<table>");
                strMain.Append("<tr><td>").Append("Hi,<br><br>").Append(mailSegment[1].Replace("[TableInfo]", string.Empty)).Append("</td></tr>");
                strMain.Append("<tr><td>&nbsp;</td></tr>");
                strMain.Append(strAttr);
                strMain.Append("<tr><td>&nbsp;</td></tr><tr><td>Regards</td></tr><tr><td>&nbsp;</td></tr><tr><td>This is an automated mail. Please do not reply.</td></tr>");
                strMain.Append("</table>");
                strMain.Append("</html>");
                mailSegment[1] = strMain.ToString();

                var lstMailTo = new List<string>();
                lstMailTo.AddRange(workflowSubscriptionInfo.UserNameVsMailId.Values.Where(x => !string.IsNullOrEmpty(x)));
                lstMailTo.AddRange(workflowSubscriptionInfo.GroupNameVsUserNameVsMailId.SelectMany(x => x.Value.Select(y => y.Value).Where(y => !string.IsNullOrEmpty(y))));
                if (lstMailTo.Count > 0)
                    lstMailTo = lstMailTo.Distinct().OrderBy(x => x).ToList();
                else
                {
                    if (!string.IsNullOrEmpty(workflowSubscriptionInfo.RequestedByMailId))
                    {
                        switch (workflowSubscriptionInfo.Action)
                        {
                            case WorkflowActionEnum.INTERMEDIATE_REQUEST:
                            case WorkflowActionEnum.REJECTED:
                                lstMailTo.Add(workflowSubscriptionInfo.RequestedByMailId);
                                break;
                        }
                    }
                }

                var subscriptionInfo = new SMWorkflowSubscriptionInfo();
                switch (workflowSubscriptionInfo.Action)
                {
                    case WorkflowActionEnum.INITIAL_REQUEST:
                    case WorkflowActionEnum.INTERMEDIATE_REQUEST:
                    case WorkflowActionEnum.REJECTED:
                        subscriptionInfo.MailTo = string.Join(";", lstMailTo);
                        subscriptionInfo.MailCc = workflowSubscriptionInfo.RequestedByMailId;
                        break;
                    case WorkflowActionEnum.APPROVED:
                    case WorkflowActionEnum.REJECTEDTOLAST:
                    case WorkflowActionEnum.SUPPRESSED:
                    case WorkflowActionEnum.DELETED:
                        subscriptionInfo.MailTo = workflowSubscriptionInfo.RequestedByMailId;
                        break;
                    case WorkflowActionEnum.REVOKED:
                        subscriptionInfo.MailTo = string.Join(";", lstMailTo);
                        break;
                }

                if (!string.IsNullOrEmpty(subscriptionInfo.MailTo))
                {
                    subscriptionInfo.MailFrom = mailConfigInfo.FromEmailIdForWorkflow;
                    subscriptionInfo.MailSubject = mailSegment[0];
                    subscriptionInfo.MailBody = mailSegment[1];

                    if (string.IsNullOrEmpty(subscriptionInfo.MailTo))
                    {
                        subscriptionInfo.MailCc = string.Empty;
                        subscriptionInfo.MailTo = workflowSubscriptionInfo.RequestedByMailId;
                    }

                    SendSubscriptionMail(subscriptionInfo, mailConfigInfo);
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("SendMail -> Exception -> " + ee.ToString());
                throw ee;
            }
            mLogger.Debug("SendMail -> End");
        }

        internal static void SendSubscriptionMail(SMWorkflowSubscriptionInfo subscriptionInfo, WorkflowMailConfigInfo mailConfigInfo)
        {
            mLogger.Debug("SMTaskSubscriptionMail: SendMail -> Start");
            string emailTo = string.Empty;
            RNotificationInfo notificationInfo = null;
            RMailContent mailContent = null;
            try
            {
                notificationInfo = new RNotificationInfo();
                mailContent = new RMailContent();

                notificationInfo.NoOfRetry = Convert.ToByte(mailConfigInfo.NoOfRetry);
                notificationInfo.NotificationFrequency = Convert.ToByte(mailConfigInfo.NotificationFrequency);
                notificationInfo.TransportName = mailConfigInfo.TransportName;


                mailContent.To = subscriptionInfo.MailTo;
                mailContent.From = subscriptionInfo.MailFrom;
                if (subscriptionInfo.MailCc != null)
                    mailContent.CC = subscriptionInfo.MailCc;
                //mailContent.Bcc = this.baseSubscriptionInfo.MailBcc;
                mailContent.Subject = subscriptionInfo.MailSubject;
                mailContent.Body = subscriptionInfo.MailBody;
                mailContent.IsBodyHTML = true;

                notificationInfo.NotificationMessage = mailContent;
                if (mailContent.To.Length > 0)
                {
                    System.Threading.Thread taskThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendMailInThread));
                    taskThread.Start(notificationInfo);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally { mLogger.Debug("SMTaskSubscriptionMail: SendMail -> End"); }
        }

        internal static void SendMailInThread(object objNotificationInfo)
        {
            RNotificationManager manager = new RNotificationManager();
            RNotificationInfo notificationInfo = (RNotificationInfo)objNotificationInfo;
            manager.GenerateNotification(notificationInfo);
        }

        public static string SMSaveWorkflow(XDocument info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            mLogger.Debug("WorkflowController : Start -> SMSaveWorkflow");
            try
            {
                mHList = new RHashlist();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPSecMaster].[dbo].[SECM_SaveUpdateWorkflow] @infoDoc='" + info + "', @workflowName='" + workflowName + "', @isCreate=" + isCreate + ", @applyTimeSeries=" + applyTimeSeries + ", @username='" + username + "', @isCreateWorkflow=1, @applyBlankToNonBlank=" + applyBlankToNonBlank + ";", ConnectionConstants.SecMaster_Connection);
                
                //Changed By Dhruv for recentlyCreatedWorkflowInstanceID
                string response = "";

                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1)
                {
                    response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]) + "@^@" + Convert.ToString(ds.Tables[0].Rows[0][2]);
                }
                else
                {
                    response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]);
                }

                return response;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> SMSaveWorkflow");
            }
        }

        public static string RMSaveWorkflow(XDocument info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank, int workflowTypeId)
        {
            mLogger.Debug("WorkflowController : Start -> RMSaveWorkflow");
            try
            {
                mHList = new RHashlist();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPRefMaster].[dbo].[REFM_SaveUpdateWorkflow] @infoDoc='" + info + "', @workflowName='" + workflowName + "', @isCreate=" + isCreate + ", @applyTimeSeries=" + applyTimeSeries + ", @username='" + username + "', @isCreateWorkflow=1, @applyBlankToNonBlank=" + applyBlankToNonBlank + ", @workflowType = " + workflowTypeId + ";", ConnectionConstants.RefMaster_Connection);
                
                //Changed By Dhruv for recentlyCreatedWorkflowInstanceID
                string response = "";

                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 1)
                {
                    response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]) + "@^@" + Convert.ToString(ds.Tables[0].Rows[0][2]);
                }
                else
                {
                    response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]);
                }

                return response;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> RMSaveWorkflow");
            }
        }

        public static string SMUpdateWorkflow(XDocument info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            mLogger.Debug("WorkflowController : Start -> SMUpdateWorkflow");
            try
            {
                mHList = new RHashlist();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPSecMaster].[dbo].[SECM_SaveUpdateWorkflow] @infoDoc='" + info + "', @workflowName='" + workflowName + "', @isCreate=" + isCreate + ", @applyTimeSeries=" + applyTimeSeries + ", @username='" + username + "', @isCreateWorkflow=0, @instance_id=" + instanceId + ", @applyBlankToNonBlank = " + applyBlankToNonBlank + ";", ConnectionConstants.SecMaster_Connection);
                string response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]);
                return response;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> SMUpdateWorkflow");
            }
        }

        public static string RMUpdateWorkflow(XDocument info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank, int workflowTypeId)
        {
            mLogger.Debug("WorkflowController : Start -> RMUpdateWorkflow");
            try
            {
                mHList = new RHashlist();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPRefMaster].[dbo].[REFM_SaveUpdateWorkflow] @infoDoc='" + info + "', @workflowName='" + workflowName + "', @isCreate=" + isCreate + ", @applyTimeSeries=" + applyTimeSeries + ", @username='" + username + "', @isCreateWorkflow=0, @instance_id=" + instanceId + ", @applyBlankToNonBlank = " + applyBlankToNonBlank + ", @workflowType = " + workflowTypeId + ";", ConnectionConstants.RefMaster_Connection);
                string response = Convert.ToString(ds.Tables[0].Rows[0][0]) + "||" + Convert.ToString(ds.Tables[0].Rows[0][1]);
                return response;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> RMUpdateWorkflow");
            }
        }

        public static DataSet SMGetAllWorkflows()
        {
            mLogger.Debug("WorkflowController : Start -> SMGetAllWorkflows");
            try
            {
                mHList = new RHashlist();

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"EXEC [IVPSecMaster].[dbo].[SECM_GetAllWorkflows]", ConnectionConstants.SecMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> SMGetAllWorkflows");
            }
        }

        public static DataSet RMGetAllWorkflows()
        {
            mLogger.Debug("WorkflowController : Start -> RMGetAllWorkflows");
            try
            {
                mHList = new RHashlist();

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"EXEC [IVPRefMaster].[dbo].[REFM_GetAllWorkflows]", ConnectionConstants.RefMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("WorkflowController : End -> RMGetAllWorkflows");
            }
        }

        public static DataTable RMGetDownStreamSystems(int entityTypeID) //, int moduleId)
        {
            DataSet dsSystems = null;
            DataTable systems = null;

            if (entityTypeID > 0)
            {
                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportSystemManagementController");
                MethodInfo rmGetDownstreamSystem = objType.GetMethod("SelectAllReportSystemsForEntityType", BindingFlags.Static | BindingFlags.Public);
                dsSystems = (DataSet)rmGetDownstreamSystem.Invoke(null, new object[] { entityTypeID });

            }
            else
            {
                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportSystemManagementController");
                MethodInfo rmGetDownstreamSystem = objType.GetMethod("SelectAllReportSystems", BindingFlags.Static | BindingFlags.Public);
                //dsSystems = (DataSet)rmGetDownstreamSystem.Invoke(null, new object[] { moduleId });
                dsSystems = (DataSet)rmGetDownstreamSystem.Invoke(null, new object[] { 0 });
            }

            
            if (dsSystems != null && dsSystems.Tables.Count > 0)
                systems = dsSystems.Tables[0];

            return systems;
        }

        public static void RMPostToDownstream(List<int> systemIDs, List<string> entityCodes, string userName, bool isRealTimeExtract = false)
        {
            Assembly RefMControllerAssembly = Assembly.Load("RefMDownstream");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.downstream.RMDownstreamPost");
            MethodInfo rmGetDownstreamSystem = objType.GetMethod("PostEntities");

            object obj = Activator.CreateInstance(objType);

            if (!isRealTimeExtract)
                rmGetDownstreamSystem.Invoke(obj, new object[] { systemIDs, entityCodes, userName, false });
            else
                rmGetDownstreamSystem.Invoke(obj, new object[] { new List<int>(), entityCodes, userName, isRealTimeExtract });

        }

        public static DataSet GetAllSecTypeNames()
        {
            mLogger.Debug("CommonServiceController : Start -> GetAllSecTypeNames");
            try
            {
                mHList = new RHashlist();

                return (DataSet)CommonDALWrapper.ExecuteSelectQuery(@"SELECT sm.sectype_id, sm.sectype_name, sm.sectype_description FROM [IVPSecMaster].[dbo].[ivp_secm_sectype_master] sm(NOLOCK) WHERE sm.is_active = 'TRUE' ORDER BY sm.sectype_name",
                    ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAllSecTypeNames");
            }
        }

        public static DataSet GetAllEntityTypes()
        {
            mLogger.Debug("CommonServiceController : Start -> GetAllEntityTypes");
            try
            {
                mHList = new RHashlist();

                return (DataSet)CommonDALWrapper.ExecuteSelectQuery(@"SELECT rm.entity_type_id, rm.entity_display_name FROM [IVPRefMaster].[dbo].[ivp_refm_entity_type] rm(NOLOCK) WHERE rm.structure_type_id = 2 AND rm.is_active = 1 ORDER BY rm.entity_display_name",
                    ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAllEntityTypes");
            }
        }

        public static DataSet GetAttributeBasedOnSecTypeSelection(string secTypeIds, string userName)
        {
            mLogger.Debug("CommonServiceController : Start -> GetAttributeBasedOnSecTypeSelection");
            try
            {
                mHList = new RHashlist();

                string[] sectypeArr = secTypeIds.Split(',');
                DataSet ds = null;
                if (sectypeArr.Length > 1 || (sectypeArr.Length == 1 && sectypeArr[0] == "-1"))
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attr.attribute_id, attr.attribute_name, display_name FROM [ivpsecmaster].[dbo].[ivp_secm_attribute_details] AS attr (NOLOCK) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_sectype_table] AS tbl (NOLOCK)
                                                                ON tbl.sectype_table_id = attr.sectype_table_id 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_details] AS tem (NOLOCK) 
                                                                ON (attr.attribute_id = tem.attribute_id) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_master] AS mas (NOLOCK) 
                                                                ON (tem.template_id = mas.template_id AND mas.template_id = 0)
                                                                WHERE NOT EXISTS ( 
                                                                    SELECT DISTINCT attribute_id, sectype_id FROM [IVPSecMaster].[dbo].[ivp_secm_security_workflow_mapping] wm WHERE attr.attribute_id = wm.attribute_id AND tbl.sectype_id = wm.sectype_id
                                                                ) AND attr.is_active = 'TRUE' AND tem.is_active = 'TRUE' ORDER BY tem.display_name", ConnectionConstants.SecMaster_Connection);
                }
                else if (sectypeArr.Length == 1)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attr.attribute_id, attr.attribute_name, display_name FROM [ivpsecmaster].[dbo].[ivp_secm_attribute_details] AS attr (NOLOCK) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_sectype_table] AS tbl (NOLOCK)
                                                                ON tbl.sectype_table_id = attr.sectype_table_id 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_details] AS tem (NOLOCK) 
                                                                ON (attr.attribute_id = tem.attribute_id) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_master] AS mas (NOLOCK) 
                                                                ON (tem.template_id = mas.template_id AND mas.template_id = 0)
                                                                WHERE NOT EXISTS ( 
                                                                    SELECT DISTINCT attribute_id, sectype_id FROM [IVPSecMaster].[dbo].[ivp_secm_security_workflow_mapping] wm WHERE attr.attribute_id = wm.attribute_id AND tbl.sectype_id = wm.sectype_id
                                                                ) AND attr.is_active = 'TRUE' AND tem.is_active = 'TRUE'
                                                            UNION ALL
                                                            SELECT DISTINCT ad.attribute_id, ad.attribute_name, td.display_name FROM [IVPSecMaster].[dbo].[ivp_secm_attribute_details] ad
                                                            INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_table] st
                                                            ON st.sectype_table_id = ad.sectype_table_id
                                                            INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_details] td
                                                            ON td.attribute_id = ad.attribute_id 
                                                            INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_master] tm
                                                            ON tm.template_id = td.template_id
                                                            WHERE NOT EXISTS ( 
	                                                            SELECT DISTINCT attribute_id, sectype_id FROM [IVPSecMaster].[dbo].[ivp_secm_security_workflow_mapping] wm WHERE ad.attribute_id = wm.attribute_id AND st.sectype_id = wm.sectype_id
                                                            ) AND st.sectype_id IN ('" + secTypeIds + @"') AND st.[priority] > 0
                                                            ORDER BY display_name", ConnectionConstants.SecMaster_Connection);
                }
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAttributeBasedOnSecTypeSelection");
            }
        }

        public static DataSet GetAttributeBasedOnEntityTypeSelection(string entityTypeId)
        {
            mLogger.Debug("CommonServiceController : Start -> GetAttributeBasedOnEntityTypeSelection");
            try
            {
                mHList = new RHashlist();

                DataSet ds = null;

                ds = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @entityTypeID INT = " + entityTypeId + @";

                                                                SELECT EAT.entity_attribute_id, CASE WHEN ET.structure_type_id = 3 THEN ET.entity_display_name + ' - ' + EAT.display_name 
                                                                ELSE EAT.display_name END AS attribute_display_name
                                                                FROM IVPRefMaster.dbo.ivp_refm_entity_attribute EAT
                                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type ET 
                                                                ON EAT.entity_type_id = ET.entity_type_id
                                                                WHERE (ET.entity_type_id = @entityTypeID OR (ET.derived_from_entity_type_id = @entityTypeID AND ET.structure_type_id = 3) )
                                                                AND ET.is_active = 1 AND EAT.is_active = 1
                                                                ORDER BY ET.entity_type_id, attribute_display_name", ConnectionConstants.RefMaster_Connection);

                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAttributeBasedOnEntityTypeSelection");
            }
        }

        public static DataSet GetAllUsers()
        {
            mLogger.Debug("CommonServiceController : Start -> GetAllUsers");
            try
            {
                mHList = new RHashlist();

                DataSet ds = (DataSet)CommonDALWrapper.ExecuteSelectQuery("SELECT user_login_name, first_name, last_name FROM [IVPRAD].[dbo].[ivp_rad_user_master] WHERE is_active = 1", ConnectionConstants.RefMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAllUsers");
            }
        }

        public static DataSet GetAllGroups()
        {
            mLogger.Debug("CommonServiceController : Start -> GetAllGroups");
            try
            {
                mHList = new RHashlist();

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT group_name FROM [IVPRAD].[dbo].[ivp_rad_group_master] WHERE is_active = 1", ConnectionConstants.RefMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> GetAllGroups");
            }
        }

        public static DataSet RMGetWorkflowType()
        {
            mLogger.Debug("CommonServiceController : Start -> RMGetWorkflowType");
            try
            {
                mHList = new RHashlist();

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT workflow_type_id, workflow_type_name FROM IVPRefMaster.dbo.ivp_refm_workflow_type", ConnectionConstants.RefMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                mHList.Clear();
                if (mHList != null) mHList = null;
                mLogger.Debug("CommonServiceController : End -> RMGetWorkflowType");
            }
        }

        public static DataSet GetImpactedSecurities(List<int> statusIds, string userName, out string guid)
        {
            RDBConnectionManager connManager = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);
            try
            {
                DataSet dsImpactedSecurities = new DataSet();

                //dtPassedSecurities
                dsImpactedSecurities.Tables.Add();
                dsImpactedSecurities.Tables[0].Columns.Add("Security Id", typeof(string));
                dsImpactedSecurities.Tables[0].Columns.Add("Attribute Id", typeof(int));
                dsImpactedSecurities.Tables[0].Columns.Add("Sectype Id", typeof(int));
                dsImpactedSecurities.Tables[0].Columns.Add("Attribute/Leg Name", typeof(string));
                dsImpactedSecurities.Tables[0].Columns.Add("Old Value", typeof(string));
                dsImpactedSecurities.Tables[0].Columns.Add("New Value", typeof(string));
                dsImpactedSecurities.Tables[0].Columns.Add("Attribute Value", typeof(string));
                dsImpactedSecurities.Tables[0].Columns.Add("Alert", typeof(string));

                //dtFailedSecurities
                DataTable dt = dsImpactedSecurities.Tables[0].Clone();
                dt.TableName = "Table2";
                dsImpactedSecurities.Tables.Add(dt);
                dsImpactedSecurities.Tables[1].Columns["Alert"].ColumnName = "Validation Failed";

                dsImpactedSecurities.Tables.Add(new DataTable("dtBasketInfo"));
                dsImpactedSecurities.Tables[2].Columns.Add("basket_xml");

                DataSet dsAlreadyPassed = new DataSet();
                dsAlreadyPassed.Tables.Add(dsImpactedSecurities.Tables[0].Clone());

                Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkflowSecurityCreationUpdation");
                MethodInfo checkImpactedSecuritiesforNormalUpdate = objType.GetMethod("CheckImpactedSecuritiesforNormalUpdate", BindingFlags.Static | BindingFlags.Public);

                IEnumerable<DataRow> idr = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetFinalApprovedSecurities '{0}','{1}'", string.Join(",", statusIds), userName), connManager).Tables[0].AsEnumerable();

                Dictionary<string, Dictionary<bool, Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>>> userVssecTypeVsSecIdVsAttributeIdVsValue = new Dictionary<string, Dictionary<bool, Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>>>();
                Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>> normalUserVssecTypeVsSecIdVsAttributeIdVsValue = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>();
                Dictionary<string, Dictionary<int, Dictionary<DateTime, Dictionary<string, Dictionary<string, string>>>>> TSuserVssecTypeVsSecIdVsAttributeIdVsValue = new Dictionary<string, Dictionary<int, Dictionary<DateTime, Dictionary<string, Dictionary<string, string>>>>>();
                Dictionary<string, Dictionary<int, Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>>> OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue = new Dictionary<string, Dictionary<int, Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>>>();

                DateTime today = DateTime.Now.Date;

                foreach (DataRow dr in idr)
                {
                    bool isFinalApproval = Convert.ToBoolean(dr["is_final_approval"]);
                    int sectypeId = Convert.ToInt32(dr["sectype_id"]);
                    string secId = Convert.ToString(dr["sec_id"]);
                    string requestedBy = Convert.ToString(dr["requested_by"]);
                    string attributeId = Convert.ToString(dr["attribute_id"]);
                    string attributeValue = Convert.ToString(dr["attribute_value"]);
                    DateTime requestedOn = Convert.ToDateTime(dr["requested_on"]);
                    DateTime effectiveFromDate = (string.IsNullOrEmpty(Convert.ToString(dr["effective_from_date"]))) ? today : Convert.ToDateTime(dr["effective_from_date"]);
                    DateTime effectiveToDate = (string.IsNullOrEmpty(Convert.ToString(dr["effective_to_date"]))) ? today : Convert.ToDateTime(dr["effective_to_date"]);
                    int is_normal_update = Convert.ToInt32(dr["is_normal_update"]);
                    bool isCreate = false;
                    if (isFinalApproval)
                    {
                        if (is_normal_update == 1 || is_normal_update == 2)// UPDATE OR CREATE
                        {
                            if (is_normal_update == 2)
                                isCreate = true;
                            if ((today.Date - requestedOn.Date).Days == 0)
                            {
                                if (!normalUserVssecTypeVsSecIdVsAttributeIdVsValue.ContainsKey(requestedBy))
                                    normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy] = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();
                                if (!normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy].ContainsKey(sectypeId))
                                    normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId] = new Dictionary<string, Dictionary<string, string>>();
                                if (!normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId].ContainsKey(secId))
                                    normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][secId] = new Dictionary<string, string>();
                                if (!normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][secId].ContainsKey(attributeId))
                                    normalUserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][secId][attributeId] = attributeValue;
                            }
                            else
                            {
                                if (!TSuserVssecTypeVsSecIdVsAttributeIdVsValue.ContainsKey(requestedBy))
                                    TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy] = new Dictionary<int, Dictionary<DateTime, Dictionary<string, Dictionary<string, string>>>>();
                                if (!TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy].ContainsKey(sectypeId))
                                    TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId] = new Dictionary<DateTime, Dictionary<string, Dictionary<string, string>>>();
                                if (!TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId].ContainsKey(requestedOn))
                                    TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][requestedOn] = new Dictionary<string, Dictionary<string, string>>();
                                if (!TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][requestedOn].ContainsKey(secId))
                                    TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][requestedOn][secId] = new Dictionary<string, string>();
                                if (!TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][requestedOn][secId].ContainsKey(attributeId))
                                    TSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][requestedOn][secId][attributeId] = attributeValue;
                            }

                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue.ContainsKey(requestedBy))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy] = new Dictionary<bool, Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>>();
                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy].ContainsKey(isCreate))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate] = new Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>();
                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate].ContainsKey(requestedOn))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn] = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();
                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn].ContainsKey(sectypeId))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn][sectypeId] = new Dictionary<string, Dictionary<string, string>>();
                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn][sectypeId].ContainsKey(secId))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn][sectypeId][secId] = new Dictionary<string, string>();
                            if (!userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn][sectypeId][secId].ContainsKey(attributeId))
                                userVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][isCreate][requestedOn][sectypeId][secId][attributeId] = attributeValue;
                        }
                        else if (is_normal_update == 0)
                        {
                            if (!OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue.ContainsKey(requestedBy))
                                OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy] = new Dictionary<int, Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>>();
                            if (!OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy].ContainsKey(sectypeId))
                                OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId] = new Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>();

                            KeyValuePair<DateTime, DateTime> kvp = new KeyValuePair<DateTime, DateTime>(effectiveFromDate, effectiveToDate);
                            if (!OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId].ContainsKey(kvp))
                                OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][kvp] = new Dictionary<string, Dictionary<string, string>>();
                            if (!OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][kvp].ContainsKey(secId))
                                OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][kvp][secId] = new Dictionary<string, string>();
                            if (!OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][kvp][secId].ContainsKey(attributeId))
                                OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue[requestedBy][sectypeId][kvp][secId][attributeId] = attributeValue;
                        }
                    }
                }

                Dictionary<string, Dictionary<int, List<SMSecurityWorkflowAttributes>>> userVslist = new Dictionary<string, Dictionary<int, List<SMSecurityWorkflowAttributes>>>();
                foreach (KeyValuePair<string, Dictionary<bool, Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>>> userLevel in userVssecTypeVsSecIdVsAttributeIdVsValue)
                {
                    foreach (KeyValuePair<bool, Dictionary<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>>> createLevel in userLevel.Value)
                    {
                        foreach (KeyValuePair<DateTime, Dictionary<int, Dictionary<string, Dictionary<string, string>>>> requestedOnLevel in createLevel.Value)
                        {
                            SMWorkflowImpactedSecuritiesResponse objSMWorkflowImpactedSecuritiesResponse = (SMWorkflowImpactedSecuritiesResponse)checkImpactedSecuritiesforNormalUpdate.Invoke(null, new object[] { requestedOnLevel.Value, userLevel.Key, userName, requestedOnLevel.Key, createLevel.Key, connManager });
                            DataSet returned = objSMWorkflowImpactedSecuritiesResponse.DsImpactedSecurities;

                            if (!userVslist.ContainsKey(userLevel.Key))
                                userVslist[userLevel.Key] = new Dictionary<int, List<SMSecurityWorkflowAttributes>>();

                            foreach (KeyValuePair<int, List<SMSecurityWorkflowAttributes>> kvp in objSMWorkflowImpactedSecuritiesResponse.DictSectypeIdVsLstRequestAttributes)
                            {
                                if (!userVslist[userLevel.Key].ContainsKey(kvp.Key))
                                    userVslist[userLevel.Key][kvp.Key] = new List<SMSecurityWorkflowAttributes>();
                                userVslist[userLevel.Key][kvp.Key].AddRange(kvp.Value);
                            }

                            if (returned.Tables.Count > 0 && returned.Tables[0].TableName != "AlreadyPassed")
                            {
                                foreach (DataRow dr in returned.Tables[0].Rows)
                                {
                                    dsImpactedSecurities.Tables[0].Rows.Add(dr.ItemArray);
                                }
                            }
                            else
                            {
                                foreach (DataRow dr in returned.Tables[0].Rows)
                                {
                                    dsAlreadyPassed.Tables[0].Rows.Add(dr.ItemArray);
                                }
                            }

                            if (returned.Tables.Count > 1)
                            {
                                foreach (DataRow dr in returned.Tables[1].Rows)
                                {
                                    dsImpactedSecurities.Tables[1].Rows.Add(dr.ItemArray);
                                }
                            }

                            if (returned.Tables.Count > 2)
                            {
                                foreach (DataRow dr in returned.Tables[2].Rows)
                                {
                                    dsImpactedSecurities.Tables[2].Rows.Add(dr.ItemArray);
                                }
                            }
                        }
                    }
                }
                guid = Guid.NewGuid().ToString();
                lock (((ICollection)dictGuidVsCachedInfo).SyncRoot)
                {
                    dictGuidVsCachedInfo[guid] = new SMWorkflowCachedInfo { DsAlreadyPassedSecurities = dsAlreadyPassed, DsImpactedSecurities = dsImpactedSecurities, normalUserVssecTypeVsSecIdVsAttributeIdVsValue = normalUserVssecTypeVsSecIdVsAttributeIdVsValue, TSuserVssecTypeVsSecIdVsAttributeIdVsValue = TSuserVssecTypeVsSecIdVsAttributeIdVsValue, OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue = OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue };
                    dictGuidVsCachedInfo[guid].UserVsLstRequestAttributes = userVslist;
                }

                lock (((ICollection)dictGuidVsValidUpto).SyncRoot)
                {
                    dictGuidVsValidUpto[guid] = DateTime.Now.AddHours(1);
                }
                connManager.CommitTransaction();
                CommonDALWrapper.PutConnectionManager(connManager);
                return dsImpactedSecurities;
            }
            catch (Exception ee)
            {
                connManager.RollbackTransaction();
                CommonDALWrapper.PutConnectionManager(connManager);
                throw ee;
            }
        }

        public static WorkflowHandlerResponseInfo WorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int actionType, string guid)
        {
            if (statusIds.Count > 0)
            {
                HashSet<string> updatedSecIds = new HashSet<string>();
                RDBConnectionManager connManager = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

                WorkflowHandlerResponseInfo result = new WorkflowHandlerResponseInfo();
                mLogger.Debug("WorkflowRequestHandler -> Start");
                result.FailureMessage = string.Empty;
                WorkflowActionEnum action = (WorkflowActionEnum)Enum.Parse(typeof(WorkflowActionEnum), actionType.ToString());

                try
                {
                    //Process Updates first
                    lock (((ICollection)dictGuidVsCachedInfo).SyncRoot)
                    {
                        if (dictGuidVsCachedInfo.ContainsKey(guid))
                        {
                            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                            Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkflowSecurityCreationUpdation");
                            MethodInfo updateImpactedSecurities = objType.GetMethod("UpdateImpactedSecurities", BindingFlags.Static | BindingFlags.Public);
                            updatedSecIds.UnionWith((HashSet<string>)updateImpactedSecurities.Invoke(null, new object[] { dictGuidVsCachedInfo[guid], userName, connManager }));

                            MethodInfo timeSeriesUpdate = objType.GetMethod("TimeSeriesUpdate", BindingFlags.Static | BindingFlags.Public);
                            foreach (KeyValuePair<string, Dictionary<int, Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>>> userLevel in dictGuidVsCachedInfo[guid].OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue)
                            {
                                updatedSecIds.UnionWith((HashSet<string>)timeSeriesUpdate.Invoke(null, new object[] { userLevel.Value, userLevel.Key, true, userName, connManager }));
                            }

                            objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMSecurityController");
                            MethodInfo raiseRequestInWorkflow = objType.GetMethod("RaiseRequestInWorkflow");
                            foreach (KeyValuePair<string, Dictionary<int, List<SMSecurityWorkflowAttributes>>> userLevel in dictGuidVsCachedInfo[guid].UserVsLstRequestAttributes)
                            {
                                foreach (KeyValuePair<int, List<SMSecurityWorkflowAttributes>> sectypeLevel in userLevel.Value)
                                {
                                    var objSMSecurityController = Activator.CreateInstance(objType);
                                    raiseRequestInWorkflow.Invoke(objSMSecurityController, new object[] { userName, sectypeLevel.Value, false, connManager });
                                }
                            }
                        }
                    }

                    Dictionary<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>> dictMail = new Dictionary<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>>();
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format("EXEC IVPSecMaster.dbo.SECM_WorkflowActionHandler '{0}','{1}','{2}','{3}'", string.Join(",", statusIds), userName, remarks, action.ToString()), connManager);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        bool status = Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
                        if (status)
                        {
                            if (ds.Tables.Count > 1)
                            {
                                Dictionary<int, DataRow> statusIdVsRow = ds.Tables[1].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["status_id"]));
                                Dictionary<int, Dictionary<string, string>> userVsMailId = new Dictionary<int, Dictionary<string, string>>();
                                Dictionary<int, Dictionary<string, Dictionary<string, string>>> groupNameVsUserNameVsMailId = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();

                                if (ds.Tables.Count == 4)
                                {
                                    userVsMailId = ds.Tables[2].AsEnumerable().GroupBy(q => Convert.ToInt32(q["status_id"])).ToDictionary(x => x.Key, y => y.ToDictionary(k => Convert.ToString(k["username"]), t => Convert.ToString(t["mail_id"])));
                                    groupNameVsUserNameVsMailId = ds.Tables[3].AsEnumerable().GroupBy(q => Convert.ToInt32(q["status_id"])).ToDictionary(x => x.Key, y => y.GroupBy(r => Convert.ToString(r["group_name"])).ToDictionary(k => k.Key, t => t.ToDictionary(w => Convert.ToString(w["username"]), v => Convert.ToString(v["mail_id"]))));
                                }

                                foreach (int statusId in statusIds)
                                {
                                    if (statusIdVsRow.ContainsKey(statusId))
                                    {
                                        DataRow masterRow = statusIdVsRow[statusId];
                                        bool isFinalApproval = Convert.ToBoolean(masterRow["is_final_approval"]);
                                        string requestedBy = Convert.ToString(masterRow["requested_by"]);
                                        DateTime requestedOn = Convert.ToDateTime(masterRow["requested_on"]);
                                        int queueId = Convert.ToInt32(masterRow["queue_id"]);
                                        bool isLastRejection = Convert.ToBoolean(masterRow["is_final_rejection"]);

                                        WorkflowMailInfo mailInfo = new WorkflowMailInfo();
                                        mailInfo.ActionBy = Convert.ToString(masterRow["username"]);
                                        mailInfo.ActionTime = Convert.ToDateTime(masterRow["action_time"]);
                                        mailInfo.Comments = Convert.ToString(masterRow["remarks"]);
                                        mailInfo.RequestedBy = requestedBy;
                                        mailInfo.RequestedByMailId = Convert.ToString(masterRow["requester_mail_id"]);
                                        mailInfo.RequestedTime = requestedOn;
                                        mailInfo.Uri = string.Empty;
                                        mailInfo.UserNameVsMailId = userVsMailId.ContainsKey(statusId) ? userVsMailId[statusId] : new Dictionary<string, string>();
                                        mailInfo.GroupNameVsUserNameVsMailId = groupNameVsUserNameVsMailId.ContainsKey(statusId) ? groupNameVsUserNameVsMailId[statusId] : new Dictionary<string, Dictionary<string, string>>();
                                        mailInfo.Action = (action == WorkflowActionEnum.APPROVED && !isFinalApproval) ? WorkflowActionEnum.INTERMEDIATE_REQUEST : action;

                                        if (isLastRejection)
                                            mailInfo.Action = WorkflowActionEnum.REJECTEDTOLAST;

                                        mailInfo.UpdateType = Convert.ToString(masterRow["update_type"]);

                                        string key = Convert.ToInt32(masterRow["workflow_instance_id"]) + 'ž' + requestedBy + 'ž' + requestedOn;
                                        if (!dictMail.ContainsKey(key))
                                            dictMail[key] = new KeyValuePair<WorkflowMailInfo, HashSet<int>>(mailInfo, new HashSet<int> { queueId });
                                        else
                                            dictMail[key].Value.Add(queueId);
                                    }
                                }

                                foreach (KeyValuePair<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>> kvp in dictMail)
                                {
                                    Assembly SecMasterSubscriptionAssembly = Assembly.Load("SecMasterSubscription");
                                    Type objType = SecMasterSubscriptionAssembly.GetType("com.ivp.secm.subscription.SMAttributeWorkflowSubscription");
                                    MethodInfo formatWorkflowMail = objType.GetMethod("FormatWorkflowMail", BindingFlags.Static | BindingFlags.Public);
                                    formatWorkflowMail.Invoke(null, new object[] { kvp.Value.Key, kvp.Value.Value.ToList(), connManager });

                                }

                                Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                                Type objTypee = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkflowSecurityCreationUpdation");
                                MethodInfo generateRealTimeExtracts = objTypee.GetMethod("GenerateRealTimeExtracts", BindingFlags.Static | BindingFlags.Public);
                                generateRealTimeExtracts.Invoke(null, new object[] { updatedSecIds.ToList() });

                                result.IsSuccess = true;
                            }
                        }
                        else
                        {
                            result.FailureMessage = Convert.ToString(ds.Tables[0].Rows[0][1]);
                            result.IsSuccess = false;
                        }
                    }
                    else
                        result.IsSuccess = false;
                }
                catch (Exception ex)
                {
                    mLogger.Error("WorkflowRequestHandler -> Exception ->" + ex.ToString());
                    result.IsSuccess = false;
                    result.FailureMessage = ex.ToString();
                }
                finally
                {
                    if (result.IsSuccess)
                        connManager.CommitTransaction();
                    else
                        connManager.RollbackTransaction();
                    CommonDALWrapper.PutConnectionManager(connManager);
                }
                mLogger.Debug("WorkflowRequestHandler -> End");
                return result;
            }
            else
                throw new Exception("No Status Id to process");
        }

        public static Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>> RMWorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int actionType, RDBConnectionManager connManager = null, List<RMProcessedEntityInfo> rmProcessedInfo = null, bool skipValidations = false)
        {
            WorkflowHandlerResponseInfo result = new WorkflowHandlerResponseInfo();
            List<RMFinalApprovalEntity> responseInfo = null;
            Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>> finalDictionary = new Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>>();

            if (statusIds.Count > 0)
            {
                WorkflowActionEnum action = (WorkflowActionEnum)Enum.Parse(typeof(WorkflowActionEnum), actionType.ToString());
                bool existingConnection = true;

                HashSet<string> updatedSecIds = new HashSet<string>();
                if (connManager == null)
                {
                    existingConnection = false;
                    connManager = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.Serializable);
                }

                mLogger.Debug("RMWorkflowRequestHandler -> Start");
                result.FailureMessage = string.Empty;
                result.IsSuccess = true;

                try
                {
                    Dictionary<string, DateTime> entityCodeVsEffectiveDate = new Dictionary<string, DateTime>();
                    Dictionary<string, string> entityCodeVsRequester = new Dictionary<string, string>();

                    Dictionary<int, string> approvalLegInfo = new Dictionary<int, string>();

                    Dictionary<int, DataTable> finalApprovalLegInfo = new Dictionary<int, DataTable>();

                    DateTime actionTime = DateTime.Now;

                    Dictionary<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>> dictMail = new Dictionary<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>>();
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format("EXEC IVPRefMaster.dbo.REFM_WorkflowActionHandler '{0}','{1}','{2}','{3}'", string.Join(",", statusIds), userName, remarks, action.ToString()), connManager);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        bool status = Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
                        if (status)
                        {
                            if (ds.Tables.Count > 1)
                            {
                                Dictionary<int, DataRow> statusIdVsRow = ds.Tables[1].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["status_id"]));
                                Dictionary<int, Dictionary<string, string>> userVsMailId = new Dictionary<int, Dictionary<string, string>>();
                                Dictionary<int, Dictionary<string, Dictionary<string, string>>> groupNameVsUserNameVsMailId = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();

                                entityCodeVsEffectiveDate = ds.Tables[1].AsEnumerable().Where(x => Convert.ToBoolean(x["is_final_approval"]) == true && Convert.ToString(x["workflow_type"]).Equals("Entity")).ToDictionary(x => Convert.ToString(x["entity_code"]), y => Convert.ToDateTime(y["effective_from_date"]));
                                entityCodeVsRequester = ds.Tables[1].AsEnumerable().Where(x => Convert.ToBoolean(x["is_final_approval"]) == true && Convert.ToString(x["workflow_type"]).Equals("Entity")).ToDictionary(x => Convert.ToString(x["entity_code"]), y => Convert.ToString(y["requested_by"]));

                                approvalLegInfo = ds.Tables[1].AsEnumerable().Where(x => Convert.ToBoolean(x["is_final_approval"]) == true && Convert.ToString(x["workflow_type"]).Equals("Leg")).ToDictionary(x => Convert.ToInt32(x["leg_entity_type_id"]), y => Convert.ToString(y["attribute_value"]));

                                if (statusIdVsRow != null && statusIdVsRow.Count > 0 && rmProcessedInfo != null)
                                {
                                    statusIdVsRow.Keys.ToList<int>().ForEach(sid =>
                                    {
                                        RMProcessedEntityInfo rmpInfo = new RMProcessedEntityInfo();
                                        rmpInfo.StatusID = sid;
                                        rmpInfo.EntityCode = Convert.ToString(statusIdVsRow[sid]["entity_code"]);
                                        rmpInfo.isProcessed = false;

                                        rmProcessedInfo.Add(rmpInfo);
                                    });

                                }

                                if (approvalLegInfo.Count > 0)
                                {
                                    foreach (int id in approvalLegInfo.Keys)
                                    {
                                        DataTable dt = (DataTable)(JsonConvert.DeserializeObject(approvalLegInfo[id], typeof(DataTable)));
                                        finalApprovalLegInfo.Add(id, dt);
                                    }
                                }

                                if (ds.Tables.Count == 4)
                                {
                                    userVsMailId = ds.Tables[2].AsEnumerable().GroupBy(q => Convert.ToInt32(q["status_id"])).ToDictionary(x => x.Key, y => y.ToDictionary(k => Convert.ToString(k["username"]), t => Convert.ToString(t["mail_id"])));
                                    groupNameVsUserNameVsMailId = ds.Tables[3].AsEnumerable().GroupBy(q => Convert.ToInt32(q["status_id"])).ToDictionary(x => x.Key, y => y.GroupBy(r => Convert.ToString(r["group_name"])).ToDictionary(k => k.Key, t => t.ToDictionary(w => Convert.ToString(w["username"]), v => Convert.ToString(v["mail_id"]))));
                                }

                                foreach (int statusId in statusIds)
                                {
                                    if (statusIdVsRow.ContainsKey(statusId))
                                    {
                                        DataRow masterRow = statusIdVsRow[statusId];

                                        actionTime = Convert.ToDateTime(masterRow["action_time"]);

                                        bool isFinalApproval = Convert.ToBoolean(masterRow["is_final_approval"]);
                                        string requestedBy = Convert.ToString(masterRow["requested_by"]);
                                        DateTime requestedOn = Convert.ToDateTime(masterRow["requested_on"]);
                                        int queueId = Convert.ToInt32(masterRow["queue_id"]);
                                        bool isLastRejection = Convert.ToBoolean(masterRow["is_final_rejection"]);

                                        WorkflowMailInfo mailInfo = new WorkflowMailInfo();
                                        mailInfo.ActionBy = Convert.ToString(masterRow["username"]);
                                        mailInfo.ActionTime = Convert.ToDateTime(masterRow["action_time"]);
                                        mailInfo.Comments = Convert.ToString(masterRow["remarks"]);
                                        mailInfo.RequestedBy = requestedBy;
                                        mailInfo.RequestedByMailId = Convert.ToString(masterRow["requester_mail_id"]);
                                        mailInfo.RequestedTime = requestedOn;
                                        mailInfo.Uri = string.Empty;
                                        mailInfo.UserNameVsMailId = userVsMailId.ContainsKey(statusId) ? userVsMailId[statusId] : new Dictionary<string, string>();
                                        mailInfo.GroupNameVsUserNameVsMailId = groupNameVsUserNameVsMailId.ContainsKey(statusId) ? groupNameVsUserNameVsMailId[statusId] : new Dictionary<string, Dictionary<string, string>>();
                                        mailInfo.Action = (action == WorkflowActionEnum.APPROVED && !isFinalApproval) ? WorkflowActionEnum.INTERMEDIATE_REQUEST : action;

                                        if (isLastRejection)
                                            mailInfo.Action = WorkflowActionEnum.REJECTEDTOLAST;

                                        mailInfo.UpdateType = Convert.ToString(masterRow["update_type"]);

                                        string key = Convert.ToInt32(masterRow["workflow_instance_id"]) + 'ž' + requestedBy + 'ž' + requestedOn;
                                        if (!dictMail.ContainsKey(key))
                                            dictMail[key] = new KeyValuePair<WorkflowMailInfo, HashSet<int>>(mailInfo, new HashSet<int> { queueId });
                                        else
                                            dictMail[key].Value.Add(queueId);
                                    }
                                }

                                //Final Approval
                                if (entityCodeVsEffectiveDate.Keys.Count > 0)
                                {
                                    string failMessage = string.Empty;
                                    Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                    Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowController");
                                    MethodInfo saveApprovedEntities = objType.GetMethod("SaveApprovedEntities", BindingFlags.Static | BindingFlags.Public);
                                    responseInfo = (List<RMFinalApprovalEntity>)saveApprovedEntities.Invoke(null, new object[] { entityCodeVsEffectiveDate, actionTime, userName, connManager, skipValidations, entityCodeVsRequester });

                                    failMessage = string.Join(",", responseInfo.AsEnumerable().Where(x => x.IsSuccess == false).Select(x => string.Join(",", x.FailureReasons)).ToList());

                                    if (!string.IsNullOrEmpty(failMessage))
                                    {
                                        responseInfo.Where(r => !r.IsSuccess).ToList().ForEach(resp =>
                                        {
                                            result.FailureMessage = result.FailureMessage + (string.IsNullOrEmpty(result.FailureMessage) ? string.Empty : Environment.NewLine) + resp.EntityCode + ": " + resp.FailureReasons[0];
                                        });
                                    }

                                    //result.FailureMessage = string.Join(",", responseInfo.AsEnumerable().Where(x => x.IsSuccess == false).Select(x => string.Join(",", x.FailureReasons)).ToList());
                                    if (!string.IsNullOrEmpty(result.FailureMessage))
                                        result.IsSuccess = false;
                                }

                                if (finalApprovalLegInfo.Keys.Count > 0)
                                {
                                    Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                    Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowController");
                                    MethodInfo saveApprovedEntities = objType.GetMethod("ApproveLegsFinally", BindingFlags.Static | BindingFlags.Public);
                                    List<string> legApprovalResponse = (List<string>)saveApprovedEntities.Invoke(null, new object[] { finalApprovalLegInfo, userName, actionTime, false, connManager });
                                    if (legApprovalResponse != null && legApprovalResponse.Any(x => !string.IsNullOrEmpty(x)))
                                    {
                                        result.FailureMessage = string.Join(",", legApprovalResponse.Distinct().ToList());
                                        result.IsSuccess = false;
                                    }
                                }

                                if (result.IsSuccess)
                                {
                                    List<string> finalSecIds = new List<string>();
                                    entityCodeVsEffectiveDate.Keys.ToList<string>().ForEach(ec =>
                                    {
                                        if (!finalSecIds.Contains(ec))
                                            finalSecIds.Add(ec);
                                    });

                                    if (finalSecIds != null && finalSecIds.Count > 0)
                                        result.secIds = finalSecIds;

                                    foreach (KeyValuePair<string, KeyValuePair<WorkflowMailInfo, HashSet<int>>> kvp in dictMail)
                                    {
                                        Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                        Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowController");
                                        MethodInfo formatWorkflowMail = objType.GetMethod("FormatWorkflowMail", BindingFlags.Static | BindingFlags.Public);
                                        formatWorkflowMail.Invoke(null, new object[] { kvp.Value.Key, kvp.Value.Value.ToList(), connManager });
                                    }
                                }

                                //Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                                //Type objTypee = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkflowSecurityCreationUpdation");
                                //MethodInfo generateRealTimeExtracts = objTypee.GetMethod("GenerateRealTimeExtracts", BindingFlags.Static | BindingFlags.Public);
                                //generateRealTimeExtracts.Invoke(null, new object[] { updatedSecIds.ToList() });


                            }
                        }
                        else
                        {
                            result.FailureMessage = Convert.ToString(ds.Tables[0].Rows[0][1]);
                            result.IsSuccess = false;
                        }
                    }
                    else
                        result.IsSuccess = false;

                }
                catch (Exception ex)
                {
                    mLogger.Error("RMWorkflowRequestHandler -> Exception ->" + ex.ToString());
                    result.IsSuccess = false;
                    result.FailureMessage = ex.ToString();
                }
                finally
                {
                    if (result.IsSuccess && !existingConnection)
                        connManager.CommitTransaction();
                    else if (!existingConnection)
                        connManager.RollbackTransaction();

                    if (!existingConnection)
                        CommonDALWrapper.PutConnectionManager(connManager);
                }
                mLogger.Debug("RMWorkflowRequestHandler -> End");

                finalDictionary.Add(result, responseInfo);

                return finalDictionary;
            }
            else
                throw new Exception("No Status Id to process");
        }

        public static bool RemoveKey(string guid, bool removeValidity = true)
        {
            try
            {
                if (removeValidity)
                {
                    lock (((ICollection)dictGuidVsValidUpto).SyncRoot)
                    {
                        if (dictGuidVsValidUpto.ContainsKey(guid))
                        {
                            dictGuidVsValidUpto.Remove(guid);
                        }
                    }
                }

                lock (((ICollection)dictGuidVsCachedInfo).SyncRoot)
                {
                    if (dictGuidVsCachedInfo.ContainsKey(guid))
                    {
                        dictGuidVsCachedInfo.Remove(guid);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void PerformRefDataMassage(ref DataSet ds, string date)
        {
            mLogger.Debug("Start => PerformRefDataMassage");

            try
            {
                DataSet refData = new DataSet();
                List<RMAttributeValueInfoOptimized> attributeValueInfoList = new List<RMAttributeValueInfoOptimized>();
                var refAttrDetails = ds.Tables[ds.Tables.Count - 1].AsEnumerable().ToDictionary(x => Convert.ToString(x["sectype_table_id"]) + "|" + x.Field<string>("display_name"), y => new KeyValuePair<string, int>(y.Field<string>("reference_attribute_name"), y.Field<int>("reference_type_id")));

                string SECTYPE_TABLE_ID = "sectype_table_id";
                string ATTRIBUTE_NAME = "Attribute";
                string ATTRIBUTE_VALUE = "Attribute Value";
                string ATTRIBUTE_VALUE_OLD = "Old Attribute Value";

                for (int tabIndex = 0; tabIndex < ds.Tables.Count - 1; tabIndex++)
                {
                    if (ds.Tables[tabIndex].Rows.Count > 0)
                    {
                        for (int row = 0; row < ds.Tables[tabIndex].Rows.Count; row++)
                        {
                            var displayName = Convert.ToString(ds.Tables[tabIndex].Rows[row][SECTYPE_TABLE_ID]) + "|" + Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_NAME]);
                            if (refAttrDetails.ContainsKey(displayName))
                            {
                                string refValue = Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE]);
                                string refValueOld = Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE_OLD]);
                                if (!string.IsNullOrEmpty(refValue) || !string.IsNullOrEmpty(refValueOld))
                                {
                                    var info = attributeValueInfoList.FirstOrDefault(x => x.EntityTypeId == refAttrDetails[displayName].Value);
                                    if (info == null)
                                    {
                                        info = new RMAttributeValueInfoOptimized();
                                        info.EffectiveDate = date;
                                        info.EntityTypeId = refAttrDetails[displayName].Value;
                                        info.KnowledgeDate = date;
                                        info.AttributeList = new HashSet<string>();
                                        info.EntityCodeList = new HashSet<string>();
                                        attributeValueInfoList.Add(info);
                                    }

                                    info.AttributeList.Add(refAttrDetails[displayName].Key);
                                    if (!string.IsNullOrEmpty(refValue))
                                        info.EntityCodeList.Add(refValue);
                                    if (!string.IsNullOrEmpty(refValueOld))
                                        info.EntityCodeList.Add(refValueOld);
                                }
                            }
                        }
                    }
                }

                if (attributeValueInfoList.Any())
                {
                    refData = new RMRefMasterAPI().GetMasssagedDataOnKnowledgeDate_Curve_Optimized(attributeValueInfoList,true);

                    if (refData.Tables.Count > 0)
                    {
                        for (int tabIndex = 0; tabIndex < refData.Tables.Count; tabIndex++)
                        {
                            refData.Tables[tabIndex].PrimaryKey = new DataColumn[] { refData.Tables[tabIndex].Columns["entityCodes"] };
                        }

                        for (int tabIndex = 0; tabIndex < ds.Tables.Count - 1; tabIndex++)
                        {
                            for (int row = 0; row < ds.Tables[tabIndex].Rows.Count; row++)
                            {
                                var displayName = Convert.ToString(ds.Tables[tabIndex].Rows[row][SECTYPE_TABLE_ID]) + "|" + Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_NAME]);
                                if (refAttrDetails.ContainsKey(displayName))
                                {
                                    string refValue = Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE]);
                                    string refValueOld = Convert.ToString(ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE_OLD]);
                                    if (!string.IsNullOrEmpty(refValue) || !string.IsNullOrEmpty(refValueOld))
                                    {
                                        if (refData.Tables.Contains(refAttrDetails[displayName].Value.ToString()))
                                        {
                                            if (!string.IsNullOrEmpty(refValue))
                                            {
                                                DataRow refRow = refData.Tables[refAttrDetails[displayName].Value.ToString()].Rows.Find(refValue);
                                                if (refRow != null)
                                                {
                                                    ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE] = Convert.ToString(refRow[refAttrDetails[displayName].Key]);
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(refValueOld))
                                            {
                                                DataRow refRow = refData.Tables[refAttrDetails[displayName].Value.ToString()].Rows.Find(refValueOld);
                                                if (refRow != null)
                                                {
                                                    ds.Tables[tabIndex].Rows[row][ATTRIBUTE_VALUE_OLD] = Convert.ToString(refRow[refAttrDetails[displayName].Key]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw ex;
            }
            mLogger.Debug("End => PerformRefDataMassage");
        }

        public static DataSet GetEntityTypeLegs(int entityTypeId, string username = null)
        {
            try
            {
                mLogger.Debug("WorkflowController : Start -> GetEntityTypeLegs");

                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityTypeController");
                MethodInfo formatWorkflowMail = objType.GetMethod("GetMultiInfoLegs", BindingFlags.Static | BindingFlags.Public);
                return (DataSet)formatWorkflowMail.Invoke(null, new object[] { entityTypeId, username, null });
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("WorkflowController : End -> GetEntityTypeLegs");
            }
        }

        public static DataSet GetAllWorkflowsConfiguration()
        {
            try
            {
                mLogger.Debug("WorkflowController::GetAllWorkflowsConfiguration - START");
                DataSet refConfigDS = null;
                DataSet secConfigDS = null;
                DataSet configDS = null;
                string productName = RConfigReader.GetConfigAppSettings("ProductName");

                if (productName.Equals("secmaster", StringComparison.OrdinalIgnoreCase))
                {
                    refConfigDS = CommonDALWrapper.ExecuteSelectQuery(@"EXEC IVPRefMaster.dbo.SRM_FetchCompleteWorkflowConfiguration", ConnectionConstants.RefMaster_Connection);
                    secConfigDS = CommonDALWrapper.ExecuteSelectQuery(@"EXEC IVPSecMaster.dbo.SRM_FetchCompleteWorkflowConfiguration", ConnectionConstants.SecMaster_Connection);
                }
                else if (productName.Equals("refmaster", StringComparison.OrdinalIgnoreCase))
                    refConfigDS = CommonDALWrapper.ExecuteSelectQuery(@"EXEC IVPRefMaster.dbo.SRM_FetchCompleteWorkflowConfiguration", ConnectionConstants.RefMaster_Connection);

                secConfigDS = RenameWorkflowConfigurationTables(secConfigDS);
                refConfigDS = RenameWorkflowConfigurationTables(refConfigDS);

                configDS = MergeWorkflowConfigurationTables(secConfigDS, refConfigDS);

                return configDS;
            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController::GetAllWorkflowsConfiguration - ERROR - " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("WorkflowController::GetAllWorkflowsConfiguration - END");
            }
            return null;
        }

        private static DataSet MergeWorkflowConfigurationTables(DataSet secConfigDS, DataSet refConfigDS)
        {
            try
            {
                mLogger.Debug("WorkflowController::MergeWorkflowConfigurationTables - START");

                if (secConfigDS != null && refConfigDS == null)
                    return secConfigDS;
                if (secConfigDS == null && refConfigDS != null)
                    return refConfigDS;

                DataSet mergedDS = refConfigDS;

                foreach (DataTable dt in secConfigDS.Tables)
                {
                    if (mergedDS.Tables.Contains(dt.TableName))
                        mergedDS.Tables[dt.TableName].Merge(dt);
                    else
                        mergedDS.Tables.Add(dt.Copy());
                }

                return mergedDS;
            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController::MergeWorkflowConfigurationTables - ERROR - " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("WorkflowController::MergeWorkflowConfigurationTables - END");
            }
            return null;
        }

        private static DataSet RenameWorkflowConfigurationTables(DataSet configDS)
        {
            try
            {
                mLogger.Debug("WorkflowController::RenameWorkflowConfigurationTables - START");
                if (configDS != null && configDS.Tables.Count > 0)
                {
                    foreach (DataRow dr in configDS.Tables[0].Rows)
                    {
                        int index = dr.Field<int>("id");
                        string configName = dr.Field<string>("configName");
                        configDS.Tables[index].TableName = configName;
                    }

                    configDS.Tables.RemoveAt(0);
                }
                return configDS;
            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController::RenameWorkflowConfigurationTables- ERROR - " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("WorkflowController::RenameWorkflowConfigurationTables - END");
            }
            return null;
        }
    }

}
