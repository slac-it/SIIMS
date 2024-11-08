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
using System.IO;

namespace SIIMS
{
    public partial class action_changeDue : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Action_AID = 0;
        bool isActOwner = false;
        bool isIOwner = false;
        protected static readonly ILog Log = LogManager.GetLogger("action_changeDue");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            if (Request["aid"] != null)
            {
                _Action_AID = int.Parse(Request["aid"].ToString());
            }
            else
            {
                Response.Redirect("~/default.aspx", true);
            }
           

            Helpers _helper = new Helpers();
            isIOwner = _helper.IsIOwnerOfAction(_loginSID, _Action_AID) || _helper.IsPOCOfAction(_loginSID, _Action_AID);
            isActOwner = _helper.IsActionOwner(_loginSID, _Action_AID);

            if (!isIOwner && !isActOwner)
            {
                // only IIP or Deputy director can do issue approval
                Response.Redirect("action_view.aspx?aid=" + _Action_AID, true);
            }


            if (!Page.IsPostBack && _Action_AID > 0)
            {
                int issue_ID=bindControls();
                bindIssueControls(issue_ID);
                BindFileGrid();

            }

            string sub_status = checkPendingRequest();

            if (sub_status.Contains("222"))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "The action is locked out since there is a pending request for closure. ";
                btnChange.Visible = false;
                btnCancel.Visible = false;
            }
            else if (sub_status.Contains("221"))
            {
                 lblMsg.Visible = true;
                 lblMsg.Text = "The action is locked out since there is a pending request for deletion. ";
                btnChange.Visible = false;
                btnCancel.Visible = false;
            } if (sub_status.Contains("224"))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "There is a pending request for Due Date change already. ";
                btnChange.Visible = false;
                btnCancel.Visible = false;
            }

        }

        private string checkPendingRequest()
        {

            string sub_status = ",";

            string _sql = " select sub_status_id from siims_action_change where is_active='Y' and sub_status_id in (220,221,222,223,224)  and action_id=:AID ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    sub_status += reader.GetInt32(0) + ",";

                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkPendingRequest", ex);
            }

            return sub_status;

        }

        private void bindIssueControls(int _Issue_IID)
        {
            lblIssueID.Text = _Issue_IID.ToString();
            string _sql = "   select TITLE,DESCRIPTION,ORG_ID,OWNER_SID,SIG_LEVEL,STYPE_ID,  STitle, FY, QUARTER, owner_name " +
             " , status_id, status, sub_status, sub_status_id, TYPE , NAME " +
             " from VW_SIIMS_ISSUE_VIEW where issue_id=:IID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblITitle.Text = "<a href='issue_view.aspx?iid=" + _Issue_IID + "'>" + reader.GetString(0) + "</a>";
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    desc = desc.Replace("\r", "<br />");
                    lblIDesc.Text = desc;
                    if (desc.Length > 80)
                    {
                        string desc1 = desc.Substring(0, Math.Min(desc.Length, 80));
                        desc1 = desc1 + " ... &nbsp; &nbsp; <a href='javascript:showFull();'>Read More</a>";

                        lblIDescShort.Text = desc1;
                        lblIDescShort.Visible = true;
                        lblIDesc.CssClass = "invisibeText";
                    }
                    else
                    {
                        lblIDescShort.Visible = false;

                    }

                    //lblDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        //lblOrg.Text = reader.GetInt32(2).ToString();
                        lblOrg.Text = reader.GetString(15);
                    }


                    if (!reader.IsDBNull(4))
                    {
                        lblLevel.Text = reader.GetString(4);
                    }

                    string _sType = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    if (_sType.Length > 0)
                    {
                        lblSType.Text = reader.GetString(14);

                        lblSourceTitle.Text = reader.GetString(6);
                        lblSourceFY.Text = reader.GetString(7);
                        lblSourceQtr.Text = reader.GetString(8);
                    }


                    lblIOwner.Text = reader.GetString(9);



                }
                else
                {
                    Response.Redirect("permission.aspx");
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindIssueControls", ex);
            }
        }

        private int bindControls()
        {

            int issue_ID = 0;
            lblAID2.Text = "A" + _Action_AID.ToString();

            string _sql = " select action.issue_id, action.TITLE as title, action.description, p.name as owner_name,to_char(action.due_date, 'MM/DD/YYYY') as due_date, issue.sig_level, action.note " +
                        " from siims_action action left join persons.person p on p.key=action.owner_sid  join siims_issue issue on issue.issue_id=action.issue_id where action.action_id=:AID and action.is_active='Y' ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
            
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    issue_ID = reader.GetInt32(0);
                    lblATitle.Text = reader.GetString(1);
                    lblADesc.Text = reader.GetString(2);
                    lblAOwner.Text = reader.GetString(3);
                    lblDue.Text = FormatDueDate(reader.GetString(4));
                    lblLevel.Text = reader.GetString(5);
                   
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return issue_ID;
        }

        public string FormatDueDate(string _dueDate)
        {
            DateTime dueDate = DateTime.Parse(_dueDate);
            TimeSpan ts = DateTime.Now.Date - dueDate;
            // Difference in days.
            int differenceInDays = ts.Days;
            if (differenceInDays > 0)
            {
                return " <span style='color:red; font-weight:bold;'>" + _dueDate + " </span>";
            }
            else
            {
                return _dueDate;
            }
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            string _dueDate = Request.Form[txtDueDate.UniqueID];
            if (string.IsNullOrEmpty(_dueDate))
            {
                lblError.Visible = true;
                return;
            }
            else
            {
                lblError.Visible = false;
            }


            if (isIOwner)
            {
                updateAction(_Action_AID, _dueDate);
            }
            else if (isActOwner)
            {
                requestChangeAction(_Action_AID, _dueDate);
            }
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        private void updateAction(int _Action_AID, string _dueDate)
        {


            //string _dueDate = Request.Form[txtDueDate.UniqueID];

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTIONEDIT_PKG.PROC_ACTION_CHANGEDUE";


                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inAction_ID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAction_ID.Direction = ParameterDirection.Input;
                    inAction_ID.Value = _Action_AID;



                    OracleParameter inDuedate = _cmd.Parameters.Add("PI_NEW_DUEDATE", OracleDbType.Date);
                    inDuedate.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_dueDate))
                    {
                        inDuedate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDuedate.Value = Convert.ToDateTime(_dueDate);
                    }


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

            }
        }

        private void requestChangeAction(int _Action_AID, string _dueDate)
        {

            //string _dueDate = Request.Form[txtDueDate.UniqueID];

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTIONEDIT_PKG.PROC_ACTIONOWNER_CHANGEDUE";


                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inAction_ID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAction_ID.Direction = ParameterDirection.Input;
                    inAction_ID.Value = _Action_AID;




                    OracleParameter inDuedate = _cmd.Parameters.Add("PI_NEW_DUEDATE", OracleDbType.Date);
                    inDuedate.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_dueDate))
                    {
                        inDuedate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDuedate.Value = Convert.ToDateTime(_dueDate);
                    }


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        private void BindFileGrid()
        {
            // TBD: 

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = "select ACATT_ID,FILE_NAME, CREATED_ON from SIIMS_ACTION_ATT where action_id=:AID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            lv_Files.DataSource = ds;
            lv_Files.DataBind();

        }

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                FileData(_attachmentId);
            }

        }

        private void FileData(int _attachmentId)
        {

            FileUtil objFile = new FileUtil();
            bool isError = objFile.downLoadAttachment(_attachmentId, 2);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }
    }
}