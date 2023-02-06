using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using com.ivp.rad.utils;
using com.ivp.refmaster.common;
using System.Xml;
using System.IO;
using System.Globalization;
//using com.ivp.secm.api.Info;
using com.ivp.rad.viewmanagement;
using com.ivp.secm.ui;
using com.ivp.common.exceptionManagerUI;

namespace Reconciliation.BaseUserControls.Service
{
    /// <summary>
    /// Summary description for CommonService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class CommonService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void SetValuesForException(string secIds, string sessionIdentifierKey, FilterInfoForException objFilterExceptionInfo)
        {
            try
            {
                FilterInfoForException objFilterInfoForException = null;
                if (objFilterExceptionInfo == null)
                {
                    //com.ivp.secm.exceptions.SMExceptionFilterInfo objSMExceptionFilterInfo = new com.ivp.secm.exceptions.SMExceptionFilterInfo();

                    if (!string.IsNullOrEmpty(secIds) && secIds.Split(',').Length > 0)
                    {
                        objFilterInfoForException = new FilterInfoForException();
                        objFilterInfoForException.SecurityIds = secIds;
                        objFilterInfoForException.EntityCodes = secIds;
                        // objFilterInfoForException.ExceptionSectype = sectypeIds;
                        objFilterInfoForException.ExceptionAttribute = "-1";
                        objFilterInfoForException.ExceptionDate = ((int)SMExceptionDateType.AnyTime).ToString();
                        objFilterInfoForException.CustomDateSelected = "0";
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Unresolved | (int)SMExceptionFilterType.UnSuppressed).ToString();
                        objFilterInfoForException.ExceptionType = "-1";
                        objFilterInfoForException.DetailsType = ((int)(SMExceptionDetailType.Details)).ToString();
                        objFilterInfoForException.ShowFilter = true;
                        objFilterInfoForException.TagName = "-1";
                        objFilterInfoForException.ExceptionState = "5";
                        //Added ModuleId
                        objFilterInfoForException.ModuleId = objFilterExceptionInfo.ModuleId;
                    }
                }
                else
                {
                    objFilterInfoForException = new FilterInfoForException();
                    objFilterInfoForException.ExceptionSectype = objFilterExceptionInfo.ExceptionSectype;
                    objFilterInfoForException.ExceptionDate = objFilterExceptionInfo.ExceptionDate;
                    objFilterInfoForException.CustomDateSelected = objFilterExceptionInfo.CustomDateSelected;
                    if(objFilterExceptionInfo.ExceptionFilter == "1")
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Unresolved | (int)SMExceptionFilterType.UnSuppressed).ToString();
                    else if (objFilterExceptionInfo.ExceptionFilter == "2")
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Resolved).ToString();
                    else if (objFilterExceptionInfo.ExceptionFilter == "3")
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Unresolved).ToString();
                    else if (objFilterExceptionInfo.ExceptionFilter == "4")
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Suppressed).ToString();
                    else if (objFilterExceptionInfo.ExceptionFilter == "5")
                        objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.UnSuppressed).ToString();

                    //objFilterInfoForException.ExceptionFilter = ((int)SMExceptionFilterType.Unresolved | (int)SMExceptionFilterType.UnSuppressed).ToString();
                    objFilterInfoForException.ExceptionType = objFilterExceptionInfo.ExceptionType;
                    objFilterInfoForException.DetailsType = ((int)(SMExceptionDetailType.Details)).ToString();
                    objFilterInfoForException.ShowFilter = objFilterExceptionInfo.ShowFilter;
                    objFilterInfoForException.StartDate = objFilterExceptionInfo.StartDate;
                    objFilterInfoForException.EndDate = objFilterExceptionInfo.EndDate;
                    objFilterInfoForException.ExceptionAttribute = objFilterExceptionInfo.ExceptionAttribute;
                    objFilterInfoForException.UserName = objFilterExceptionInfo.UserName;
                    objFilterInfoForException.TagName = objFilterExceptionInfo.TagName;
                    objFilterInfoForException.ExceptionEntityType = objFilterExceptionInfo.ExceptionEntityType;
                    objFilterInfoForException.ExceptionState = objFilterExceptionInfo.ExceptionState;
                    //Added ModuleId
                    objFilterInfoForException.ModuleId = objFilterExceptionInfo.ModuleId;

                }
                Session[sessionIdentifierKey] = objFilterInfoForException;
            }

            catch (Exception Ex)
            {
            }
        }

        [WebMethod]
        public bool CompareDate(string startDate, string endDate, bool setServerDate = false)
        {
            if (setServerDate)
                endDate = DateTime.Now.ToString();
            DateTime startDt, endDt;
            int result = 0;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDt = Convert.ToDateTime(startDate);
                endDt = Convert.ToDateTime(endDate);
                result = DateTime.Compare(startDt, endDt);
            }
            return ((result > 0) ? true : false);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetServerDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
        }
    }
}
