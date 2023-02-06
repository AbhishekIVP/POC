using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using com.ivp.rad.common;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.Data;
using System.Web.Script.Serialization;
using com.ivp.commom.commondal;
using System.Diagnostics;
using com.ivp.srmcommon;

namespace SRMMessageInspector
{
    public class ApiMonitoringApiCallPostToNode
    {
        public ApiMonitoringApiCallPostToNode()
        {
            this.ChainDetailsData = new ApiMonitoringApiCallDetailsData();
        }

        public int ApiUniqueId { get; set; }
        public string ChainName { get; set; }
        public string ChainURL { get; set; }
        public string ChainMethod { get; set; }
        public string ChainClientMachine { get; set; }
        public string ChainClientIP { get; set; }
        public int ChainPort { get; set; }
        public string ChainStartDateTime { get; set; }
        public string ChainEndDateTime { get; set; }
        public List<int> ChainTimeTaken { get; set; }
        public string ChainDetailsDataFormatRequest { get; set; }
        public string ChainDetailsDataFormatResponse { get; set; }
        public bool IsApiCallOver { get; set; }
        public ApiMonitoringApiCallDetailsData ChainDetailsData { get; set; }
        public int ThreadId { get; set; }
    }

    public class ApiMonitoringApiCallDetailsData
    {
        public string RequestHeaderData { get; set; }
        public string RequestBodyDataFileLocation { get; set; }
        public string RequestBodyDataFileName { get; set; }
        //public bool RequestBodyDataToShow { get; set; }
        public string RequestBodyErrorMsg { get; set; }
        public string ResponseHeaderData { get; set; }
        public string ResponseBodyDataFileLocation { get; set; }
        public string ResponseBodyDataFileName { get; set; }
        //public bool ResponseBodyDataToShow { get; set; }
        public string ResponseBodyErrorMsg { get; set; }
    }



