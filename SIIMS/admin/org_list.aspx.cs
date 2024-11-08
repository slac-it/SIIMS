using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using log4net;

namespace SIIMS.admin
{
    public partial class org_list : System.Web.UI.Page
    {
        string _connStr;
        protected static readonly ILog Log = LogManager.GetLogger("org_list");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            // make sure the login is Admin
            if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            {
                Response.Redirect("../permission.aspx");
            }
        }

        #region Variables
        string gvUniqueID = String.Empty;
        int gvNewPageIndex = 0;
        int gvEditIndex = -1;

        #endregion

        //This procedure prepares the query to bind the child GridView
        private SqlDataSource ChildDataSource(string strCustometId, string strSort)
        {
            string strQRY = "";
            SqlDataSource dsTemp = new SqlDataSource();
            dsTemp.ConnectionString = _connStr;
            dsTemp.ProviderName = "Oracle.ManagedDataAccess.Client";
            dsTemp.DataSourceMode = SqlDataSourceMode.DataSet;
            strQRY = "select dlg.org_id, dlg.delegate_id, dlg.sid as delegate_sid, poc.name as delegate_name from siims_smt_delegate dlg join persons.person poc on poc.key=dlg.sid " +
                " where dlg.is_active='Y' and dlg.org_id='" + strCustometId + "' order by poc.name" ;

            dsTemp.SelectCommand = strQRY;
            return dsTemp;
        }

        #region GridView1 Event Handlers
        //This event occurs for each row
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            string strSort = string.Empty;

            // Make sure we aren't in header/footer rows
            if (row.DataItem == null)
            {
                return;
            }

            //Find Child GridView control
            GridView gv = new GridView();
            gv = (GridView)row.FindControl("GridView2");

            string orgID=((DataRowView)e.Row.DataItem)["org_id"].ToString();

            //Check if any additional conditions (Paging, Sorting, Editing, etc) to be applied on child GridView
            if (gv.UniqueID == gvUniqueID)
            {
                gv.PageIndex = gvNewPageIndex;
                gv.EditIndex = gvEditIndex;

                //Expand the Child grid
                ClientScript.RegisterStartupScript(GetType(), "Expand", "<SCRIPT LANGUAGE='javascript'>expandcollapse('div" + orgID + "','one');</script>");
            }

            //Prepare the query for Child GridView by passing the Customer ID of the parent row
            gv.DataSource = ChildDataSource(orgID, strSort);
            gv.DataBind();

        }




        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gvTemp = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string orgID = (gvTemp).DataKeys[e.Row.RowIndex].Value.ToString();

            }


        }

        #endregion
    }
}