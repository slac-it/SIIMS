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

namespace SIIMS
{
    public partial class todolist : System.Web.UI.Page
    {
        int _userSID;
        string _connStr;

        protected static readonly ILog Log = LogManager.GetLogger("todolist");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                //getUserSID();
                getSSOSID();
            }


            _userSID = int.Parse(Session["LoginSID"].ToString());

            if (_userSID == 0)
            {
                Response.Redirect("permission.aspx", true);
            }
        }

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

            string _sql = @"SELECT b.but_sid,INITCAP(p.fname) as fname,p.maildisp,(select count(*) from siims_rir_reviewer where reviewer_sid=b.but_sid and is_active='Y' and is_owner='Y') as is_owner
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
                    Session["LoginEmail"] = reader.GetString(2);
                    Session["IS_OWNER"] = reader.GetInt32(3).ToString();
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("winLogin2SID", ex);
            }
        }

        //  format data for TODO list
        public string DataFormatID(string type, string id)
        {
            string item_id = "";
            if (type == "1" || type == "11" )
            {
                item_id = "ISSUE-";
            } else if ( type == "20" || type == "22")
            {
                item_id = "RIR-";
            }
            else
            {
                item_id = "ACTION-";
            }
            item_id += id;

            return item_id;
        }

        public string DataFormatAction(string type, string status_id, string sub_status_id, string id)
        {
            string action = "";
            if (type == "1" && status_id == "10")
            {
                action = "<a href=issue.aspx?from=todo&iid=" + id + ">Complete Issue Draft </a>  ";
            }
            if (type == "1" && status_id == "11")
            {
                action = "<a href=iip_issueApp.aspx?from=todo&iid=" + id + "&subid=" + sub_status_id + ">Concur Change Request </a>  ";
            }
            if (type == "2" && status_id == "20")
            {
                action = "<a href=action.aspx?from=todo&aid=" + id + ">Complete Action Draft </a>";
            }

            if (type == "2" && status_id == "21")
            {
                action = "<a href=action_take.aspx?from=todo&aid=" + id + ">Accept Assignment </a>";
            }

            if (type == "2" && status_id == "22")
            {
                action = "<a href=iip_actionApp.aspx?from=todo&aid=" + id + "&subid=" + sub_status_id + ">Concur Change Request </a>  ";
            }

            if (type == "3" && status_id == "22")
            {
                action = "<a href=action_iapp.aspx?from=todo&aid=" + id + "&subid=" + sub_status_id + ">Approve Change Request </a>  ";
            }

            if (type == "4")
            {
                action = "<a href=action_changeDue.aspx?from=todo&aid=" + id + ">Request Extention </a>  ";
            }

            if (type == "11")
            {
                action = "<a href=action.aspx?from=todo&iid=" + id + ">Create Action </a>  ";
            }

            if (type == "20")
            {
                action = "<a href=rir/create.aspx?from=todo&iid=" + id + ">Complete Report Draft </a>  ";
            }

            if (type == "22")
            {
                action = "<a href=rir/report_view.aspx?from=todo&iid=" + id + ">Approve RIR Report </a>  ";
            }

            return action;
        }


        public string DataFormatStatus(string type, string status_id, string sub_status_id)
        {
            string status = "";
            if (status_id == "10")
            {
                status = "Draft";
            }
            if (status_id == "11" && sub_status_id == "110")
            {
                status = "Open (Pending for Modification)";
            }
            if (status_id == "11" && sub_status_id == "111")
            {
                status = "Open (Pending for Deletion)";
            }
            if (status_id == "11" && sub_status_id == "115")
            {
                status = "Open (Pending for Closure)";
            }

            if (status_id == "20")
            {
                if (sub_status_id == "200")
                {
                    status = "Draft (Rejected by Assigned Owner)";
                }
                else
                {
                    status = "Draft ";
                }

            }

            if (status_id == "21")
            {
                status = "Waiting for Acceptance ";

            }

            if (status_id == "22" && sub_status_id == "224")
            {
                status = "Open (Pending for Due Date Change)";
            }

            if (status_id == "22" && sub_status_id == "223")
            {
                status = "Open (Pending for Owner Change)";
            }
            if (status_id == "22" && sub_status_id == "220")
            {
                status = "Open (Pending for Modification)";
            }

            if (status_id == "22" && sub_status_id == "222")
            {
                status = "Open (Pending for Closure)";
            }
            if (status_id == "22" && sub_status_id == "221")
            {
                status = "Open (Pending for Deletion)";
            }

            if (type == "4")
            {
                status = "Open (OverDue)";
            }

            if (type == "11")
            {
                status = "Open (No Actions)";
            }

            return status;
        }


        #region GDActions Event Handlers
        //This event occurs for each row
        protected void GVTodo_RowDataBound(object sender, GridViewRowEventArgs e)
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

                DropDownList ddlTeams = e.Row.FindControl("drwTesting") as DropDownList;
                // bind the dropdown here if needed
                //ddlTeams.DataSource = TeamMember.TeamsUserAllocatedTo(userId);
                //ddlTeams.DataBind();
            }


        }

        //This event occurs for any operation on the row of the grid
        protected void GVTodo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Check if Add button clicked
            if (e.CommandName == "AddIssue")
            {

            }
        }

        #endregion
    }
}