    public class SRMMessageInspectorBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            throw new Exception("Behavior not supported on the consumer side!");
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SRMMessageInspector inspector = new SRMMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class SRMMessageInspectorBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SRMMessageInspectorBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(SRMMessageInspectorBehavior);
            }
        }
    }

    public class SRMMessageInspector : IDispatchMessageInspector
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMMessageInspector");

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
            mLogger.Error("ReadRawBody -> Start");
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
            mLogger.Error("ReadRawBody -> End");
            return messageBody;
        }


        private string MessageToString(ref Message message)
        {
            mLogger.Error("MessageToString -> Start");
            WebContentFormat messageFormat = this.GetMessageContentFormat(message);
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

            mLogger.Error("MessageToString 1 -> Start");

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
            mLogger.Error("MessageToString -> End");
            return messageBody;
        }

        private void WriteLog(string contents, string filePath)
        {
            File.AppendAllText(filePath, contents);
        }

        private string GetFileName(int Id, string request)
        {
            string fileName, directoryName = string.Empty, newLogFileName = string.Empty;
            List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                    {
                        fileName = node.Attributes["value"].Value;
                        directoryName = fileName.Substring(0, fileName.LastIndexOf("\\")) + "APIRequestLogs";
                        newLogFileName = directoryName + "\\API_" + request + "_" + Id + ".txt";
                    }
                }
            }

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            return newLogFileName;
        }


        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            string clientName = SRMMTConfig.SetClientNameFromHeaders(false);

            string filePath = string.Empty;
            Uri requestUri = request.Headers.To;
            bool isJSONRequest = false;
            bool isDebug = false;
            try
            {
                mLogger.Error("AfterReceiveRequest -> Start");
                HttpRequestMessageProperty httpReq = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                StringBuilder headers = new StringBuilder(httpReq.Method + " " + requestUri + Environment.NewLine);
                string url = channel.LocalAddress.ToString();
                string method = string.Empty;

                WebContentFormat messageFormat = this.GetMessageContentFormat(request);

                if (httpReq.Method.Equals("GET"))
                {
                    method = request.Properties["HttpOperationName"].ToString();
                    isJSONRequest = true;
                }
                else if (messageFormat == WebContentFormat.Json)
                {
                    isJSONRequest = true;
                    if (httpReq.Method.Equals("POST"))
                    {
                        method = requestUri.ToString().Split('/').Last();
                    }
                    else
                    {
                        string[] contents = requestUri.ToString().Split('/');
                        method = contents[contents.Length - 2];
                    }
                }

                foreach (var header in httpReq.Headers.AllKeys)
                {
                    if (header.Equals("SOAPAction"))
                    {
                        method = httpReq.Headers[header].Replace("\"", string.Empty).Split('/').Last();
                    }
                    else
                        headers.AppendLine(string.Format("{0} : {1}", header, httpReq.Headers[header]));

                    if (header.Equals("DebugRequest", StringComparison.OrdinalIgnoreCase) && httpReq.Headers[header].Equals("true", StringComparison.OrdinalIgnoreCase))
                        isDebug = true;
                }

                RemoteEndpointMessageProperty oRemoteEndpointMessageProperty = (RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];

                string clientIP = oRemoteEndpointMessageProperty.Address;
                string clientMachine = string.Empty;

                try
                {
                    clientMachine = Dns.GetHostEntry(oRemoteEndpointMessageProperty.Address).HostName;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                    clientMachine = clientIP;
                }

                int port = oRemoteEndpointMessageProperty.Port;

                Process currentProcess = Process.GetCurrentProcess();
                int processId = currentProcess.Id;
                string processName = currentProcess.ProcessName;
                int threadId = Thread.CurrentThread.ManagedThreadId;

                DataSet retValue = CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_InsertAPIRequest] '" + url + "','" + method + "','" + clientMachine + "','" + clientIP + "'," + port + ",'" + headers.ToString() + "'," + isJSONRequest + "," + processId + ",'" + processName + "'," + threadId, "RefMDBConnectionId");

                int Id = Convert.ToInt32(retValue.Tables[0].Rows[0]["api_call_id"]);

                OutgoingWebResponseContext ctx = WebOperationContext.Current.OutgoingResponse;
                ctx.Headers.Add("APIRequestId", Id.ToString());
                if (isDebug)
                    ctx.Headers.Add("DebugRequest", "true");


                var tempStartDateTime = Convert.ToDateTime(retValue.Tables[0].Rows[0]["start_date_time"]);
                string start_date_time = tempStartDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff");

                if (!isDebug)
                {
                    string DebugRequest = SRMCommon.GetConfigFromDB("DebugRequest");
                    if (!string.IsNullOrEmpty(DebugRequest) && DebugRequest.Equals("true", StringComparison.OrdinalIgnoreCase))
                        isDebug = true;
                }
                if (!request.IsEmpty && isDebug)
                {
                    filePath = GetFileName(Id, "Request");
                    string contents = this.MessageToString(ref request);
                    Task tsk = new Task(() => WriteLog(contents, filePath));
                    tsk.Start();
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_UpdateAPIRequestData] " + Id + ",'" + filePath.Replace("IVP", "<@>") + "', '', '', '', 1", "RefMDBConnectionId");
                }

                #region NodeJsServer : Initial Data

                //Prepare data to send to NodeJSServer initially
                ApiMonitoringApiCallPostToNode dataNode = new ApiMonitoringApiCallPostToNode();
                dataNode.IsApiCallOver = false;
                dataNode.ApiUniqueId = Id;
                dataNode.ChainName = "API_Call_" + dataNode.ApiUniqueId;
                dataNode.ChainURL = url;
                dataNode.ChainMethod = method;
                dataNode.ChainClientMachine = clientMachine;
                dataNode.ChainClientIP = clientIP;
                dataNode.ChainPort = port;
                dataNode.ChainStartDateTime = start_date_time;
                dataNode.ChainDetailsDataFormatRequest = isJSONRequest ? "JSON" : "XML";
                dataNode.ThreadId = threadId;

                dataNode.ChainDetailsData.RequestHeaderData = headers.ToString();

                // Handling Null Exception for Request Body
                if (!string.IsNullOrEmpty(filePath))
                {
                    dataNode.ChainDetailsData.RequestBodyDataFileLocation = filePath;
                    dataNode.ChainDetailsData.RequestBodyDataFileName = Path.GetFileName(filePath);
                    //if (System.IO.File.Exists(dataNode.ChainDetailsData.RequestBodyDataFileLocation))
                    //{
                    //    FileInfo f1 = new FileInfo(dataNode.ChainDetailsData.RequestBodyDataFileLocation);

                    //    //FileSize is in Bytes
                    //    var fileSize = f1.Length;

                    //    //If fileSize is less than or equal to 1MB, then we render the data on UI.
                    //    if (fileSize <= 1000000)
                    //    {
                    //        dataNode.ChainDetailsData.RequestBodyDataToShow = true;
                    //    }
                    //    else
                    //    {
                    //        dataNode.ChainDetailsData.RequestBodyDataToShow = false;
                    //        dataNode.ChainDetailsData.RequestBodyErrorMsg = "The Request Body is too Large to display. Please Download the file.";
                    //    }
                    //}
                    //else
                    //{
                    //    dataNode.ChainDetailsData.RequestBodyErrorMsg = "The Request Body File Not Found at specified location.";
                    //}
                }
                else
                {
                    dataNode.ChainDetailsData.RequestBodyDataFileLocation = "";
                    dataNode.ChainDetailsData.RequestBodyDataFileName = "";
                    //dataNode.ChainDetailsData.RequestBodyDataToShow = false;
                    dataNode.ChainDetailsData.RequestBodyErrorMsg = "No Request Body available. Please check server logs for more details.";

                }

                ////Send Notification to NodeJSServer
                //var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(RConfigReader.GetConfigAppSettings("NodeJsURl") + "ApiCallDataPostToNodeServer");
                ////var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8888/" + "ApiCallDataPostToNodeServer");
                //requestNode.Method = "POST";
                //requestNode.ContentType = "application/json";
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //string serializedData = serializer.Serialize(dataNode);
                //string json = "{\"data\":" + serializedData + "}";
                //var data = Encoding.ASCII.GetBytes(json);
                //requestNode.ContentLength = data.Length;
                //using (var streamWriter = requestNode.GetRequestStream())
                //{
                //    streamWriter.Write(data, 0, data.Length);
                //}                

                Thread nodeThread = new Thread(() =>
                {
                    try
                    {
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            return true;
                        };

                        //Send Notification to NodeJSServer
                        var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(RConfigReader.GetConfigAppSettings("NodeJsURl") + "ApiCallDataPostToNodeServer");
                        //var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8888/" + "ApiCallDataPostToNodeServer");
                        requestNode.Method = "POST";
                        requestNode.ContentType = "application/json";
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string serializedData = serializer.Serialize(dataNode);
                        string json = "{\"data\":" + serializedData + "}";
                        var data = Encoding.ASCII.GetBytes(json);
                        requestNode.ContentLength = data.Length;
                        using (var streamWriter = requestNode.GetRequestStream())
                        {
                            streamWriter.Write(data, 0, data.Length);
                        }

                        //No use. But need to take response.
                        var webResponse = (System.Net.HttpWebResponse)requestNode.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("AfterReceiveRequest -> Node -> " + ex.ToString());
                    }
                    finally
                    {
                        mLogger.Error("AfterReceiveRequest -> Node -> End");
                    }
                });
                nodeThread.Start();

                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("AfterReceiveRequest -> " + ex.ToString());
            }
            finally
            {
                mLogger.Error("AfterReceiveRequest -> End");
            }

            return requestUri;

        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //Debugging 
            //Thread.Sleep(10000);

            mLogger.Error("BeforeSendReply -> Start");
            string filePath = string.Empty;
            int Id = 1;
            bool isDebug = false;
            try
            {
                StringBuilder headers = new StringBuilder();
                foreach (var header in reply.Headers)
                {
                    headers.AppendLine(string.Format("{0} : {1}", header.Name, header.ToString()));
                }

                foreach (var header in WebOperationContext.Current.OutgoingResponse.Headers.AllKeys)
                {
                    if (header.Equals("APIRequestId"))
                        Id = Convert.ToInt32(WebOperationContext.Current.OutgoingResponse.Headers[header]);
                    else
                        headers.AppendLine(string.Format("{0} : {1}", header, WebOperationContext.Current.OutgoingResponse.Headers[header]));

                    if (header.Equals("DebugRequest", StringComparison.OrdinalIgnoreCase) && WebOperationContext.Current.OutgoingResponse.Headers[header].Equals("true", StringComparison.OrdinalIgnoreCase))
                        isDebug = true;
                }

                // To check if Response Body is JSON or XML
                bool isJSONResponse = false;
                WebContentFormat messageFormat = this.GetMessageContentFormat(reply);
                if (messageFormat == WebContentFormat.Json)
                {
                    isJSONResponse = true;
                }

                if (!isDebug)
                {
                    string DebugRequest = RConfigReader.GetConfigAppSettings("DebugRequest");
                    if (!string.IsNullOrEmpty(DebugRequest) && DebugRequest.Equals("true", StringComparison.OrdinalIgnoreCase))
                        isDebug = true;
                }
                if (!reply.IsEmpty && isDebug)
                {
                    filePath = GetFileName(Id, "Response");
                    string contents = this.MessageToString(ref reply);
                    Task tsk = new Task(() => WriteLog(contents, filePath));
                    tsk.Start();
                }

                DataSet retValue = CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_UpdateAPIRequestData] " + Id + ",'','" + headers.ToString() + "','" + (!string.IsNullOrEmpty(filePath) ? filePath.Replace("IVP", "<@>") : filePath) + "', " + isJSONResponse + ", 2", "RefMDBConnectionId");

                DateTime start_d_t = Convert.ToDateTime(retValue.Tables[0].Rows[0]["start_date_time"]);
                DateTime end_d_t = Convert.ToDateTime(retValue.Tables[0].Rows[0]["end_date_time"]);

                var tempEndDateTime = Convert.ToDateTime(retValue.Tables[0].Rows[0]["end_date_time"]);
                string end_date_time = tempEndDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff");

                #region NodeJsServer : Response Data

                //Prepare data to send to NodeJSServer initially
                ApiMonitoringApiCallPostToNode dataNode = new ApiMonitoringApiCallPostToNode();
                dataNode.IsApiCallOver = true;
                dataNode.ApiUniqueId = Id;
                dataNode.ChainDetailsData.ResponseHeaderData = headers.ToString();

                dataNode.ChainDetailsDataFormatResponse = isJSONResponse ? "JSON" : "XML";
                // Handling Null Exception for Response Body
                if (!string.IsNullOrEmpty(filePath))
                {
                    dataNode.ChainDetailsData.ResponseBodyDataFileLocation = filePath;
                    dataNode.ChainDetailsData.ResponseBodyDataFileName = Path.GetFileName(filePath);

                    //if (System.IO.File.Exists(dataNode.ChainDetailsData.ResponseBodyDataFileLocation))
                    //{
                    //    FileInfo f1 = new FileInfo(dataNode.ChainDetailsData.ResponseBodyDataFileLocation);

                    //    //FileSize is in Bytes
                    //    var fileSize = f1.Length;

                    //    //If fileSize is less than or equal to 1MB, then we render the data on UI.
                    //    if (fileSize <= 1000000)
                    //    {
                    //        dataNode.ChainDetailsData.ResponseBodyDataToShow = true;
                    //    }
                    //    else
                    //    {
                    //        dataNode.ChainDetailsData.ResponseBodyDataToShow = false;
                    //        dataNode.ChainDetailsData.ResponseBodyErrorMsg = "The Response Body is too Large to display. Please Download the file.";
                    //    }
                    //}
                    //else
                    //{
                    //    dataNode.ChainDetailsData.ResponseBodyErrorMsg = "The Response Body File Not Found at specified location.";
                    //}
                }
                else
                {
                    dataNode.ChainDetailsData.ResponseBodyDataFileLocation = "";
                    dataNode.ChainDetailsData.ResponseBodyDataFileName = "";
                    //dataNode.ChainDetailsData.ResponseBodyDataToShow = false;
                    dataNode.ChainDetailsData.ResponseBodyErrorMsg = "No Response Body available. Please check server logs for more details.";
                }

                // Handling Null Exception for End Date Time and accordingly compute ChainTimeTaken
                if (!string.IsNullOrEmpty(end_date_time))
                {
                    dataNode.ChainEndDateTime = end_date_time;

                    TimeSpan time_taken = end_d_t - start_d_t;

                    dataNode.ChainTimeTaken = new List<int>();
                    dataNode.ChainTimeTaken.Add(time_taken.Hours);           //Hour
                    dataNode.ChainTimeTaken.Add(time_taken.Minutes);         //Minutes
                    dataNode.ChainTimeTaken.Add(time_taken.Seconds);         //Seconds
                    dataNode.ChainTimeTaken.Add(time_taken.Milliseconds);    //Milli - Seconds
                }
                else
                {
                    dataNode.ChainEndDateTime = "NA";

                    dataNode.ChainTimeTaken = new List<int>();
                    dataNode.ChainTimeTaken.Add(-1);
                    dataNode.ChainTimeTaken.Add(-1);
                    dataNode.ChainTimeTaken.Add(-1);
                    dataNode.ChainTimeTaken.Add(-1);
                }

                ////Send Notification to NodeJSServer
                //var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(RConfigReader.GetConfigAppSettings("NodeJsURl") + "ApiCallDataPostToNodeServer");
                ////var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8888/" + "ApiCallDataPostToNodeServer");
                //requestNode.Method = "POST";
                //requestNode.ContentType = "application/json";
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //string serializedData = serializer.Serialize(dataNode);
                //string json = "{\"data\":" + serializedData + "}";
                //var data = Encoding.ASCII.GetBytes(json);
                //requestNode.ContentLength = data.Length;
                //using (var streamWriter = requestNode.GetRequestStream())
                //{
                //    streamWriter.Write(data, 0, data.Length);
                //}

                Thread nodeThread = new Thread(() =>
                {
                    try
                    {
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            return true;
                        };

                        // Send Notification to NodeJSServer
                        var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(RConfigReader.GetConfigAppSettings("NodeJsURl") + "ApiCallDataPostToNodeServer");
                        //var requestNode = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://localhost:8888/" + "ApiCallDataPostToNodeServer");
                        requestNode.Method = "POST";
                        requestNode.ContentType = "application/json";
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string serializedData = serializer.Serialize(dataNode);
                        string json = "{\"data\":" + serializedData + "}";
                        var data = Encoding.ASCII.GetBytes(json);
                        requestNode.ContentLength = data.Length;
                        using (var streamWriter = requestNode.GetRequestStream())
                        {
                            streamWriter.Write(data, 0, data.Length);
                        }

                        //No use. But need to take response.
                        var webResponse = (System.Net.HttpWebResponse)requestNode.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("BeforeSendReply -> Node -> " + ex.ToString());
                    }
                    finally
                    {
                        mLogger.Error("BeforeSendReply -> Node -> End");
                    }
                });
                nodeThread.Start();
                //var webResponse = (System.Net.HttpWebResponse)requestNode.GetResponse();

                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("BeforeSendReply -> " + ex.ToString());
            }
            finally
            {
                mLogger.Error("BeforeSendReply -> End");
            }

        }
    }

}
