using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class Update : Stream
    {
        #region Fields

        private static readonly char[] HREF_ATTRIBUTE = "href".ToCharArray();
        private static readonly char[] HTTP_PREFIX = "http://".ToCharArray();
        private static readonly char[] IMG_TAG = "img".ToCharArray();
        private static readonly char[] LINK_TAG = "link".ToCharArray();
        private static readonly char[] SCRIPT_TAG = "script".ToCharArray();
        private static readonly char[] SRC_ATTRIBUTE = "src".ToCharArray();
        private static readonly char[] SCRIPT_BLOCK = "|scriptBlock|ScriptPath|".ToCharArray();
        private static readonly char[] PARTIAL_POSTBACK_BLOCK = "1|#||4|".ToCharArray();
        private static readonly char[] HIDDEN_FIELD_BLOCK = "|hiddenField|".ToCharArray();

        private List<byte> _allText = new List<byte>();
        private byte[] _CssPrefix;
        Encoding _Encoding;
        private byte[] _ImagePrefix;
        private byte[] _JavascriptPrefix;
        private char[] _ApplicationPath;
        private byte[] _BaseUrl;
        private byte[] _CurrentFolder;

        /// <summary>
        /// Holds characters from last Write(...) call where the start tag did not
        /// end and thus the remaining characters need to be preserved in a buffer so 
        /// that a complete tag can be parsed
        /// </summary>
        private char[] _PendingBuffer = null;
        private Stream _ResponseStream;
        //private StringBuilder debug = new StringBuilder();
        private Func<string, string> _getVersionOfFile;

        private int postbackContentLength = 0;
        private bool isPartialPostback = false;
        private bool isHiddenFieldsStarted = false;
        private List<char> _allTextProcessed = new List<char>();

        #endregion Fields

        #region Constructors

        public Update(
            HttpResponse response,
            Func<string, string> getVersionOfFile,
            string imagePrefix,
            string javascriptPrefix,
            string cssPrefix,
            string baseUrl,
            string applicationPath,
            string currentRelativePath)
        {
            this._Encoding = response.Output.Encoding;
            this._ResponseStream = response.Filter;

            this._ImagePrefix = _Encoding.GetBytes(imagePrefix);
            this._JavascriptPrefix = _Encoding.GetBytes(javascriptPrefix);
            this._CssPrefix = _Encoding.GetBytes(cssPrefix);

            this._ApplicationPath = applicationPath.ToCharArray();
            this._BaseUrl = _Encoding.GetBytes(baseUrl);
            this._CurrentFolder = _Encoding.GetBytes(currentRelativePath);
            this._getVersionOfFile = getVersionOfFile;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position { get; set; }

        #endregion Properties

        #region Methods

        public override void Close()
        {
            WriteAllText(this._Encoding.GetChars(_allText.ToArray()));
            this.FlushPendingBuffer();
            _ResponseStream.Close();
            _ResponseStream = null;
            _getVersionOfFile = null;
            _PendingBuffer = null;
            _allText = null;
        }

        public override void Flush()
        {
            this.FlushPendingBuffer();
            _ResponseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ResponseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _ResponseStream.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            _ResponseStream.SetLength(length);
        }

        public void WriteAllText(char[] content)
        {
            try
            {
                int lastPosWritten = 0;
                int posScriptBlock = 0;

                int startPos = 0;
                if (HasMatch(content, 0, PARTIAL_POSTBACK_BLOCK))
                {
                    try
                    {
                        startPos = PARTIAL_POSTBACK_BLOCK.Length;

                        while (content.Length > startPos + 1 && char.IsDigit(content[startPos])) startPos++;

                        if (startPos != PARTIAL_POSTBACK_BLOCK.Length)
                        {
                            postbackContentLength = Convert.ToInt32(new string(content, PARTIAL_POSTBACK_BLOCK.Length, startPos - PARTIAL_POSTBACK_BLOCK.Length));

                            lastPosWritten = startPos;
                            isPartialPostback = true;
                        }
                        else
                        {
                            startPos = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        startPos = 0;
                        lastPosWritten = 0;
                        isPartialPostback = false;
                        SRMCacheBusterLogger.writeError("Error occured while identifying partial postback details for cache bust : " + Environment.NewLine + ex.ToString());
                    }
                }

                for (int pos = startPos; pos < content.Length; pos++)
                {
                    // See if tag start
                    char c = content[pos];

                    if (isPartialPostback && !isHiddenFieldsStarted && HasMatch(content, pos, HIDDEN_FIELD_BLOCK))
                        isHiddenFieldsStarted = true;

                    if ('<' == c)
                    {
                        pos++;
                        /* Make sure there are enough characters available in the buffer to finish
                         * tag start. This will happen when a tag partially starts but does not end
                         * For example, a partial img tag like <img
                         * We need a complete tag upto the > character.
                        */
                        if (HasTagEnd(content, pos))
                        {
                            if ('/' == content[pos])
                            {

                            }
                            else
                            {
                                if (HasMatch(content, pos, IMG_TAG))
                                {
                                    lastPosWritten = this.WritePrefixIf(SRC_ATTRIBUTE,
                                        content, pos, lastPosWritten, this._ImagePrefix);
                                }
                                else if (HasMatch(content, pos, SCRIPT_TAG))
                                {
                                    lastPosWritten = this.WritePrefixIf(SRC_ATTRIBUTE,
                                        content, pos, lastPosWritten, this._JavascriptPrefix);

                                    lastPosWritten = this.WritePathWithVersion(content, lastPosWritten);
                                }
                                else if (HasMatch(content, pos, LINK_TAG))
                                {
                                    lastPosWritten = this.WritePrefixIf(HREF_ATTRIBUTE,
                                        content, pos, lastPosWritten, this._CssPrefix);

                                    lastPosWritten = this.WritePathWithVersion(content, lastPosWritten);
                                }

                                // If buffer was written beyond current position, skip
                                // upto the position that was written
                                if (lastPosWritten > pos)
                                    pos = lastPosWritten;
                            }
                        }
                        else
                        {
                            // a tag started but it did not end in this buffer. Preserve the content
                            // in a buffer. On next write call, we will take an attempt to check it again
                            this._PendingBuffer = new char[content.Length - pos];
                            Array.Copy(content, pos, this._PendingBuffer, 0, content.Length - pos);

                            // Write from last write position upto pos. the rest is now in pending buffer
                            // will be processed later
                            this.WriteOutput(content, lastPosWritten, pos - lastPosWritten);

                            return;
                        }
                    }
                    else if ('|' == c && IsScriptBlock(content, pos, ref posScriptBlock))
                    //else if (content.Length > pos + SCRIPT_BLOCK.Length && HasMatch(content, pos, SCRIPT_BLOCK))
                    {
                        //lastPosWritten = this.WritePrefixIfScriptBlock(content, pos + SCRIPT_BLOCK.Length, lastPosWritten, this._JavascriptPrefix);

                        // First, write content upto this position
                        this.WriteOutput(content, lastPosWritten, pos - lastPosWritten);

                        pos = posScriptBlock;

                        lastPosWritten = this.WritePathWithVersion(content, pos, true);
                    }
                }

                // Write whatever is left in the buffer from last write pos to the end of the buffer
                this.WriteOutput(content, lastPosWritten, content.Length - lastPosWritten);

                if (isPartialPostback)
                {
                    isPartialPostback = false;

                    string postbackContentLengthString = postbackContentLength.ToString();

                    this.WriteOutput(PARTIAL_POSTBACK_BLOCK, 0, PARTIAL_POSTBACK_BLOCK.Length);
                    this.WriteOutput(postbackContentLengthString.ToCharArray(), 0, postbackContentLengthString.Length);
                    this.WriteOutput(_allTextProcessed.ToArray(), 0, _allTextProcessed.Count);
                }
            }
            catch (Exception ex)
            {
                SRMCacheBusterLogger.writeError("Error occured while Cache Busting : " + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
                _allText.Add(buffer[i]);
        }

        private int WritePathWithVersion(char[] content, int lastPosWritten, bool isScriptBlock = false)
        {
            // We will do it for relative urls only
            if (!HasMatch(content, lastPosWritten, HTTP_PREFIX))
            {
                int pos = lastPosWritten + 1;
                if (isScriptBlock)
                    while ('|' != content[pos]) pos++;
                else
                    while ('"' != content[pos]) pos++;
                // pos is now right before the closing double quote
                var relativePath = new string(content, lastPosWritten, pos - lastPosWritten);
                relativePath = relativePath.Trim();

                if (isPartialPostback && !isHiddenFieldsStarted && pos - lastPosWritten != relativePath.Length)
                    postbackContentLength -= pos - lastPosWritten - relativePath.Length;

                // get the last modification date time of the file at relative path
                var version = this._getVersionOfFile(relativePath).ToCharArray();

                if (isScriptBlock)
                {
                    string scriptNameLength = "|" + (version.Length + relativePath.Length).ToString();
                    // Write length of the file name and script block
                    this.WriteOutput(scriptNameLength.ToCharArray(), 0, scriptNameLength.Length);
                    this.WriteOutput(SCRIPT_BLOCK, 0, SCRIPT_BLOCK.Length);
                }

                // Emit the relative path as is
                this.WriteOutput(relativePath.ToCharArray(), 0, relativePath.Length);
                //this.WriteOutput(content, lastPosWritten, pos - lastPosWritten);
                lastPosWritten = pos;

                // Add a version number at the end of the path
                this.WriteOutput(version, 0, version.Length);
                if (isPartialPostback && !isHiddenFieldsStarted && version.Length > 0)
                    postbackContentLength += version.Length;
            }
            else
            {

            }

            return lastPosWritten;
        }

        private int FindAttributeValuePos(char[] attributeName, char[] content, int pos)
        {
            for (int i = pos; i < content.Length - attributeName.Length; i++)
            {
                // Tag closing reached but the attribute was not found
                if ('>' == content[i]) return -1;

                if (HasMatch(content, i, attributeName))
                {
                    pos = i + attributeName.Length;

                    // find the position of the double quote from where value is started
                    // We won't allow value without double quote, not even single quote.
                    // The content must be XHTML valid for now.
                    while ('"' != content[pos++]) ;

                    return pos;
                }
            }

            return -1;
        }

        private void FlushPendingBuffer()
        {
            /// Some characters were left in the buffer
            if (null != this._PendingBuffer)
            {
                this.WriteOutput(this._PendingBuffer, 0, this._PendingBuffer.Length);
                this._PendingBuffer = null;
            }
        }

        private bool HasMatch(char[] content, int pos, char[] match)
        {
            if (content.Length - pos <= match.Length)
                return false;

            for (int i = 0; i < match.Length; i++)
                if (content[pos + i] != match[i]
                    //&& content[pos + i] != char.ToUpper(match[i]))
                    //&& content[pos + i] != match[i]
                    )
                    return false;

            return true;
        }

        private bool HasTagEnd(char[] content, int pos)
        {
            for (; pos < content.Length; pos++)
                if ('>' == content[pos])
                    return true;

            return false;
        }

        private void WriteBytes(byte[] bytes, int pos, int length)
        {
            this._ResponseStream.Write(bytes, 0, bytes.Length);
        }

        private void WriteOutput(char[] content, int pos, int length)
        {
            if (length == 0) return;

            //debug.Append(content, pos, length);

            if (isPartialPostback)
            {
                _allTextProcessed.AddRange(new string(content, pos, length));
            }
            else
            {
                byte[] buffer = this._Encoding.GetBytes(content, pos, length);
                this.WriteBytes(buffer, 0, buffer.Length);
            }
        }

        //private void WriteOutput(string content)
        //{
        //    //debug.Append(content);
        //    byte[] buffer = this._Encoding.GetBytes(content);
        //    this.WriteBytes(buffer, 0, buffer.Length);
        //}

        /// <summary>
        /// Write the prefix if the specified attribute was found and the attribute has a value
        /// that does not start with http:// prefix.
        /// If atttribute is not found, it just returns the lastWritePos as it is
        /// If attribute was found but the attribute already has a fully qualified URL, then return lastWritePos as it is
        /// If attribute has relative URL, then lastWritePos is the starting position of the attribute value. However,
        /// content from lastWritePos to position of the attribute value will already be written to output
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="content"></param>
        /// <param name="pos"></param>
        /// <param name="lastWritePos"></param>
        /// <param name="prefix"></param>
        /// <returns>The last position upto which content was written.</returns>
        private int WritePrefixIf(char[] attributeName, char[] content, int pos, int lastWritePos, byte[] prefix)
        {
            // write upto the position where image source tag comes in
            int attributeValuePos = this.FindAttributeValuePos(attributeName, content, pos);

            // ensure attribute was found
            if (attributeValuePos > 0)
            {
                if (HasMatch(content, attributeValuePos, HTTP_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do
                    return lastWritePos;
                }
                else
                {
                    // It's a relative URL. So, let's prefix the URL with the
                    // static domain name

                    // First, write content upto this position
                    this.WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    lastWritePos = attributeValuePos;

                    // Now write the prefix
                    if (prefix.Length > 0)
                    {
                        //this.WriteBytes(prefix, 0, prefix.Length);
                    }
                    else
                    {
                        // Turn this on if you want to emit an absolute URL from the relative URL
                        //this.WriteBytes(this._BaseUrl, 0, this._BaseUrl.Length);
                    }

                    // If the attribute value starts with the application path it needs to be skipped as 
                    // that value should be in the prefix. Doubling it will cause problems. This occurs
                    // with some of the scripts.
                    if (HasMatch(content, attributeValuePos, _ApplicationPath))
                    {
                        // Aboslute path starting with / or /Vdir. So, we need to keep the /Vdir/ part.
                        attributeValuePos = attributeValuePos + _ApplicationPath.Length;
                    }
                    else
                    {
                        // Relative path. So, we need to emit the current folder path. eg folder/
                        //if (this._CurrentFolder.Length > 0)
                        //    this.WriteBytes(this._CurrentFolder, 0, this._CurrentFolder.Length);
                    }

                    // Ensure the attribute value does not start with a leading slash because the prefix
                    // is supposed to have a trailing slash. If value does start with a leading slash,
                    // skip it
                    if ('/' == content[attributeValuePos]) attributeValuePos++;

                    if (attributeValuePos != lastWritePos)
                        this.WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    return attributeValuePos;
                }
            }
            else
            {
                return lastWritePos;
            }
        }

        private bool IsScriptBlock(char[] content, int currentPos, ref int attributeValuePos)
        {
            int pos = currentPos + 1;
            while (content.Length > pos + 1 && char.IsDigit(content[pos])) pos++;

            // If the attribute value starts with the application path it needs to be skipped as 
            // that value should be in the prefix. Doubling it will cause problems. This occurs
            // with some of the scripts.
            bool isMatch = content.Length > pos + SCRIPT_BLOCK.Length && HasMatch(content, pos, SCRIPT_BLOCK);

            pos = pos + SCRIPT_BLOCK.Length;

            if (isMatch && !HasMatch(content, pos, HTTP_PREFIX))
                attributeValuePos = pos;
            else
                isMatch = false;

            return isMatch;
        }

        #endregion Methods
    }

    public class SRMCacheBuster
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMCacheBuster");

        static bool DisableCacheBust = false;
        static bool EnableCacheBustingLogs = false;

        static SRMCacheBuster()
        {
            string DisableCacheBustString = ConfigurationManager.AppSettings["DisableCacheBurst"];

            if (!string.IsNullOrEmpty(DisableCacheBustString) && DisableCacheBustString.Equals("true", StringComparison.OrdinalIgnoreCase))
                DisableCacheBust = true;

            string EnableCacheBustingLogsString = ConfigurationManager.AppSettings["EnableCacheBustingLogs"];

            if (!string.IsNullOrEmpty(EnableCacheBustingLogsString) && EnableCacheBustingLogsString.Equals("true", StringComparison.OrdinalIgnoreCase))
                EnableCacheBustingLogs = true;
        }

        public void BustCache(HttpRequest Request, HttpResponse Response, HttpServerUtility Server)
        {
            try
            {
                if (DisableCacheBust)
                {
                    if (EnableCacheBustingLogs)
                        mLogger.Debug("Cache Bust disabled");
                }
                else
                {
                    if (EnableCacheBustingLogs)
                        mLogger.Debug("Before Cache Busting -> " + Request.FilePath);
                    BustCacheInternal(Request, Response, Server);
                    if (EnableCacheBustingLogs)
                        mLogger.Debug("After Cache Busting -> " + Request.FilePath);
                }
            }
            catch (Exception ex)
            {
                SRMCacheBusterLogger.writeError("Error occured while Cache Busting : " + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        private void BustCacheInternal(HttpRequest Request, HttpResponse Response, HttpServerUtility Server)
        {
            //if (Request.IsLocal)
            //    System.Threading.Thread.Sleep(50);
            var request = Request;
            var url = request.Url;
            var applicationPath = request.ApplicationPath;
            string fullurl = url.ToString();
            string baseUrl = url.Scheme + "://" + url.Authority + applicationPath.TrimEnd('/') + '/';
            string currentRelativePath = request.AppRelativeCurrentExecutionFilePath;
            if (request.HttpMethod == "GET" || request.HttpMethod == "POST")
            {
                if (currentRelativePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) && !(currentRelativePath.IndexOf("SRMFileUpload.aspx", StringComparison.OrdinalIgnoreCase) > -1 || currentRelativePath.IndexOf("Download.aspx", StringComparison.OrdinalIgnoreCase) > -1 || currentRelativePath.IndexOf("FileUpload.aspx", StringComparison.OrdinalIgnoreCase) > -1 || currentRelativePath.IndexOf("SMUploadFile.aspx", StringComparison.OrdinalIgnoreCase) > -1 || currentRelativePath.IndexOf("RADGridExportToExcel.aspx", StringComparison.OrdinalIgnoreCase) > -1))
                {
                    var folderPath = currentRelativePath.Substring(2, currentRelativePath.LastIndexOf('/') - 1);
                    Response.Filter = new com.ivp.common.Update(
                            Response,
                            relativePath =>
                            {
                                try
                                {
                                    //if (!string.IsNullOrEmpty(relativePath))
                                    //    relativePath = relativePath.Trim();

                                    if (relativePath.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || relativePath.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                                    {
                                        relativePath = System.Web.HttpUtility.UrlDecode(relativePath);
                                        string version = string.Empty;
                                        try
                                        {
                                            List<string> PathsTriedForFileInfo = new List<string>();
                                            try
                                            {
                                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                                                PathsTriedForFileInfo.Add(path);
                                                var fileInfo = new System.IO.FileInfo(path);

                                                if (fileInfo.Exists)
                                                    version = "?smver=" + fileInfo.LastWriteTime.ToString("yyyyMMddHHmmssfff");
                                            }
                                            catch (Exception)
                                            {
                                            }

                                            if (string.IsNullOrEmpty(version))
                                            {
                                                var physicalPath = Server.MapPath(relativePath);
                                                PathsTriedForFileInfo.Add(physicalPath);
                                                var fileInfo = new System.IO.FileInfo(physicalPath);

                                                if (fileInfo.Exists)
                                                    version = "?smver=" + fileInfo.LastWriteTime.ToString("yyyyMMddHHmmssfff");
                                                else
                                                    throw new Exception("Not able to fetch file info. Tried using paths - " + string.Join(",", PathsTriedForFileInfo));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SRMCacheBusterLogger.writeError("Failed to version file : (" + relativePath + ") with error" + Environment.NewLine + ex.ToString());
                                        }
                                        return version;
                                    }
                                    else
                                        return string.Empty;
                                }
                                catch (Exception ex)
                                {
                                    SRMCacheBusterLogger.writeError("Failed to version file : (" + relativePath + ") with error" + Environment.NewLine + ex.ToString());
                                    return string.Empty;
                                }
                            },
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            baseUrl,
                            applicationPath,
                            folderPath);
                }
            }
        }
    }

    static class SRMCacheBusterLogger
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMCacheBusterLogger");

        public static void writeError(string errorMessage)
        {
            string newLogFileName = string.Empty;
            try
            {
                List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
                foreach (XmlDocument loggerDoc in lstConfig)
                {
                    XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                        {
                            string fileName = node.Attributes["value"].Value;
                            newLogFileName = fileName.Substring(0, fileName.LastIndexOf("\\")) + "\\SRMCacheBust" + DateTime.Now.ToString("yyyyddMM") + ".txt";
                            node.Attributes["value"].Value = newLogFileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Error occured while creating new log file path : " + Environment.NewLine + ex.ToString());
            }

            mLogger.Error(errorMessage);
            if (!string.IsNullOrEmpty(newLogFileName))
            {
                try
                {
                    errorMessage = Environment.NewLine + DateTime.Now + "  ==>  " + errorMessage + Environment.NewLine;
                    if (!string.IsNullOrEmpty(newLogFileName))
                        File.AppendAllText(newLogFileName, errorMessage);
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error occured while writing error log for cache bust : " + Environment.NewLine + ex.ToString());
                }
            }
        }
    }
}
