using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using SIIMS.App_Code;

namespace SIIMS
{
    public partial class team_issue : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        protected static readonly ILog Log = LogManager.GetLogger("team_issue");

        protected void Page_Load(object sender, EventArgs e)
        {

            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);

        }


        #region Variables
        string gvUniqueID = String.Empty;
        int gvNewPageIndex = 0;
        int gvEditIndex = -1;
        string gvSortExpr = String.Empty;
        private string gvSortDir
        {

            get { return ViewState["SortDirection"] as string ?? "ASC"; }

            set { ViewState["SortDirection"] = value; }

        }
        #endregion

        //This procedure returns the Sort Direction
        private string GetSortDirection()
        {
            switch (gvSortDir)
            {
                case "ASC":
                    gvSortDir = "DESC";
                    break;

                case "DESC":
                    gvSortDir = "ASC";
                    break;
            }
            return gvSortDir;
        }

        //This procedure prepares the query to bind the child GridView
        private SqlDataSource ChildDataSource(string strCustometId, string strSort)
        {

            string strQRY = "";
            SqlDataSource dsTemp = new SqlDataSource();
            dsTemp.ConnectionString = _connStr;
            dsTemp.ProviderName = "Oracle.ManagedDataAccess.Client";
            dsTemp.DataSourceMode = SqlDataSourceMode.DataSet;
            strQRY = "SELECT action.issue_id,action_id, 'A' || action.action_id  as action_id2 , action.title as ActTitle, to_char( action.due_date, 'MM/DD/YYYY') due_date, sta.status_id, sta.status, p.name as owner " +
                     " from siims_action action join siims_status sta on action.status_id=sta.status_id and object_id=2 left join persons.person p " +
                     " on action.owner_sid=p.key where  action.is_active='Y' and action.issue_id='" + strCustometId + "' order by action.created_on desc ";


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

            //Check if any additional conditions (Paging, Sorting, Editing, etc) to be applied on child GridView
            if (gv.UniqueID == gvUniqueID)
            {
                gv.PageIndex = gvNewPageIndex;
                gv.EditIndex = gvEditIndex;
                //Check if Sorting used
                if (gvSortExpr != string.Empty)
                {
                    GetSortDirection();
                    strSort = " ORDER BY " + string.Format("{0} {1}", gvSortExpr, gvSortDir);
                }
                //Expand the Child grid
                ClientScript.RegisterStartupScript(GetType(), "Expand", "<SCRIPT LANGUAGE='javascript'>expandcollapse('div" + ((DataRowView)e.Row.DataItem)["ISSUE_ID"].ToString() + "','one');</script>");
            }

            //Prepare the query for Child GridView by passing the Customer ID of the parent row
            gv.DataSource = ChildDataSource(((DataRowView)e.Row.DataItem)["ISSUE_ID"].ToString(), strSort);
            gv.DataBind();


        }

        //This event occurs for any operation on the row of the grid
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        #endregion

        #region GridView2 Event Handlers
        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gvTemp = (GridView)sender;
            gvUniqueID = gvTemp.UniqueID;
            gvNewPageIndex = e.NewPageIndex;
            GridView1.DataBind();
        }

        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddAction")
            {

            }
        }


        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Check if this is our Blank Row being databound, if so make the row invisible
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem)["Action_ID"].ToString() == String.Empty) e.Row.Visible = false;
            }
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView gvTemp = (GridView)sender;
            gvUniqueID = gvTemp.UniqueID;
            gvSortExpr = e.SortExpression;
            GridView1.DataBind();
        }
        #endregion

        public string FORMATACTIONStatus(string status, string due_date)
        {
            string msg = "";
            if (status == "Open")
            {
                msg = due_date;
            }
            else
            {
                msg = status;
            }

            return msg;
        }
    }
}