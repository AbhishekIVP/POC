using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonService
{
    public class HTTPRequestUtility
    {
        public static HTTPResponse CreateRequest(string url, HTTPRequestType method, string requestData)
        {
            var resp = new HTTPResponse { IsSuccess = true };
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.ServicePoint.Expect100Continue = false;
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = method.ToString();

                if (method == HTTPRequestType.POST)
                {
                    StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                    requestStream.Write(requestData);
                    requestStream.Flush();
                    requestStream.Close();
                }

                using (HttpWebResponse responseStream = (HttpWebResponse)req.GetResponse())
                {
                    StreamReader sr = new StreamReader(responseStream.GetResponseStream());
                    resp.Response = sr.ReadToEnd();
                    responseStream.Close();
                }

            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            resp.FailureMessage = readStream.ReadToEnd();
                            resp.IsSuccess = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                resp.FailureMessage = e.ToString();
                resp.IsSuccess = false;
            }
            return resp;
        }
    }

    public class HTTPResponse
    {
        public string Response { get; set; }
        public bool IsSuccess { get; set; }
        public string FailureMessage { get; set; }
    }

    public enum HTTPRequestType
    {
        GET = 0,
        POST
    }
}
