using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using log4net;
using System.Configuration;
using SIIMS.App_Code;

namespace SIIMS
{
    public partial class _default : System.Web.UI.Page
    {
    
        int _userSID;

        string _connStr;

        protected static readonly ILog Log = LogManager.GetLogger("_default");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            // login SSO info
            Log.Debug("Hello");

            string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];
            getSSOSID();


            // end login SSO info


            //getUserSID();


            if (Session["LoginSID"] != null)
            {
                if (Session["LoginSID"].ToString() != "")
                {
                    _userSID = int.Parse(Session["LoginSID"].ToString());
                }
                else
                {
                    Response.Redirect("default.aspx");
                }
            }
            else
            {
                Response.Redirect("default.aspx");
            }


            Helpers _helper = new Helpers();
            bool _isPoc = _helper.IsSMTLeader(_userSID);
            //int  _num_Issues = _helper.Num_OpenIssues(_userSID);
            //int _num_Actions = _helper.Num_OpenActions(_userSID);      
            int _num_Reports = _helper.Num_Reports(_userSID);
            if (_num_Reports > 0)
            {

                linkMyTeamIssue.Visible = true;
                linkMyTeamAction.Visible = true;
            }
             
            //if (_isPoc)
            //{
            //    setUpPOCLinks();
            //}
           
     
        }

        public string GetOrgLink(string org_id, string org_name)
        {
            string linkText = "<a href='issue_org.aspx?orgID=" + org_id + "&smt=" + HttpUtility.UrlEncode(org_name) + "'>" + org_name + " Open Issues" + "</a>";
            return linkText;
        }

        //private void setUpPOCLinks()
        //{
        //    string _sql = "select org_id,NAME from siims_org where IS_ACTIVE='Y' and (poc_sid=:loginSID or manager_sid=:loginSID ) " +
        //         " or exists (select org_id from SIIMS_SMT_DELEGATE where IS_ACTIVE='Y' and ORG_ID=siims_org.org_id and sid=:loginSID)";

        //    try
        //    {
        //        OracleConnection con = new OracleConnection();
        //        con.ConnectionString = _connStr;
        //        con.Open();

        //        OracleCommand cmd = con.CreateCommand();
        //        cmd.CommandText = _sql;
        //        cmd.BindByName = true;
        //        cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _userSID;

        //        OracleDataReader reader = cmd.ExecuteReader();
        //        int iCount = 0;
        //        while (reader.Read())
        //        {
        //            int org_id=reader.GetInt32(0);
        //            string org_name=reader.GetString(1);
        //            if (iCount == 0)
        //            {
        //                linkOrg1.NavigateUrl = "issue_org.aspx?orgID=" + org_id + "&smt=" + HttpUtility.UrlEncode(org_name);
        //                linkOrg1.Text=org_name + " Open Issues";
        //                linkOrg1.Visible = true;
        //                linkOrg2.Visible = false;
        //            }
        //            else if(iCount == 1)
        //            {
        //                linkOrg2.NavigateUrl = "issue_org.aspx?orgID=" + org_id + "&smt=" + HttpUtility.UrlEncode(org_name);
        //                linkOrg2.Text=org_name + " Open Issues";
        //                linkOrg2.Visible = true;
        //            }

        //            iCount++;
        //        }

        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("IsSMTLeader", ex);
        //    }
        //}


        private void getSSOSID()
        {
            var _attSid = Request.ServerVariables["SSO_SID"];
            Response.Write(@"SID: " + _attSid + "  | ");
            //Log.Debug("Get SID:" + _attSid);
            if (_attSid != null)
            {
                if (_attSid.IndexOf(";") != -1)
                {
                    _attSid = _attSid.Substring(0, _attSid.IndexOf(";"));
                }
                else
                {
                    Session["LoginSID"] = _attSid;
                }
            }
            else
            {
                Session["LoginSID"] = "";
            }

            var loginName = Request.ServerVariables["SSO_UPN"];
            Response.Write(@"Login Name: " + loginName + "  | ");
            if (loginName != null)
            {
                if (loginName.IndexOf(";") != -1)
                {
                    loginName = loginName.Substring(0, loginName.IndexOf(";"));
                    if (loginName.IndexOf("@") != -1)
                    {
                        Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
                    }
                    else
                    {
                        Session["LoginName"] = loginName;
                    }
                }
                else
                {
                    if (loginName.IndexOf("@") != -1)
                    {
                        Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
                    }
                    else
                    {
                        Session["LoginName"] = loginName;
                    }

                }
            }
            else
            {
                Session["LoginName"] = "None";
            }

            var loginEmail = Request.ServerVariables["SSO_EMAIL"];
            Response.Write(@"Email: " + loginEmail);
            if (loginEmail != null)
            {
                if (loginEmail.IndexOf(";") != -1)
                {
                    loginEmail = loginEmail.Substring(0, loginEmail.IndexOf(";"));
                    Session["LoginEmail"] = loginEmail;
                }
                else
                {
                    Session["LoginEmail"] = loginEmail;
                }
            }
            else
            {
                Session["LoginEmail"] = "None";
            }

            //Session["IS_OWNER"] = "1";
            getIsOwner(_attSid);
        }


        private void getIsOwner(string oSID)
        {
            //string ownerSID = Session["LoginSID"].ToString();
            string ownerSID = oSID;

            string _sql = @"select count(*) as isOwner from siims_rir_reviewer where reviewer_sid=" + ownerSID + @" and is_active='Y' and is_owner='Y'";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Session["IS_OWNER"] = reader.GetInt32(0).ToString();
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Is Owner for login=" + Session["LoginName"].ToString(), ex);
            }

        }





        private void getUserSID()
        {
            string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];

            string _loginName = HttpContext.Current.Request.ServerVariables["LOGON_USER"];
            string _UserWinName = _loginName.Substring(_loginName.LastIndexOf("\\") + 1);


            if (_isUAT == "1")
            {
                if (Request["user"] != null && Request["user"] != "")
                {
                    winLogin2SID(Request["user"]);
                }
                else
                {
                    winLogin2SID(_UserWinName);
                }

            }
            else
            {
                winLogin2SID(_UserWinName);
            }

        }

        public void winLogin2SID(string winlogin)
        {

            string _sql = @"SELECT b.but_sid,INITCAP(p.fname) as fname,NVL(NVL(maildisp,sid_email),' ') as email,(select count(*) from siims_rir_reviewer where reviewer_sid=b.but_sid and is_active='Y' and is_owner='Y') as is_owner
                            FROM but b join  persons.person p on b.but_sid = p.key and b.But_ldt = 'win'
                                where p.gonet = 'ACTIVE'  and b.BUT_LID = lower(:Login)";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":Login", OracleDbType.Varchar2).Value = winlogin;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    Session["LoginSID"] = reader.GetInt32(0).ToString();
                    Session["LoginName"] = reader.GetString(1);
                    Session["LoginEmail"] = reader.GetString(2)==null?"": reader.GetString(2);
                    Session["IS_OWNER"] = reader.GetInt32(3).ToString();
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("winLogin2SID for login=" + winlogin, ex);
            }
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
            strQRY = "SELECT action.issue_id,action_id, 'A' || action.action_id  as action_id2 , action.title as ActTitle, to_char( action.due_date, 'MM/DD/YYYY') due_date, sta.status_id, sta.status, p.name as owner, NVL(action.is_SIIMS,'Y') as is_siims " +
                     " from siims_action action join siims_status sta on action.status_id=sta.status_id and object_id=2 left join persons.person p " +
                     " on action.owner_sid=p.key where  action.is_active='Y' and action.issue_id='" + strCustometId + "' order by action.created_on desc " ;
         

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

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
          
                string issueId = ((GridView)sender).DataKeys[e.Row.RowIndex].Value.ToString();

                Label lbl_Sub115 = e.Row.FindControl("lblSub115") as Label;
                string is_Sub115 = lbl_Sub115.Text;

                Label lblIS_RIR = e.Row.FindControl("lblIS_RIR") as Label;
                string is_rir = lblIS_RIR.Text;

                if(is_Sub115=="Y")
                {
                    DropDownList ddlIssueTodo = e.Row.FindControl("drwIssueP1Closure") as DropDownList;

                    ddlIssueTodo.Visible = true;
                } else
                {
                    if (is_rir == "Y")
                    {
                        DropDownList ddlIssueTodo = e.Row.FindControl("drwIssueRIR") as DropDownList;

                        ddlIssueTodo.Visible = true;
                    }
                    else
                    {
                        DropDownList ddlIssueTodo = e.Row.FindControl("drwIssueSIIMS") as DropDownList;
                        ddlIssueTodo.Visible = true;
                    }
                }

              
               

            }


        }

      


        #endregion


        #region GridView2 Event Handlers
        //protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    GridView gvTemp = (GridView)sender;
        //    gvUniqueID = gvTemp.UniqueID;
        //    gvNewPageIndex = e.NewPageIndex;
        //    GridView1.DataBind();
        //}

    

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gvTemp = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string issueId = (gvTemp).DataKeys[e.Row.RowIndex].Value.ToString();

                Label lblStatusID = e.Row.FindControl("lblStatusID") as Label;

                Label lbl_IS_SIIMS = e.Row.FindControl("lblIS_SIIMS") as Label;

                string statusID = lblStatusID.Text;
                string is_siims = lbl_IS_SIIMS.Text;
              

                if (statusID == "20")  // this is draft
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueDraft") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }

                if (statusID == "21")  // this is waiting
                {
                    if(is_siims=="Y")
                    {
                        DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssuePending") as DropDownList;
                        ddlActionsInIssue.Visible = true;
                    } else
                    {
                        DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueClosed") as DropDownList;
                        ddlActionsInIssue.Visible = true;
                    }

                }

                if (statusID == "22")  // this is open
                {
                    if (is_siims == "Y")
                    {
                        DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueOpen") as DropDownList;
                        ddlActionsInIssue.Visible = true;
                    }
                    else
                    {
                        DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueOpenRIR") as DropDownList;
                        ddlActionsInIssue.Visible = true;
                    }

                }

                if (statusID == "23")  // this is Closed
                {
                    DropDownList ddlActionsInIssue = e.Row.FindControl("drwActionInIssueClosed") as DropDownList;
                    ddlActionsInIssue.Visible = true;
                }


                //DropDownList ddlTeams = e.Row.FindControl("drwActionInIssue") as DropDownList;
                //ddlTeams.DataSource = TeamMember.TeamsUserAllocatedTo(issueId);
                //ddlTeams.DataBind();
            }

            //Check if this is our Blank Row being databound, if so make the row invisible
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (((DataRowView)e.Row.DataItem)["Action_ID"].ToString() == String.Empty) e.Row.Visible = false;
            //}
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView gvTemp = (GridView)sender;
            gvUniqueID = gvTemp.UniqueID;
            gvSortExpr = e.SortExpression;
            GridView1.DataBind();
        }
        #endregion



        public string FormatActionEdit(string act_id)
        {
            string action = "";

            action = "<a href=action_edit.aspx?id=" + act_id + ">Edit </a>";

            return action;
        }

       


        public string FORMATACTIONStatus(string status, string due_date)
        {
            string msg = "";
            if (status == "Open")
            {
                msg = FormatDueDate(due_date);
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

            if(Selected_id=="1")  // Add Actions 
            {
                Response.Redirect("action.aspx?iid=" +issue_id);
            }
             if(Selected_id=="2")  // close issue
            {
                Response.Redirect("issue_close.aspx?iid=" +issue_id);
            }

             if(Selected_id=="3")  // edit issue
            {
                Response.Redirect("issue_edit.aspx?iid=" +issue_id);
            }

             if(Selected_id=="4")  // change level
            {
                Response.Redirect("issue_level.aspx?iid=" +issue_id);
            }

              if(Selected_id=="5")  // delete issue
            {
                Response.Redirect("issue_delete.aspx?iid=" + issue_id);
            }

            if (Selected_id == "115")  // For P1 Closure Requested issue
            {
                Response.Redirect("issue_view.aspx?iid=" + issue_id);
            }

        }

        protected void ddlActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlCurrentDropDownList = (DropDownList)sender;
            GridViewRow grdrDropDownRow = ((GridViewRow)ddlCurrentDropDownList.Parent.Parent);
            string Selected_id = ddlCurrentDropDownList.SelectedValue;
            string text = ddlCurrentDropDownList.SelectedItem.Text;
            Label lblActionID = (Label)grdrDropDownRow.FindControl("lblActionID");
            string action_id = lblActionID.Text;
            ddlCurrentDropDownList.SelectedValue = "-1";

            if (action_id.StartsWith("A"))
            {
                action_id=action_id.Remove(0, 1);
            }

            if (Selected_id == "1")  //  Close
            {
                Response.Redirect("action_close.aspx?type=c&aid=" + action_id);
            }

            if (Selected_id == "2")  //  Edit
            {
                Response.Redirect("action_edit.aspx?type=e&aid=" + action_id);
            }

            if (Selected_id == "3")  // Change Due date
            {
                Response.Redirect("action_changeDue.aspx?aid=" + action_id);
            }

            if (Selected_id == "4")  // Delete
            {
                Response.Redirect("action_delete.aspx?type=d&aid=" + action_id);
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
                Response.Redirect("action_close.aspx?type=c&aid=" + action_id);
            }

            if (Selected_id == "2")  //  Edit
            {
                Response.Redirect("action_edit.aspx?aid=" + action_id);
            }

            if (Selected_id == "3")  //  Change Owner
            {
                Response.Redirect("action_changeOwner.aspx?aid=" + action_id);
            }

            if (Selected_id == "4")  // Change Due date
            {
                Response.Redirect("action_changeDue.aspx?aid=" + action_id);
            }

            if (Selected_id == "5")  // Delete
            {
                Response.Redirect("action_delete.aspx?type=d&aid=" + action_id);
            }

            if (Selected_id == "12" )
            {

                Response.Redirect("action.aspx?aid=" + action_id);
            }

            if ( Selected_id == "15")
            {
                Response.Redirect("action.aspx?aid=" + action_id);
            }

            if (Selected_id == "22")
            {
                Response.Redirect("action_view.aspx?aid=" + action_id);
            }

        }

        #region GDActions Event Handlers
        //This event occurs for each row
        protected void GVActions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            string strSort = string.Empty;

            // Make sure we aren't in header/footer rows
            if (row.DataItem == null)
            {
                return;
            }


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string userId = ((GridView)sender).DataKeys[e.Row.RowIndex].Value.ToString();

                Label lbl_IS_SIIMS = e.Row.FindControl("lblSIIMS") as Label;
                string is_siims = lbl_IS_SIIMS.Text;
                if(is_siims=="Y")
                {
                    DropDownList ddlActions = e.Row.FindControl("drwActionSIIMSOpen") as DropDownList;
                    ddlActions.Visible = true;
                } else
                {
                    DropDownList ddlActions = e.Row.FindControl("drwActionRIROpen") as DropDownList;
                    ddlActions.Visible = true;
                }
               
                // bind the dropdown here if needed
                //ddlTeams.DataSource = TeamMember.TeamsUserAllocatedTo(userId);
                //ddlTeams.DataBind();
            }


        }
        #endregion

        public string FormatDueDate(string _dueDate)
        {
            if (string.IsNullOrEmpty(_dueDate))
            {
                return _dueDate;
            }
            DateTime dueDate = DateTime.Parse(_dueDate);
            TimeSpan ts = DateTime.Now.Date - dueDate;
            // Difference in days.
            int differenceInDays = ts.Days;
            if (differenceInDays > 0)
            {
                return " <span style='color:red;'>" + _dueDate + " </span>";
            }
            else
            {
                return _dueDate;
            }
        }

      
      
    }
}