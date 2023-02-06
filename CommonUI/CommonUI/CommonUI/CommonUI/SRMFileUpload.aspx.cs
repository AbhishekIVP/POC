using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.ivp.rad.viewmanagement;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Reflection;
using com.ivp.rad.utils;
using com.ivp.srmcommon;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMFileUpload : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Upload Case
            base.Page_Load(sender, e);
            string contentType = string.Empty;

            if (Request.QueryString["directUploadFile"] != null && bool.Parse(Request.QueryString["directUploadFile"]) == true)
            {
                UploadFileDirect();
                return;
            }

            //Download case
            else { 
                string fileName;
                bool isResource = false;
                if (!string.IsNullOrEmpty(Request.QueryString["res"]))
                    isResource = true;

                bool fileOnServer = false;
                if (!string.IsNullOrEmpty(Request.QueryString["fileonserver"]))
                    fileOnServer = true;

                bool isTempFile = false;
                if (!string.IsNullOrEmpty(Request.QueryString["guid"]))
                    isTempFile = true;
    
                else
                {
                    try
                    {
                        if (fileOnServer)
                        {
                            fileName = Convert.ToString(Request.QueryString["name"]);
                            string fullFilePath = Convert.ToString(Request.QueryString["fullPath"]);
                            FileInfo fInfo = new FileInfo(fullFilePath);

                            if (string.IsNullOrEmpty(fileName))
                            {
                                fileName = fInfo.Name;
                            }


                            contentType = fInfo.Extension.ToString().Replace(".", "");
                            if (contentType.Equals("xml", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/xml";
                            else if (contentType.Equals("xls", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/ms-excel";
                            else if (contentType.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                            Response.Clear();
                            Stream stream = new FileStream(fullFilePath, FileMode.Open);
                            if (stream != null)
                            {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                stream.Close();
                                Response.ContentType = contentType;
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                                Response.BinaryWrite(data);
                            }
                            if (Convert.ToString(Request.QueryString["deleteFile"]) == "true")
                            {
                                if (File.Exists(fullFilePath))
                                    File.Delete(fullFilePath);
                            }
                        }
                        else if (isTempFile)
                        {
                            
                            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["contentType"])))
                            {
                                contentType = Convert.ToString(Request.QueryString["contentType"]);
                            }
                            if (contentType.Equals("xml", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/xml";
                            else if (contentType.Equals("xls", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/ms-excel";
                            else if (contentType.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
                                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                            string path = Server.MapPath("~/" + Request.QueryString["guid"].ToString() + "." + contentType);
                            Response.Clear();
                            Stream stream = new FileStream(path, FileMode.Open);
                            if (stream != null)
                            {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                stream.Close();

                                if (contentType == "xls")
                                    Response.ContentType = "application/ms-excel";
                                else if (contentType == "xlsx")
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                                fileName = Convert.ToString(Request.QueryString["name"]) + "." + contentType;
                                Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
                                Response.BinaryWrite(data);
                            }

                            if (File.Exists(Server.MapPath("~/" + Request.QueryString["guid"].ToString() + "." + contentType)))
                                File.Delete(path);
                        }
                        else if (isResource)
                        {
                            Response.Clear();
                            Assembly assembly = Assembly.GetExecutingAssembly();
                            Stream stream = assembly.GetManifestResourceStream("com.ivp.secm.ui.ConfigFiles.Embedded." + Request.QueryString["name"].ToString());
                            if (stream != null)
                            {
                                byte[] data = new byte[stream.Length];
                                stream.Read(data, 0, data.Length);
                                stream.Close();
                                Response.ContentType = "application/ms-excel";
                                fileName = Convert.ToString(Request.QueryString["name"]);
                                Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
                                Response.BinaryWrite(data);
                            }
                        }
                    }
                    catch (Exception eeeee)
                    {
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        Response.Write("<script>alert('");
                        Response.Write("There was an error downloading file." + eeeee.Message);
                        Response.Write("');</script>");
                    }
                    finally
                    {
                        Response.End();
                        if (isTempFile && File.Exists(Server.MapPath("~/" + Request.QueryString["guid"].ToString() + "." + contentType)))
                            File.Delete(Server.MapPath("~/" + Request.QueryString["guid"].ToString() + "." + contentType));
                    }
                }
            }
        }

        private void UploadFileDirect()
        {
            try
            {
                SRMFileUploadInfo objFileUploadInfo = new SRMFileUploadInfo();
                SRMFileUploadReturnInfo objFileUploadReturnInfo = new SRMFileUploadReturnInfo();
                byte[] fileByteData;
                HttpPostedFile postedFile = Request.Files[0];

                string fileDisplayName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('\\') + 1).Trim();
                string fileExt = fileDisplayName.Substring(fileDisplayName.LastIndexOf('.'));
                string contentType = postedFile.ContentType;

                //handled on the UI side as well
                var allowedFileTypeStr = ".xls|.xlsx|.7z|.xml";
                //var allowedFileTypeStr = RConfigReader.GetConfigAppSettings("SRMAllowedFileTypes");
                var allowedFileTypes = allowedFileTypeStr.ToLower().Split('|');
                if (allowedFileTypeStr != string.Empty && allowedFileTypeStr != "*" && !allowedFileTypes.Contains(fileExt.ToLower()))
                {
                    Response.Clear();
                    Response.Write("Error While Uploading File. File type not allowed.");
                    Response.End();
                    return;
                }

                objFileUploadInfo.FileName = fileDisplayName;
                objFileUploadInfo.FileType = postedFile.ContentType;
                objFileUploadInfo.FileSize = postedFile.ContentLength;
                objFileUploadInfo.User = SessionInfo.LoginName;

                using (MemoryStream ms = new MemoryStream())
                {
                    postedFile.InputStream.CopyTo(ms);
                    fileByteData = ms.ToArray();
                }

                //TODO
                //add request query string in future
                //also caching
                string directoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "MigrationUtilityFiles\\" + SessionInfo.LoginName + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\Upload_UI_Location\\";
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string filePath = directoryPath + postedFile.FileName;
                DeleteFile(filePath);
                //write file
                File.WriteAllBytes(filePath, fileByteData);

                bool scanPassed;
                SRMCommon.WaitForScanForUploadedFile(SessionInfo.LoginName, filePath, null, string.Empty, out scanPassed);
                if (!scanPassed)
                {
                    Response.Clear();
                    Response.Write("Error While Uploading File. File type not allowed.");
                    Response.End();
                    return;
                }

                objFileUploadReturnInfo.filePath = filePath;
                objFileUploadReturnInfo.fileName = fileDisplayName;
                objFileUploadReturnInfo.fileExtension = fileExt;

                Response.Clear();
                Response.Write(objFileUploadReturnInfo.filePath.ToString() + "|" + objFileUploadReturnInfo.fileName.ToString() + "|" + objFileUploadReturnInfo.fileExtension);
                Response.End();

            }
            catch (Exception ex) { throw ex; }
            finally
            {

            }
        }

        private bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class SRMFileUploadInfo
    {
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>The type of the file.</value>
        public string FileType { get; set; }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public string User { get; set; }

    }

    public class SRMFileUploadReturnInfo {
        public string fileName{ get; set; }
        public string filePath{ get; set; }
        public string fileExtension{ get; set; }
    }
}