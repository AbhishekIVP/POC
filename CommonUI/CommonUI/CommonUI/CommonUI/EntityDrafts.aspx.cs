using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;
using com.ivp.secm.commons;
using System.Collections.Generic;
using com.ivp.rad.controls.xlgrid.client.info;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class EntityDrafts : BasePage
    {

        #region INFO AND CONTROL IDS TO BE SEND TO SCRIPTS
        private EntityDraftsStatusInfo GetSecurityDraftsStatusInfo()
        {
            EntityDraftsStatusInfo securityInfo = new EntityDraftsStatusInfo();
            securityInfo.isSecurityEditable = isSecurityEditable;
            securityInfo.isViewPrivilege = isViewPrivilege;
            return securityInfo;
        }


        #endregion
        #region ControlEvents
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            HtmlMeta meta1 = (HtmlMeta)this.Master.FindControl("meta1");
            if (meta1 != null)
                meta1.Content = "IE=EmulateIE8";
        }

        #region decleration

        string isSecurityEditable = "";
        bool isViewPrivilege = false;
        #endregion


        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Pages the load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>

        protected void Page_Load(object sender, EventArgs e)
        {
            //SMSearchController objSMSearch = new SMSearchController();
            //if (objSMSearch.CheckControlPrivilegeForUser("SecurityDrafts", "ctmEditSecurityDetails", SessionInfo.LoginName))
            //{
            //    isSecurityEditable = "true";
            //}
            //else if (objSMSearch.CheckControlPrivilegeForUser("SecurityDrafts", "ctmSecurityDetails", SessionInfo.LoginName))
            //{
            //    isViewPrivilege = true;
            //    isSecurityEditable = "false";
            //}
            if (String.IsNullOrEmpty(lblUniqueSessionIdentifier.Value))
                lblUniqueSessionIdentifier.Value = System.Guid.NewGuid().ToString();

            if (!IsPostBack)
            {
                if (this.Request.QueryString["dashboard"] != null)
                    ViewState["dashboard"] = this.Request.QueryString["dashboard"];
                if (this.Request.QueryString["secTypeIds"] != null)
                    ViewState["secTypeIds"] = this.Request.QueryString["secTypeIds"];
                if (this.Request.QueryString["isUserSpecific"] != null)
                    ViewState["isUserSpecific"] = this.Request.QueryString["isUserSpecific"];
                if (this.Request.QueryString["userName"] != null)
                    ViewState["userName"] = this.Request.QueryString["userName"];

                BindDraftsGrid();
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "EntityDrafts('" + serializer.Serialize(new { grid = xlGrid.ClientID, ContextMenu = ctmSecurityDrafts.ClientID, View = ctmSecurityDraftsView.ID, Discard = ctmSecurityDraftsDiscard.ID, hiddenSecId = hiddenSecID.ClientID, btnPageRefresh = btnPageRefresh.ClientID }) + "','" + serializer.Serialize(GetSecurityDraftsStatusInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "Ids", script, true);
            base.OnPreRender(e);
            SetScriptControl();
            //     ScriptManager.GetCurrent(Page).RegisterScriptControl(this);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                base.Render(writer);
                base.ConfigureControls(ctmSecurityDrafts);
                //ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(this);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the drafts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ctmSecurityDrafts_Handler(object sender, EventArgs e)
        {
            //SMSecurityController objSecurityController = new SMSecurityController();
            //SMSecurityInfo objSecurityInfo = null;

            //string secIds = hiddenSecID.Value;
            //string[] secIdArr = secIds.Split('ž');

            //try
            //{
            //    foreach(string secId in secIdArr)
            //    {
            //        if (!string.IsNullOrEmpty(secId))
            //        {
            //            objSecurityInfo = new SMSecurityInfo();
            //            objSecurityInfo.SecID = secId;
            //            objSecurityInfo.CreatedBy = SessionInfo.LoginName;
            //            objSecurityController.DeleteSecurityDrafts(objSecurityInfo);
            //        }
            //    }
            //    modalSave.Show();
            //}
            //catch (Exception ex)
            //{
            //    lblDeleteError.Text = ex.Message;
            //    modalError.Show();
            //}
            BindDraftsGrid();
        }

        #endregion

        #region Private Methods

        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Binds the drafts grid.
        /// </summary>
        private void BindDraftsGrid()
        {
            ctmSecurityDrafts.TargetControlId = xlGrid.ClientID;
            ctmSecurityDrafts.TargetControlTagName = "DIV";

            xlGrid.ClearSerializationData = true;
            xlGrid.UserID = SessionInfo.LoginName;//GRID Cache Key
            xlGrid.SessionIdentifier = lblUniqueSessionIdentifier.Value;
            xlGrid.CurrentPageID = CurrentPageId;//GRID Cache Key
            xlGrid.IdColumnName = "entity_code";

            xlGrid.ViewKey = "EntityDrafts_" + CurrentPageId + SessionInfo.LoginName;

            xlGrid.Visible = false;
            xlGrid.PageSize = 200;
            xlGrid.RequireResizing = true;
            xlGrid.RequirePaging = true;
            xlGrid.HideFooter = false;
            xlGrid.DoNotExpand = false;
            xlGrid.ItemText = "Entity Drafts";
            xlGrid.DoNotRearrangeColumn = true;
            xlGrid.RequireGrouping = true;
            xlGrid.RequireFilter = true;
            xlGrid.RequireSort = true;
            xlGrid.RequireMathematicalOperations = false;
            xlGrid.RequireSelectedRows = true;
            xlGrid.RequireEditGrid = false;
            xlGrid.RequireExportToExcel = true;
            xlGrid.RequireSearch = true;
            xlGrid.RequireFreezeColumns = false;
            xlGrid.RequireGroupExpandCollapse = true;
            xlGrid.RequireLayouts = false;
            xlGrid.AutoAdjust = AutoAdjust.HeaderAndBody;
            xlGrid.DateFormat = SessionInfo.CultureInfo.LongDateFormat;
            xlGrid.ExcelSheetName = "EntityDrafts";
            xlGrid.ColumnsToHide = new List<HiddenColumnInfo>() { new HiddenColumnInfo() { ColumnName = "entity_type_id" }, new HiddenColumnInfo() { ColumnName = "template_id" }, new HiddenColumnInfo() { ColumnName = "sectype_description" }, new HiddenColumnInfo() { ColumnName = "row_keys" } };

            xlGrid.ColumnDateFormatMapping.Add("Effective Date", SessionInfo.CultureInfo.ShortDateFormat);

            xlGrid.CheckBoxInfo = new CheckBoxInfo { CheckBoxSelectionMode = CheckBoxSelectionMode.Multiple };
            xlGrid.KeyColumns.Add(rad.controls.xlgrid.KeyType.RowKey, "row_keys");

            xlGrid.RaiseGridRenderComplete = "resizeGridFinal('" + xlGrid.ClientID + "','" + topPanel.ClientID + "','" + middlePanel.ClientID + "','',0,0);";

            try
            {
                //SMSecurityInfo objSMSecurityInfo = new SMSecurityInfo();
                //objSMSecurityInfo.CreatedBy = SessionInfo.LoginName;
                //objSMSecurityInfo.DraftType = DraftType.Manual;
                //if (ViewState["dashboard"] != null)
                //{
                //    if (ViewState["secTypeIds"] != null)
                //    {
                //        objSMSecurityInfo.SecTypeIdList = Convert.ToString(ViewState["secTypeIds"]);
                //        ViewState.Remove("secTypeIds");
                //    }

                //    if (ViewState["isUserSpecific"] != null)
                //    {
                //        objSMSecurityInfo.IsUserSpecific = Convert.ToBoolean(ViewState["isUserSpecific"]);
                //        ViewState.Remove("isUserSpecific");
                //    }

                //    if (ViewState["userName"] != null)
                //    {
                //        objSMSecurityInfo.UserName = Convert.ToString(ViewState["userName"]);
                //        ViewState.Remove("userName");
                //    }
                //    ViewState.Remove("dashboard");
                //}
                DataSet dsDrafts = new DataSet(); //new SMSecurityController().GetDrafts(objSMSecurityInfo);
                dsDrafts.Tables[0].TableName = "Table";
                ViewState["DTDRAFTS"] = dsDrafts.Tables[0];

                DataSet dsTemp = new DataSet("ROOT");
                dsTemp.Tables.Add(dsDrafts.Tables[0].Copy());
                dsTemp.Tables[0].Columns["entity_type_name"].ColumnName = "Entity Type";

                dsTemp.Tables[0].Columns.Add("row_keys");
                dsTemp.Tables[0].Columns.Add("Entity Code", typeof(string));
                dsTemp.Tables[0].Columns["Entity Type"].SetOrdinal(0);
                dsTemp.Tables[0].Columns["Entity Code"].SetOrdinal(1);

                xlGrid.CustomRowsDataInfo = GetCustomRowDataInfo(dsTemp.Tables[0]);

                if (dsDrafts.Tables[0] != null && dsDrafts.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsTemp.Tables[0].Rows)
                    {
                        dr["Entity Code"] = dr["entity_code"];
                    }

                    Div_NoResult.Style.Add("display", "none");
                    Div_Grid.Style.Add("display", "block");
                    xlGrid.Visible = true;
                    xlGrid.DataSource = dsTemp.Tables[0];
                    xlGrid.DataBind();
                }

                else
                {
                    Div_NoResult.Style.Add("display", "inline-block");
                    Div_Grid.Style.Add("display", "none");
                }
            }
            catch (Exception ex)
            { throw ex; }

        }

        private List<CustomRowDataInfo> GetCustomRowDataInfo(DataTable dt)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            if (dt != null && dt.Rows.Count > 0)
            {
                lstCustomRowDataInfo = new List<CustomRowDataInfo>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var v = dt.Rows[i]["Effective Date"] is System.DBNull ? Convert.ToString(dt.Rows[i]["entity_code"]) : Convert.ToString(dt.Rows[i]["entity_code"]) + "¦" + Convert.ToDateTime(dt.Rows[i]["Effective Date"]).ToString("yyyyMMdd");

                    CustomRowDataInfo objCustomRowDataInfo = new CustomRowDataInfo();

                    CustomCellDatainfo cellSecurityId = new CustomCellDatainfo();
                    cellSecurityId.Append = false;
                    cellSecurityId.ColumnName = "Entity Code";

                    if (((isSecurityEditable != string.Empty) && (isSecurityEditable == "true")) || (isViewPrivilege == true))
                    {
                        if (!Convert.ToString(GetText(dt.Rows[i]["entity_code"])).Equals("&nbsp;"))
                            cellSecurityId.NewChild = "<span sec_id=" + GetText(dt.Rows[i]["entity_code"]) + " id=\"lblSecId_" + GetText(dt.Rows[i]["entity_code"]) + "\" style='text-decoration:underline;cursor:pointer;color:#48a3dd;' >" + Convert.ToString(GetText(dt.Rows[i]["entity_code"])) + "</span>";
                        else
                        { }
                    }
                    else
                        cellSecurityId.NewChild = "<span sec_id=" + GetText(dt.Rows[i]["entity_code"]) + " id=\"lblSecId_" + GetText(dt.Rows[i]["entity_code"]) + "\">" + GetText(dt.Rows[i]["entity_code"]) + "</span>";
                    objCustomRowDataInfo.Cells.Add(cellSecurityId);

                    objCustomRowDataInfo.RowID = Convert.ToString(dt.Rows[i]["entity_code"]);
                    dt.Rows[i]["row_keys"] = v;

                    objCustomRowDataInfo.Attribute.Add("sec_id", v);
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }

        private void SetScriptControl()
        {
            sccSecurityDrafts.DescriptorID = "EntityDraftsHelper";
            sccSecurityDrafts.TargetClientComponent = "SecMasterJSPage.EntityDraftsHelper";
            sccSecurityDrafts.ScriptPath = new List<string> { "~/includes/EntityDraftsInfo.js", "~/includes/EntityDrafts.js" };

        }
        private string GetText(object data)
        {
            string stringData = Convert.ToString(data);
            if (string.IsNullOrWhiteSpace(stringData))
                return "&nbsp;";
            else
                return stringData;
        }
        #endregion
    }
    public class EntityDraftsStatusInfo
    {
        public string isSecurityEditable { get; set; }
        public bool isViewPrivilege { get; set; }
    }
}