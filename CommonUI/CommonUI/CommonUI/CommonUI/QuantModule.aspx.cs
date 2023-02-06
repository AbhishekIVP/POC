using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.controls.neogrid;
using System.Data;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class QuantModule : BasePage
    {
        public string radGridClientId;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            radGridClientId = RADXLGridTest.ClientID;
            //BindGrid(RADXLGridTest);

            //List<SRMQuantFilterInfo> filterInfo = SRMQuantController.GetQuantIntellisenseData();
        }

        private static void BindGrid(RADNeoGrid xlGid)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("IDC", typeof(System.String));
            dt.Columns.Add("ColumnB", typeof(System.String));
            dt.Columns.Add("ColumnC", typeof(System.String));
            dt.Columns.Add("ColumnD", typeof(System.String));
            dt.Columns.Add("ColumnE", typeof(System.String));
            dt.Columns.Add("ColumnF", typeof(System.String));
            dt.Columns.Add("ColumnG", typeof(System.String));
            dt.Columns.Add("ColumnH", typeof(System.String));
            dt.Columns.Add("ColumnI", typeof(System.String));
            dt.Columns.Add("ColumnJ", typeof(System.String));
            dt.Columns.Add("ColumnK", typeof(System.String));
            dt.Columns.Add("ColumnL", typeof(System.String));
            dt.Columns.Add("ColumnM", typeof(System.String));
            dt.Columns.Add("ColumnN", typeof(System.String));
            dt.Columns.Add("ColumnO", typeof(System.String));
            dt.Columns.Add("ColumnP", typeof(System.String));


            for (int i = 0; i < 1000; i++)
            {
                DataRow dr = dt.NewRow();
                dr["IDC"] = "A" + Convert.ToString(i);
                dr["ColumnB"] = "B" + Convert.ToString(i);
                dr["ColumnC"] = "C" + Convert.ToString(i);
                dr["ColumnD"] = "D" + Convert.ToString(i);
                dr["ColumnE"] = "E" + Convert.ToString(i);
                dr["ColumnF"] = "F" + Convert.ToString(i);
                dr["ColumnG"] = "G" + Convert.ToString(i);
                dr["ColumnH"] = "H" + Convert.ToString(i);
                dr["ColumnI"] = "I" + Convert.ToString(i);
                dr["ColumnJ"] = "J" + Convert.ToString(i);
                dr["ColumnK"] = "K" + Convert.ToString(i);
                dr["ColumnL"] = "L" + Convert.ToString(i);
                dr["ColumnM"] = "M" + Convert.ToString(i);
                dr["ColumnN"] = "N" + Convert.ToString(i);
                dr["ColumnO"] = "O" + Convert.ToString(i);
                dr["ColumnP"] = "P" + Convert.ToString(i);
                dt.Rows.Add(dr);
            }

            ds.Tables.Add(dt);
            xlGid.IdColumnName = "IDC";
            xlGid.RequireHideColumns = true;
            xlGid.ViewKey = "test";
            xlGid.CurrentPageID = "test";
            xlGid.EmptyGridText = "Test this functionality";
            xlGid.DataSource = ds;
            //xlGid.LoadPreviousDataSource = true;
            //xlGid.RequireRADExtraPopup = true;
            xlGid.RequireSearch = true;
            //xlGid.RequireSelectedRows = true;
            //xlGid.RequireSliderFilter = false;
            xlGid.RequireSort = true;
            xlGid.RequireGrouping = true;
            //xlGid.RequireFullScreen = false;
            //xlGid.SelectRecordsAcrossAllPages = false;
            //xlGid.DateFormat = "M/d/yyyy";
            xlGid.RequireExportToExcel = true;
            xlGid.RequirePaging = false;
            xlGid.RequireInfiniteScroll = true;
            xlGid.DataBind();
        }
    }
}