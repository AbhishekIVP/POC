using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.utils;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using com.ivp.rad.dal;
using System.Text;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.controls.fileupload;
using System.IO;
using System.Xml.Linq;
using System.Web.UI.HtmlControls;
using System.Threading;
using com.ivp.rad.viewmanagement;

namespace ConfigMigrationUtilityUI
{
    public partial class RMUCConfigMigrationUtility : BaseUserControl
    {
        string errorMessage = string.Empty;
        static string DBConnectionID = RADConfigReader.GetConfigAppSettings("RefMDBConnectionId");
        RDBConnectionManager rDBConnManager; RHashlist hList;
        protected override void PageLoadEvent(object sender, EventArgs e)
        {
            rDBConnManager = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionID);
            hList = new RHashlist();

            DataTable InstanceTable = (DataTable)rDBConnManager.ExecuteQuery("SELECT Source_Instance,ServiceURL,Applications FROM [dbo].[ivp_rad_config_migration_utility]", RQueryType.Select).Tables[0];
            foreach (DataRow dr in InstanceTable.Rows)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes["class"] = "Option_Source";
                div.InnerText = dr["Source_Instance"].ToString();

                HtmlInputHidden hidden = new HtmlInputHidden();
                hidden.Value = !string.IsNullOrEmpty(Convert.ToString(dr["Applications"])) ? Convert.ToString(dr["Applications"]) + "|" + dr["ServiceURL"].ToString() : string.Empty;
                div.Controls.Add(hidden);

                Div_Option_Sources.Controls.Add(div);
            }

            hList = new RHashlist();
            DataSet ds = (DataSet)rDBConnManager.ExecuteQuery("SELECT Applications,Current_Instance FROM [dbo].[ivp_rad_config_migration_utility]", RQueryType.Select);
            String Applications = ds.Tables[0].Rows[0]["Applications"].ToString();
            String[] App = Applications.Split(',');

            int id = 1;
            if (App.Length > 1)
            {
                foreach (String str in App)
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.ID = str;
                    //div.Attributes["class"] = "Tabs-Panel";
                    ConfigTabs.Controls.Add(div);

                    HtmlGenericControl li = new HtmlGenericControl("div");
                    li.ID = "li_" + id;
                    li.Attributes["class"] = "tab-class";
                    li.InnerText = str;
                    tabsList.Controls.Add(li);

                    if (id % 2 != 0)
                    {
                        HtmlGenericControl mid = new HtmlGenericControl("div");
                        mid.Attributes["class"] = "tab-middle";
                        tabsList.Controls.Add(mid);
                    }
                    id++;
                }
            }
            else
                hdn_app.Value = "refmaster";
            Div_Destination.InnerText = ds.Tables[0].Rows[0]["Current_Instance"].ToString();

            string mod = !string.IsNullOrEmpty(this.Request.QueryString["module"]) ? this.Request.QueryString["module"] : "secmaster";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Alert", "GetTabs('" + mod + "');", true);
        }
    }

    internal class Data
    {
        private string instanceName;
        private string serviceURL;


        public string InstanceName
        {
            get { return instanceName; }
            set { value = instanceName; }
        }

        public string ServiceURL
        {
            get { return serviceURL; }
            set { value = serviceURL; }
        }
    }
}
