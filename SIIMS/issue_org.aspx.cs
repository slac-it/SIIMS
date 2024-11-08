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
    public partial class issue_org : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;


        protected static readonly ILog Log = LogManager.GetLogger("issue_org");

        protected void Page_Load(object sender, EventArgs e)
        {

            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            Helpers _helper = new Helpers();
            if (!_helper.IsSMTLeader(_loginSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            string org_name;

            if (Request["orgID"] != null)
            {
                org_name = Request["smt"].ToString();
                lblTitle.Text = org_name + " Open Issues";
            }
            else
            {
                Response.Redirect("default.aspx", true);
            }

        }


        #region Variables
        string gvUniqueID = String.Empty;
        int gvNewPageIndex = 0;
        int gvEditIndex = -1;
        string gvSortExpr = String.Empty;
       
        #endregion

       

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
      

        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           
        }


      

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gvTemp = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string issueId = (gvTemp).DataKeys[e.Row.RowIndex].Value.ToString();

                Label lblStatusID = e.Row.FindControl("lblStatusID") as Label;

                string statusID = lblStatusID.Text;

                if (statusID == "20")  // this is draft
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueDraft") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }

                if (statusID == "21")  // this is waiting
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssuePending") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }

                if (statusID == "22")  // this is open
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueOpen") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }

                if (statusID == "23")  // this is Closed
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueClosed") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }
            }

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

        protected void ddlDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCurrentDropDownList = (DropDownList)sender;
            GridViewRow grdrDropDownRow = ((GridViewRow)ddlCurrentDropDownList.Parent.Parent);
            string Selected_id = ddlCurrentDropDownList.SelectedValue;
            string text = ddlCurrentDropDownList.SelectedItem.Text;
            Label lblIssueID = (Label)grdrDropDownRow.FindControl("lblIssueID");
            string issue_id = lblIssueID.Text;
            ddlCurrentDropDownList.SelectedValue = "-1";

            if (Selected_id == "1")  // Add Actions 
            {
                Response.Redirect("action.aspx?from=org&iid=" + issue_id);
            }
            if (Selected_id == "2")  // close issue
            {
                Response.Redirect("issue_close.aspx?from=org&iid=" + issue_id);
            }

            if (Selected_id == "3")  // edit issue
            {
                Response.Redirect("issue_edit.aspx?from=org&iid=" + issue_id);
            }

            if (Selected_id == "4")  // change level
            {
                Response.Redirect("issue_level.aspx?from=org&iid=" + issue_id);
            }

            if (Selected_id == "5")  // delete issue
            {
                Response.Redirect("issue_delete.aspx?from=org&iid=" + issue_id);
            }


        }

       

        protected void ddlActionInIssue_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCurrentDropDownList = (DropDownList)sender;
            GridViewRow grdrDropDownRow = ((GridViewRow)ddlCurrentDropDownList.Parent.Parent);
            string Selected_id = ddlCurrentDropDownList.SelectedValue;
            string text = ddlCurrentDropDownList.SelectedItem.Text;
            Label lblActionID = (Label)grdrDropDownRow.FindControl("lblActionID2");
            string action_id = lblActionID.Text;
            ddlCurrentDropDownList.SelectedValue = "-1";

            if (action_id.StartsWith("A"))
            {
                action_id = action_id.Remove(0, 1);
            }

            if (Selected_id == "1")  //  Close
            {
                Response.Redirect("action_close.aspx?type=c&aid=" + action_id, true);
            }

            if (Selected_id == "2")  //  Edit
            {
                Response.Redirect("action_edit.aspx?aid=" + action_id, true);
            }

            if (Selected_id == "3")  //  Change Owner
            {
                Response.Redirect("action_changeOwner.aspx?aid=" + action_id, true);
            }

            if (Selected_id == "4")  // Change Due date
            {
                Response.Redirect("action_changeDue.aspx?aid=" + action_id, true);
            }

            if (Selected_id == "5")  // Delete
            {
                Response.Redirect("action_delete.aspx?type=d&aid=" + action_id, true);
            }

            if (Selected_id == "12")
            {
                Response.Redirect("action.aspx?aid=" + action_id, true);
            }

            if (Selected_id == "15")
            {
                Response.Redirect("action.aspx?aid=" + action_id,true);
            }

            if (Selected_id == "22")
            {
                Response.Redirect("action_view.aspx?aid=" + action_id, true);
            }

        }
    }
}