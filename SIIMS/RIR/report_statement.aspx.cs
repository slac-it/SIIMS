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

namespace SIIMS
{
    public partial class rir_statement : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        protected static readonly ILog Log = LogManager.GetLogger("rir_statement");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());


            int userType = 0;
            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
                userType = checkUserType(_Issue_IID);

                //Log.Debug("Issue_id: " + _Issue_IID + "    Login_SID: " + _loginSID + "     UserType:" + userType);
                // only the initiator, RIR coordinator and two reviewers can edit
                if(userType<=0)
                    Response.Redirect("../permission.aspx", true);
            }
            else
            {
                Response.Redirect("../permission.aspx", true);
            }

            string hid = "";
            if (Request["hid"] != null)
            {
                hid = Request["hid"].ToString();
                if(!string.IsNullOrEmpty(hid))
                {
                    PanelHistory.Visible = true;
                    bind_HistoryStatement(hid);
                }
            }
          

            if (!Page.IsPostBack)
            {
                string rir_status = bindControls();
                Log.Debug("REport Status: " + rir_status);
            }
        }

        private void bind_HistoryStatement(string _hid)
        {

            string _sql = @" select statement,p.name, to_char(sh.created_on,'MM/DD/YYYY') as change_date 
                                        from SIIMS_RIR_STATEMENT_HISTORY sh
                                        join persons.person p on p.key=sh.created_by
                                        where history_id=:HID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":HID", OracleDbType.Varchar2).Value = _hid;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lblOldStatement.Text = reader.GetString(0).Replace("\r", "<br />");
                    lblPerson.Text = reader.GetString(1);
                    lblDate.Text = reader.GetString(2);
                }

                reader.Close();
                con.Close();

            }
            catch (Exception ex)
            {
                Log.Error("bind_HistoryStatement", ex);
            }

        }
        private string bindControls()
        {
            string rir_status = "D";
            string _sql = @"  select report.rir_status, issue.TITLE,report.statement,org.NAME as Org,dept.DEPT,to_char(report.date_discovered, 'MM/DD/YYYY') as date_disc,
                                        report.LOCATION,cond.CONDITION,  p.name as poc_name , report.POC_SID, s.TITLE as STitle, p1.name as Initiator
                                         ,  to_char(issue.created_on, 'MM/DD/YYYY') as date_init
                                        from siims_issue issue 
                                        join siims_rir_report report on issue.issue_id=report.issue_id
                                        join SIIMS_ORG org on org.org_id=issue.ORG_ID
                                        join SIIMS_RIR_CONDITION cond on cond.CONDITION_ID=report.CONDITION_ID
                                        left join siims_dept dept on dept.DEPT_ID=issue.DEPT_ID
                                        left join siims_source s on issue.issue_id = s.issue_id 
                                        left join persons.person p on p.key= report.POC_SID
                                         left join persons.person p1 on p1.key= issue.created_by
                                        where issue.issue_id=:IID and issue.is_rir='Y' ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rir_status = reader.GetString(0);
                    if (rir_status != "E")
                    {
                        Response.Redirect("../permission.aspx", true);
                    }
                    else

                        //lblTitle.Text = reader.GetString(1);
                        txtImage1.Text = reader.GetString(2);
                }

                reader.Close();
                con.Close();

            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return rir_status;
        }

        /// <summary>
        ///  The input is the issue_id of a report
        ///  The ooutput is the type of the user for this report. Type could be
        ///  0: regular user with any view permission
        ///  1: the report initiator, can edit during Draft and Edit report_status
        ///  2: reviewer, could edit and review the report
        ///  3: RIR coordinator: could do anything.
        ///  /// </summary>
        /// <param name="issue_id"></param>
        /// <returns></returns>

        private int checkUserType(int issue_id)
        {
            int userType = 0;
            string _sql = @"select  case when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='Y' and IS_ACTIVE='Y')=1 then '3'
                                            when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y')=1 then '2'
                                            when (select count(*) as type1 from siims_issue where created_by=:loginSID and issue_id=:IID)=1 then '1'
                                            ELSE '0'
                                            END as type
                                            from dual";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = issue_id;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userType = int.Parse(reader.GetString(0));
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkUserType", ex);
            }

            return userType;
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            updateStatement();
            Response.Redirect("create.aspx?from=s&iid=" + _Issue_IID);
        }

        private void updateStatement()
        {

            string _statement = txtImage1.Text.Trim();
         

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_UPDATE_STATEMENT";

                    OracleParameter inAID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inAID.Direction = ParameterDirection.Input;
                    inAID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inDesc = _cmd.Parameters.Add("PI_STATEMENT", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _statement;

                    
                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateStatement", ex);

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("create.aspx?from=s&iid=" + _Issue_IID);
        }
    }
}