using com.ivp.rad.common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace com.ivp.srm.common
{
    public class ValidationParameterInspector : IDispatchMessageInspector
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("ValidationParameterInspector");

        static bool IsPenTestingBuild = false;
        static bool EnablePenTestingBuildRegex = false;
        static bool RequireSqlInjectionHandling = false;
        static bool RestrictedSVCsInSqlInjectionHandling = false;
        static bool EnableParameterValidationLogs = false;

        static HashSet<string> AllowedSVCs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        static HashSet<string> BlockedInRestrictedAccessSVCs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static ValidationParameterInspector()
        {
            string IsPenTestingBuildString = ConfigurationManager.AppSettings["IsPenTestingBuild"];

            if (!string.IsNullOrEmpty(IsPenTestingBuildString) && IsPenTestingBuildString.Equals("true", StringComparison.OrdinalIgnoreCase))
                IsPenTestingBuild = true;

            string EnablePenTestingBuildRegexString = ConfigurationManager.AppSettings["EnablePenTestingBuildRegex"];

            if (!string.IsNullOrEmpty(EnablePenTestingBuildRegexString) && EnablePenTestingBuildRegexString.Equals("true", StringComparison.OrdinalIgnoreCase))
                EnablePenTestingBuildRegex = true;

            string RequireSqlInjectionHandlingString = ConfigurationManager.AppSettings["RequireSqlInjectionHandling"];

            if (!string.IsNullOrEmpty(RequireSqlInjectionHandlingString) && RequireSqlInjectionHandlingString.Equals("true", StringComparison.OrdinalIgnoreCase))
                RequireSqlInjectionHandling = true;

            string RestrictedSVCsInSqlInjectionHandlingString = ConfigurationManager.AppSettings["RestrictedSVCsInSqlInjectionHandling"];

            if (!string.IsNullOrEmpty(RestrictedSVCsInSqlInjectionHandlingString) && RestrictedSVCsInSqlInjectionHandlingString.Equals("true", StringComparison.OrdinalIgnoreCase))
                RestrictedSVCsInSqlInjectionHandling = true;

            string EnableParameterValidationLogsString = ConfigurationManager.AppSettings["EnableParameterValidationLogs"];

            if (!string.IsNullOrEmpty(EnableParameterValidationLogsString) && EnableParameterValidationLogsString.Equals("true", StringComparison.OrdinalIgnoreCase))
                EnableParameterValidationLogs = true;

            AllowedSVCs.Add("AttributeSetupPage.asmx");
            AllowedSVCs.Add("CommonService.asmx");
            AllowedSVCs.Add("CommonService.svc");
            AllowedSVCs.Add("CTMServices.asmx");
            AllowedSVCs.Add("DeDuplicationService.svc");
            AllowedSVCs.Add("ExceptionManagerService.svc");
            AllowedSVCs.Add("RMDashboardService.svc");
            AllowedSVCs.Add("RMRefMasterAPI.svc");
            AllowedSVCs.Add("RMRefMasterUIAPI.svc");
            AllowedSVCs.Add("RMService.asmx");
            AllowedSVCs.Add("SecMasterUIService.svc");
            AllowedSVCs.Add("Service.asmx");
            AllowedSVCs.Add("SMAttributeSetup.svc");
            AllowedSVCs.Add("SMBasicService.svc");
            AllowedSVCs.Add("SMBulkUploadDocumentsService.svc");
            AllowedSVCs.Add("SMCreateUpdateNew.svc");
            AllowedSVCs.Add("SMCreateUpdateNew1.svc");
            AllowedSVCs.Add("SMCreateUpdateSecurityNew.svc");
            AllowedSVCs.Add("SMDashboardService.svc");
            AllowedSVCs.Add("SMExceptionManagement.svc");
            AllowedSVCs.Add("SMExceptionManager.svc");
            AllowedSVCs.Add("SMMigrationService.asmx");
            AllowedSVCs.Add("SMMigrationUtilityManagerService.svc");
            AllowedSVCs.Add("SMPushAPI.svc");
            AllowedSVCs.Add("SMRealTimePrefSetup.svc");
            AllowedSVCs.Add("SMRealTimeSecManagement.svc");
            AllowedSVCs.Add("SMRefAttributeAddition.svc");
            AllowedSVCs.Add("SMVendorDataForSectypes.svc");
            AllowedSVCs.Add("SRMVendorAPIControl.svc");
            AllowedSVCs.Add("TaskStatusWebMethods.asmx");
            AllowedSVCs.Add("ThirdPartyUpdateService.asmx");
            AllowedSVCs.Add("WorkflowService.asmx");

            BlockedInRestrictedAccessSVCs.Add("ExceptionManagerService.svc");
            BlockedInRestrictedAccessSVCs.Add("RMRefMasterAPI.svc");
            BlockedInRestrictedAccessSVCs.Add("RMRefMasterUIAPI.svc");
            BlockedInRestrictedAccessSVCs.Add("RMService.asmx");
            BlockedInRestrictedAccessSVCs.Add("SecMasterUIService.svc");
            BlockedInRestrictedAccessSVCs.Add("Service.asmx");
            BlockedInRestrictedAccessSVCs.Add("SMCreateUpdateSecurityNew.svc");

            string AddSVCsInAllowedForSQLInjection = ConfigurationManager.AppSettings["AddSVCsInAllowedForSQLInjection"];

            if (!string.IsNullOrEmpty(AddSVCsInAllowedForSQLInjection))
            {
                foreach (var SVCName in AddSVCsInAllowedForSQLInjection.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
                {
                    AllowedSVCs.Add(SVCName);
                }
            }

            string AddSVCsInBlockedInRestrictedForSQLInjection = ConfigurationManager.AppSettings["AddSVCsInBlockedInRestrictedForSQLInjection"];

            if (!string.IsNullOrEmpty(AddSVCsInBlockedInRestrictedForSQLInjection))
            {
                foreach (var SVCName in AddSVCsInBlockedInRestrictedForSQLInjection.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
                {
                    BlockedInRestrictedAccessSVCs.Add(SVCName);
                }
            }
        }

        private WebContentFormat GetMessageContentFormat(Message message)
        {
            WebContentFormat format = WebContentFormat.Default;
            if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                WebBodyFormatMessageProperty bodyFormat;
                bodyFormat = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
                format = bodyFormat.Format;
            }

            return format;
        }

        private string ReadRawBody(ref Message message)
        {
            if (EnableParameterValidationLogs)
                mLogger.Debug("ReadRawBody -> Start");

            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] bodyBytes = bodyReader.ReadContentAsBase64();
            string messageBody = Encoding.UTF8.GetString(bodyBytes);

            // Now to recreate the message 
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
            writer.WriteStartElement("Binary");
            writer.WriteBase64(bodyBytes, 0, bodyBytes.Length);
            writer.WriteEndElement();
            writer.Flush();
            ms.Position = 0;
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            if (EnableParameterValidationLogs)
                mLogger.Debug("ReadRawBody -> End");

            return messageBody;
        }

        private string MessageToString(ref Message message, WebContentFormat messageFormat)
        {
            if (EnableParameterValidationLogs)
                mLogger.Debug("MessageToString -> Start");

            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = null;
            switch (messageFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    writer = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    // special case for raw, easier implemented separately 
                    return this.ReadRawBody(ref message);
            }

            message.WriteMessage(writer);
            writer.Flush();
            string messageBody = Encoding.UTF8.GetString(ms.ToArray());

            if (EnableParameterValidationLogs)
                mLogger.Debug("MessageToString 1 -> Start");

            // Here would be a good place to change the message body, if so desired. 

            // now that the message was read, it needs to be recreated. 
            ms.Position = 0;

            // if the message body was modified, needs to reencode it, as show below 
            // ms = new MemoryStream(Encoding.UTF8.GetBytes(messageBody)); 

            XmlDictionaryReader reader;
            if (messageFormat == WebContentFormat.Json)
            {
                reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
            }
            else
            {
                reader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
            }

            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            if (EnableParameterValidationLogs)
                mLogger.Debug("MessageToString -> End");

            return messageBody;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            string url = string.Empty;
            string method = string.Empty;
            string contents = string.Empty;
            string SVCName = string.Empty;
            Uri requestUri = request.Headers.To;
            try
            {
                if (EnableParameterValidationLogs)
                    mLogger.Debug("AfterReceiveRequest -> Start");

                if (RequireSqlInjectionHandling || IsPenTestingBuild)
                {
                    if (channel.LocalAddress.Uri != null && channel.LocalAddress.Uri.Segments != null && channel.LocalAddress.Uri.Segments.Length > 0)
                        SVCName = channel.LocalAddress.Uri.Segments.Last();

                    if (AllowedSVCs.Contains(SVCName) && (!RestrictedSVCsInSqlInjectionHandling || !BlockedInRestrictedAccessSVCs.Contains(SVCName)))
                    {
                        WebContentFormat messageFormat = this.GetMessageContentFormat(request);

                        if (!request.IsEmpty)
                            contents = this.MessageToString(ref request, messageFormat);
                        else if (EnableParameterValidationLogs)
                            mLogger.Debug("request is empty");

                        if (!string.IsNullOrEmpty(contents))
                        {
                            contents = Regex.Unescape(contents);
                            url = channel.LocalAddress.ToString();
                            HttpRequestMessageProperty httpReq = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

                            try
                            {
                                if (httpReq.Method.Equals("GET"))
                                {
                                    method = request.Properties["HttpOperationName"].ToString();
                                }
                                else if (messageFormat == WebContentFormat.Json)
                                {
                                    if (httpReq.Method.Equals("POST"))
                                    {
                                        method = requestUri.Segments.Last();
                                    }
                                    else
                                    {
                                        string[] contentsLocal = requestUri.ToString().Split('/');
                                        method = contentsLocal[contentsLocal.Length - 2];
                                    }
                                }
                                else
                                {
                                    if (httpReq.Method.Equals("POST"))
                                    {
                                        method = requestUri.Segments.Last();
                                    }
                                    else
                                    {
                                        string[] contentsLocal = requestUri.ToString().Split('/');
                                        method = contentsLocal[contentsLocal.Length - 2];
                                    }
                                }

                                if (string.IsNullOrEmpty(method))
                                {
                                    foreach (var header in httpReq.Headers.AllKeys)
                                    {
                                        if (header.Equals("SOAPAction"))
                                        {
                                            method = httpReq.Headers[header].Replace("\"", string.Empty).Split('/').Last();
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception exx)
                            {
                                //Not thrown since its not that significant for flow. Just for informational purpose.

                                if (EnableParameterValidationLogs)
                                    mLogger.Error("Failure in determining method - " + exx.ToString());
                            }

                            if (EnableParameterValidationLogs)
                                mLogger.Debug("url - " + url + ", method - " + method + ", requestUri - " + requestUri + ", SVCName - " + SVCName + ", contents - " + contents);

                            Regex inputParameterPattern = new Regex(@"^[^\';]*$");
                            Regex newLinePattern = new Regex(@"^[^\t\n\r]*$");
                            //if (EnablePenTestingBuildRegex)
                            //    inputParameterPattern = new Regex(@"^[^<>\';]*$");

                            if (!inputParameterPattern.IsMatch(contents) || (contents.Contains("--") && !newLinePattern.IsMatch(contents)) || (contents.Contains(@"/*") && contents.Contains(@"*/")))
                                throw new FaultException("Special Character Not Allowed in the Input variable");
                            //if (IsPenTestingBuild)
                            {
                                var contentsLower = contents.ToLower();

                                if ((contentsLower.Contains("<script>") || contentsLower.Contains("</script>")))
                                    throw new FaultException("Script tag is not allowed in the Input variable");
                                if (contentsLower.Contains("waitfor"))
                                    throw new FaultException("waitfor command is not allowed in the Input variable");
                            }
                        }
                        else if (EnableParameterValidationLogs)
                            mLogger.Debug("No content in request");
                    }
                    else if (EnableParameterValidationLogs)
                        mLogger.Debug("Svc name filtered for handling. SVCName - " + SVCName);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Failure in AfterReceiveRequest. url - " + url + ", method - " + method + ", requestUri - " + requestUri + ", SVCName - " + SVCName + ", contents - " + contents + ", Error in validation - " + ex.ToString());

                throw;
            }
            finally
            {
                if (EnableParameterValidationLogs)
                    mLogger.Debug("AfterReceiveRequest -> End");
            }

            return requestUri;

        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }
    }
}
