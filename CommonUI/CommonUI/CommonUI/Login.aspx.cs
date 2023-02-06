using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.culturemanager;
using com.ivp.rad.raduicontrols.HelperClasses;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using com.ivp.secm.ui;
using com.ivp.rad.controls.topmenucontrol;
using com.ivp.rad.baselicensemanager;
using com.ivp.SRMCommonModules;

namespace CommonUI
{
    public partial class Login : BasePage
    {
        static Regex encodedUrlPattern = new Regex("^[0-9]+$", RegexOptions.Compiled);
        string secID = string.Empty;
        string identifier = string.Empty;
        Dictionary<string, string> otherParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);        
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreInit"/> event at the beginning of page initialization.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            //if (Session["themeName"] != null && Page.Theme != Session["themeName"].ToString())
            //{
            //    Page.Theme = Session["themeName"].ToString();
            //}

        }

        //protected override void PageLoadEvent(object sender, EventArgs e)
        //{
        //    base.PageLoadEvent(sender, e);
        //    bool exceptionCaught = false;
        //    //string[] keyValuePair = new string[2];
        //    var keyValuePair = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        //    lblYear.Text = DateTime.Now.Year.ToString();
        //    string queryString = "";
        //    //  SMUiUtils.ApplicationMode = GetApplicationModeForBrowser();
        //    if (Context.Cache["RedirectData"] != null && ((System.Collections.Specialized.NameValueCollection)(Context.Cache["RedirectData"])).AllKeys.Length > 0)
        //    {
        //        queryString = Context.Cache["RedirectData"].ToString();
        //        Context.Cache.Remove("RedirectData");
        //    }
        //    else if (Request.Url.Query.Length > 1)
        //        queryString = Request.Url.Query.Substring(1);

        //    //if (queryString != "" && queryString.ToLower() != "identifier=home")
        //    if (Login.encodedUrlPattern.Match(queryString).Success)
        //    {
        //        try
        //        {
        //            queryString = decodeCustomURL(queryString);
        //            keyValuePair = queryString.Split('&').Where(x => x.Split('=').Length == 2).ToDictionary(x => x.Split('=')[0], y => y.Split('=')[1], StringComparer.OrdinalIgnoreCase);
        //        }
        //        catch (Exception ex)
        //        {
        //            secID = "";
        //            exceptionCaught = true;
        //            lblLicenceExpired.Visible = true;
        //            lblLicenceExpired.Text = "Invalid Request Parameters:" + queryString;
        //            //throw new Exception("Invalid Request Parameters:" + queryString);
        //        }
        //    }
        //    else
        //    {
        //        Page.SetFocus(txtLogin);
        //        int daysToExpire = (int)RBaseLicenseManager.DaysToExpire;
        //        if (daysToExpire < 30 && daysToExpire > 0)
        //        {
        //            lblLicenceExpired.Visible = true;
        //            lblLicenceExpired.Text = "Your licence for this application will expire in " + daysToExpire.ToString() + " days.";
        //        }
        //        else if (daysToExpire == 0)
        //        {
        //            lblLicenceExpired.Visible = true;
        //            lblLicenceExpired.Text = "Your licence for this application will expire today.";
        //        }
        //        else
        //        {
        //            lblLicenceExpired.Visible = false;
        //        }

        //        if (!IsPostBack)
        //        {
        //            bool IsPageRedirectedFromLogoutPage = false;// (Request.Url.Query != null && Request.Url.Query != String.Empty && SMUiUtils.DecodeCustomURL(Request.Url.Query.Remove(0, 1)).ToUpperInvariant().Contains("ISREDIRECTEDFROMLOGOUT=TRUE"));
        //            if (!IsPageRedirectedFromLogoutPage && RUserMgtConfigLoader.IsAutoLogIn)
        //            {
        //                string userName = User.Identity.Name;
        //                if (userName.Contains("\\"))
        //                {
        //                    txtLogin.Text = userName.Split('\\').Last();
        //                }
        //                else
        //                {
        //                    txtLogin.Text = userName;
        //                }
        //                btnLogin_Click(sender, e);
        //            }
        //        }
        //        return;
        //    }

        //    if (!keyValuePair.ContainsKey("ISREDIRECTEDFROMLOGOUT"))
        //    {
        //        RUserMgtConfigLoader.IsAutoLogIn = false;
        //        if (keyValuePair.Count > 0)
        //        {
        //            if (keyValuePair.ContainsKey("USER"))
        //            {
        //                txtLogin.Text = keyValuePair["USER"];
        //                keyValuePair.Remove("USER");
        //            }
        //            if (keyValuePair.ContainsKey("SECID"))
        //            {
        //                secID = keyValuePair["SECID"];
        //                keyValuePair.Remove("SECID");
        //            }
        //            if (keyValuePair.ContainsKey("identifier"))
        //            {
        //                identifier = keyValuePair["identifier"];
        //                keyValuePair.Remove("identifier");
        //            }
        //            otherParams = keyValuePair;
        //        }
        //        else
        //            return;

        //        if (!exceptionCaught)
        //        {
        //            RUserMgtConfigLoader.IsAutoLogIn = true;
        //            btnLogin_Click(sender, e);
        //        }
        //        else
        //            RUserMgtConfigLoader.IsAutoLogIn = false;
        //    }
        //}

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Page_Load(object sender, EventArgs e)
        {
            //        var dtquery1 = SMDALWrapper.ExecuteSelectQuery(@" select distinct c.field_name,cdet.column_id,cdet.specified_id  from IVPSecMasterVendor.dbo.ivp_secmv_feed_summary a
            //	   INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master d on a.vendor_id=d.vendor_id
            //       INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_source_type b on a.source_type=b.feed_source_id
            //       INNER JOIN IVPRAD.dbo.ivp_rad_file_field_details c on a.file_id=c.file_id
            //       right join IVPSecMaster.dbo.ivp_secm_wso_column_details cdet on cdet.column_id=c.field_id
            //       where a.vendor_id='256' and b.feed_source_type_name='Markit WSO' and d.is_active=1 and c.is_active=1 and a.is_active=1 ", ConnectionConstants.SecMaster_Connection);
            //  var dtquery2= SMDALWrapper.ExecuteSelectQuery(@"select distinct c.field_name,c.field_id,a.feed_id  from IVPSecMasterVendor.dbo.ivp_secmv_feed_summary a 
            //INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master d on a.vendor_id=d.vendor_id 
            //INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_source_type b on a.source_type=b.feed_source_id
            //INNER JOIN IVPRAD.dbo.ivp_rad_file_field_details c on a.file_id=c.file_id where a.vendor_id='256' and b.feed_source_type_name='Markit WSO' and d.is_active=1 and c.is_active=1 and a.is_active=1
            //              ", ConnectionConstants.SecMaster_Connection);

            //  var qry1 = dtquery1.Tables[0].AsEnumerable().Select(a => new { name = a["field_name"].ToString() });
            //  var qry2 = dtquery2.Tables[0].AsEnumerable().OrderBy(a => a.Field<string>("field_name")).Select(b => new { name = b["field_name"].ToString() });

            //     var exceptAB = qry1.Except(qry2);
            //         var exceptBA = qry2.Except(qry1);

            bool exceptionCaught = false;
            //string[] keyValuePair = new string[2];
            var keyValuePair = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            lblYear.Text = DateTime.Now.Year.ToString();
            string queryString = "";
            //  SMUiUtils.ApplicationMode = GetApplicationModeForBrowser();
            if (Context.Cache["RedirectData"] != null && ((System.Collections.Specialized.NameValueCollection)(Context.Cache["RedirectData"])).AllKeys.Length > 0)
            {
                queryString = Context.Cache["RedirectData"].ToString();
                Context.Cache.Remove("RedirectData");
            }
            else if (Request.Url.Query.Length > 1)
                queryString = Request.Url.Query.Substring(1);

            //if (queryString != "" && queryString.ToLower() != "identifier=home")
            if (Login.encodedUrlPattern.Match(queryString).Success)
            {
                try
                {
                    queryString = decodeCustomURL(queryString);
                    keyValuePair = queryString.Split('&').Where(x => x.Split('=').Length == 2).ToDictionary(x => x.Split('=')[0], y => y.Split('=')[1], StringComparer.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    secID = "";
                    exceptionCaught = true;
                    lblLicenceExpired.Visible = true;
                    lblLicenceExpired.Text = "Invalid Request Parameters:" + queryString;
                    //throw new Exception("Invalid Request Parameters:" + queryString);
                }
            }
            else
            {
                Page.SetFocus(txtLogin);
                int daysToExpire = (int)SRMCommonLicenseHandler.objRBaseLicenseManager.DaysToExpire;
                if (daysToExpire < 30 && daysToExpire > 0)
                {
                    lblLicenceExpired.Visible = true;
                    lblLicenceExpired.Text = "Your licence for this application will expire in " + daysToExpire.ToString() + " days.";
                }
                else if (daysToExpire == 0)
                {
                    lblLicenceExpired.Visible = true;
                    lblLicenceExpired.Text = "Your licence for this application will expire today.";
                }
                else
                {
                    lblLicenceExpired.Visible = false;
                }

                if (!IsPostBack)
                {
                    bool IsPageRedirectedFromLogoutPage = false;// (Request.Url.Query != null && Request.Url.Query != String.Empty && SMUiUtils.DecodeCustomURL(Request.Url.Query.Remove(0, 1)).ToUpperInvariant().Contains("ISREDIRECTEDFROMLOGOUT=TRUE"));
                    if (!IsPageRedirectedFromLogoutPage && RUserMgtConfigLoader.IsAutoLogIn)
                    {
                        string userName = User.Identity.Name;
                        if (userName.Contains("\\"))
                        {
                            txtLogin.Text = userName.Split('\\').Last();
                        }
                        else
                        {
                            txtLogin.Text = userName;
                        }
                        btnLogin_Click(sender, e);
                    }
                }
                return;
            }

            if (!keyValuePair.ContainsKey("ISREDIRECTEDFROMLOGOUT"))
            {
                RUserMgtConfigLoader.IsAutoLogIn = false;
                if (keyValuePair.Count > 0)
                {
                    if (keyValuePair.ContainsKey("USER"))
                    {
                        txtLogin.Text = keyValuePair["USER"];
                        keyValuePair.Remove("USER");
                    }
                    if (keyValuePair.ContainsKey("SECID"))
                    {
                        secID = keyValuePair["SECID"];
                        keyValuePair.Remove("SECID");
                    }
                    if (keyValuePair.ContainsKey("identifier"))
                    {
                        identifier = keyValuePair["identifier"];
                        keyValuePair.Remove("identifier");
                    }
                    otherParams = keyValuePair;
                }
                else
                    return;

                if (!exceptionCaught)
                {
                    RUserMgtConfigLoader.IsAutoLogIn = true;
                    btnLogin_Click(sender, e);
                }
                else
                    RUserMgtConfigLoader.IsAutoLogIn = false;
            }

        }

        private string decodeCustomURL(string url)
        {
            string decodedURL = "";
            for (int i = 0; i < url.Length; i = i + 3)
                decodedURL += Convert.ToChar(Convert.ToInt32(url.Substring(i, 3)));
            return decodedURL;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the Click event of the btnLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // RLicenseManager obj = new RLicenseManager();
            //RLicenseEnum lisStatus = obj.LicenseResult;

            //if (RBaseLicenseManager.LicenseResult == RBaseLicenseEnum.LICENSE_VALID)
            //{
            RUserManagementService objUserManager = new RUserManagementService();
            RUser objUser = objUserManager.GetUserDetailsGDPR(txtLogin.Text, txtPassword.Text);

            if (objUser == null)
            {
                lblServerMessage.Visible = true;
                lblServerMessage.Text = "Invalid username or password.";
            }
            else
            {
                com.ivp.rad.viewmanagement.RSessionInfo objSessionInfo = new com.ivp.rad.viewmanagement.RSessionInfo();
                RUserController objUserController = new RUserController();
                objUserController.UpdateLastLoginTime(objUser.UserName);
                RUserInfo objUserInfo = new RUserManagementService().GetUserInfoGDPR(objUser.UserName);
                objSessionInfo.LoginName = objUserInfo.UserName;
                objSessionInfo.FirstName = objUserInfo.FirstName;
                objSessionInfo.MiddleName = objUserInfo.MiddleName;
                objSessionInfo.LastName = objUserInfo.LastName;
                objSessionInfo.EmailID = objUserInfo.EmailId;
                objSessionInfo.LastLoginTime = objUserInfo.LastLoginTime;
                objSessionInfo.LastAccessTime = DateTime.Now;
                objSessionInfo.Privilege = RDBUtils.ConvertDataSetToList(objUserManager.GetPrivilegesForUserGDPR(objUserInfo.UserName));
                objSessionInfo.UserMasterId = objUserInfo.UserMasterID;
                objSessionInfo.AccountId = objUser.AccountId;
                objSessionInfo.AccountName = objUser.AccountName;
                objSessionInfo.Roles = objUser.Roles;
                objSessionInfo.GroupNames = objUser.GroupNames;

                objSessionInfo.CultureInfo = RCultureController.GetCultureInfo(objUserInfo.CultureName);
                com.ivp.rad.RSessionManager.SessionManager sessionMgr = new com.ivp.rad.RSessionManager.SessionManager();
                sessionMgr.SetSession("SessionInfo", objSessionInfo);
                new RCommon().SetSession("DefaultLoginPage", objUser.DefaultLoginPage);
                //sessionMgr.SetSession("DefaultLoginPage", objUser.DefaultLoginPage);
                com.ivp.rad.RSessionManager.RUserSessionManager objSessionManager = new com.ivp.rad.RSessionManager.RUserSessionManager();
                objSessionManager.InsertUserSession(objUserInfo.UserLoginName);
                RConfigReader.SetCurrentThemeName(objUserInfo.ThemeName);

                TopMenu.RefreshCache(objUser.UserName);
                
                string url = Convert.ToString(Context.Cache["PreviousUrl"]);
                if (secID != "")
                {
                    Context.Cache.Remove("RedirectData");
                    Response.Redirect("SMCreateUpdateSecurityNew.aspx?identifier=UpdateSecurityNew&pageType=View&secId=" + secID + "&mode=nw");
                    //Response.Redirect("SMSecurityDetails.aspx?secID=" + secID);
                }
                else if (!string.IsNullOrEmpty(url))
                {
                    Context.Cache.Remove("PreviousUrl");
                    Response.Redirect(url);
                }
                else
                {
                    //string redirectURL = "CommonUI/WorkflowStatus.aspx?identifier=WorkflowStatus";//SMUiUtils.GetDefaultLoginRedirectUrl(objUser.UserName, objUser.DefaultLoginPage);
                    string redirectURL = "/CommonUI/SRMPolarisDownstreamConfig.aspx";
                   Session["homeUrl"] = redirectURL;
                    //string[] arr = redirectURL.Split('?')[1].Split('&');
                    //foreach (string s in arr)
                    //{
                    //    if (s.ToLower().Contains("identifier"))
                    //    {
                    //        Session["homeIdentifier"] = s.Split('=')[1];
                    //        break;
                    //    }
                    //}
                  //  Response.Redirect("/CommonUI/SRMPolarisDownstreamConfig.aspx");
                }
                string key = SessionInfo.LoginName;
            }
            //}
            //else
            //{
            //    lblLicenceExpired.Visible = true;
            //    lblLicenceExpired.Text = RCustomMessages.GetLicenseMessage(RBaseLicenseManager.LicenseResult);
            //}

        }

        //public string GetApplicationModeForBrowser()
        //{
        //    //System.Web.HttpRequest request = new HttpRequest();
        //    System.Web.HttpBrowserCapabilities browser = Request.Browser;
        //    string mode = string.Empty;
        //    switch (browser.Browser.ToLower())
        //    {
        //        case "firefox":
        //        case "chrome":
        //            mode = "Lite"; break;
        //        default:
        //            mode = "Main";
        //            break;
        //    }
        //    return mode;
        //}


    }
}
