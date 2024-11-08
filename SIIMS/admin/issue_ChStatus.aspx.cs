using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class issue_ChStatus : System.Web.UI.Page
    {

        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("issue_ChStatus");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            {
                Response.Redirect("../permission.aspx");
            }


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);
        }



        protected void btnClear_Click(object sender, EventArgs e)
        {
            //txtSLACID.Text = "";

            txtOwner.Text = "";
            ddlStatuslist.Items.Clear();
            Panel1.Visible = false;

            lblStatus.Text = "";
            cmdFind.Enabled = true;
            cmdFind.Visible = true;

        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
           
            PanelWarning.Visible = false;
            string _issueID = txtOwner.Text;
            int issueID = int.Parse(_issueID);

            bool isFound=bindIssue(issueID);

            if(isFound)
            {
                PanelOK.Visible = true;
                int openActions = checkOpenActions(issueID);
                bool isPending = bindPendingRequest(issueID);
                if (openActions > 0 || isPending)
                {
                    PanelWarning.Visible = true;
                    btnChange.OnClientClick += "javascript:return confirm('There are pending requests or open actions for this issue. Are you sure you want to clear the requests, close actions, and change issue status?');";
                }
            }
            else
            {
                PanelOK.Visible = false;
                lblError.Text = "Error: this issue does not exist!";
                lblError.Visible = true;
            }
        }

        private bool bindPendingRequest(int iid)
        {
            bool isPending = false;
            try
            {
                OracleConnection _conn = new OracleConnection(_connStr);
                string _sql1 = @" select ch.issue_id, ch.sub_status_id, s.status, ch.created_by, to_char(ch.created_on,'MM/DD/YYYY') as request_on ,p.name
                                    from siims_issue_change ch
                                    join siims_status s on s.status_id = ch.sub_status_id and s.is_active = 'Y'
                                     join persons.person p on p.key = ch.created_by
                                    where ch.issue_id =:IID and ch.is_active = 'Y' ";

                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand(_sql1, _conn);
                _cmd1.BindByName = true;

                _cmd1.Parameters.Add(":IID", OracleDbType.Int32).Value = iid;
                //   _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _reviewID;

                OracleDataReader _reader1 = _cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                if (_reader1.HasRows)
                {

                    isPending = true;
                }
                gvwRequests.DataSource = _reader1;
                gvwRequests.DataBind();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindPendingRequest", ex);
            }

            return isPending;
        }
        protected bool  bindIssue(int issueID)
        {
            bool isFound = false;
            string _sql = "";

            OracleConnection _conn = new OracleConnection(_connStr);

            try
            {

                int c_statusID;

                _sql = @"select st.status, st.status_id, sa.title from siims_status st join siims_issue sa on st.status_id = sa.status_id where sa.issue_id = :IID";
                _conn.Open();

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;

                _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = issueID;

                OracleDataReader _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (_reader.HasRows)
                {
                    isFound = true;
                    _reader.Read();
                    lblError.Visible = false;
                    Panel1.Visible = true;
                    cmdFind.Enabled = false;
                    cmdFind.Visible = false;

                    lblStatus.Text = _reader.GetString(0);
                    c_statusID = _reader.GetInt32(1);
                    lblTitle.Text = _reader.GetString(2);
                    _conn.Close();
                    HiddenIssueID.Value = issueID.ToString();

                    // the employee self is removed from the list. We do not allow an employee to be his/her own matrix manager
                    string _sql2 = @" SELECT action.issue_id,action_id, 'A' || action.action_id  as action_id2 , action.title as ATitle, sta.status, p.name as owner
                            from siims_action action join siims_status sta on action.status_id = sta.status_id and object_id = 2 left join persons.person p
                            on action.owner_sid = p.key where action.is_active = 'Y' and action.issue_id =:issueID order by action.created_on desc";

                    _conn.Open();

                    OracleCommand _cmd2 = new OracleCommand(_sql2, _conn);
                    _cmd2.BindByName = true;

                    _cmd2.Parameters.Add(":issueID", OracleDbType.Int32).Value = issueID;

                    OracleDataReader _reader2 = _cmd2.ExecuteReader(CommandBehavior.CloseConnection);

                    GVActions.DataSource = _reader2;
                    GVActions.DataBind();
                    bindStatusDDL(c_statusID);

                } 
            }
            catch (Exception ex)
            {
                Log.Error("bindIssue", ex);
            }

            return isFound;

        }

        private void bindStatusDDL(int _statusID)
        {
            OracleConnection _conn = new OracleConnection(_connStr);

            // the employee self is removed from the list. We do not allow an employee to be his/her own matrix manager
            string _sql1 = " select status_id, status from siims_status where object_id = 1 and parent_id is null and is_active = 'Y' ";
            _sql1 += "  and status_id not in (10,:statusID)  ORDER BY status_id";

            _conn.Open();

            OracleCommand _cmd1 = new OracleCommand(_sql1, _conn);
            _cmd1.BindByName = true;

            _cmd1.Parameters.Add(":statusID", OracleDbType.Int32).Value = _statusID;
            //   _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _reviewID;

            OracleDataReader _reader1 = _cmd1.ExecuteReader(CommandBehavior.CloseConnection);
            ddlStatuslist.Items.Clear();

            while (_reader1.Read())
            {
                ListItem NewItem = new ListItem();
                string _status_id = _reader1.GetInt32(0).ToString();
                string _status = _reader1.GetString(1);
                NewItem.Text = _status;
                NewItem.Value = _status_id;
                ddlStatuslist.Items.Add(NewItem);
            }
        }


        private int checkOpenActions(int _issueID)
        {
 
            int totalOpen = 0;

            string _sql = @" select action_id from siims_action where status_id in (21, 22) and is_active='Y' and issue_id=:IID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _issueID;

                OracleDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows) totalOpen= 1;
               

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkOpenActions", ex);
            }

            return totalOpen;

        }

        protected void btnChange_Click(object sender, EventArgs e)
        {

            string _issueID = HiddenIssueID.Value;
            try
            {
                int status_id = int.Parse(ddlStatuslist.SelectedValue);

                using (OracleConnection _connection = new OracleConnection())
                {
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = " APPS_ADMIN.SIIMS_ADMIN_PKG.PROC_CHANGE_ISSUE_STATUS";
                    OracleParameter inLogSID = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogSID.Direction = ParameterDirection.Input;
                    inLogSID.Value = _loginSID;

                    OracleParameter inStatusID = _cmd.Parameters.Add("PI_NEW_STATUS_ID", OracleDbType.Int32);
                    inStatusID.Direction = ParameterDirection.Input;
                    inStatusID.Value = status_id;

                    OracleParameter newSID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    newSID.Direction = ParameterDirection.Input;
                    newSID.Value = int.Parse(_issueID);
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    _cmd.ExecuteNonQuery();

                    _connection.Close();

                    lblStatus.Text = ddlStatuslist.SelectedItem.Text;
                    PanelWarning.Visible = false;
                    bindPendingRequest(int.Parse(_issueID));
                    PanelOK.Visible = false;
                    cmdFind.Enabled = true;
                    cmdFind.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("btnChange_Click", ex);
                lblError.Text = "Error: " + ex.Message;
                lblError.Visible = true;

            }


        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }


    }
}