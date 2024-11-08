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

namespace SIIMS.RIR
{
    public partial class report_dist : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        //string _RIR_Status;
        protected static readonly ILog Log = LogManager.GetLogger("report_dist");
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
            int is_emailed = 0;
           
            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
                userType = checkUserType(_Issue_IID, out is_emailed);
                if(userType!=3 )
                {
                    Response.Redirect("../permission.aspx", true);
                }
              
            }
            else
            {
                Response.Redirect("../permission.aspx", true);
            }

            if(!Page.IsPostBack)
            {
                if (Request["from"] != null && Request["from"].ToString() != "v")
                {

                    prepareInitialList();
                }
                // delete existing list for this report and add new to the list
             
                 bindGrid();

            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            cmdOk.Enabled = false;
            string _smt_sid = ddlEmplist.SelectedValue;

            if (string.IsNullOrEmpty(_smt_sid))
            {
                lblError.Visible = true;
                lblError.Text = "You have to select one person from the dropdown list.";
                return;
            }
            string title = txtTitle.Text;

            try
            {
                // add the delegate to the DB table
                using (OracleConnection _conn = new OracleConnection(_connStr))
                {


                    using (OracleCommand _cmd1 = new OracleCommand("", _conn))
                    {
                        _cmd1.BindByName = true;
                        _cmd1.CommandText = "INSERT INTO SIIMS_RIR_VIEW  (ISSUE_ID, VIEWER_SID,VIEWER_TITLE,CREATED_BY) values (:IID, :M_SID, :Title, :login_SID) ";
                        _cmd1.CommandType = CommandType.Text;
                        _cmd1.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;
                        _cmd1.Parameters.Add(":M_SID", OracleDbType.Int32).Value = int.Parse(_smt_sid);
                        _cmd1.Parameters.Add(":Title", OracleDbType.Varchar2).Value = title;
                        _cmd1.Parameters.Add(":login_SID", OracleDbType.Varchar2).Value = _loginSID;

                        _conn.Open();
                        _cmd1.ExecuteNonQuery();
                        PanelFound.Visible = false;
                        txtOwner.Text = "";


                    }

                }

            }
            catch (Exception ex)
            {

                Log.Error("cmdOk_Click", ex);
            }
            finally
            {

                bindGrid();
            }

        }

        private void bindGrid()
        {
            DataSet ds = new DataSet();
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.BindByName = true;
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = @"SELECT view_id, p.key as SID, p.name as viewer, vw.VIEWER_TITLE, dept.description as dept_name 
                                                                from SIIMS_RIR_VIEW vw
                                                                join PERSONS.PERSON p on p.key = vw.VIEWER_SID  join SID.organizations dept on p.dept_id = dept.org_id
                                                                where ISSUE_ID =:IID and vw.IS_ACTIVE = 'Y'
                                                                order by name ";
                    sqlCmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            gvwDistList.DataSource = ds;
            gvwDistList.DataBind();

        }
        protected void cmdFind_Click(object sender, EventArgs e)
        {
            string errorMsg = "Please enter the first few characters to start your search";

            if (string.IsNullOrEmpty(txtOwner.Text.Trim()))
            {
                lblError.Text = errorMsg;
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;

            OracleConnection _conn = null;
            OracleDataReader _reader = null;


            string _sql = "";

            ddlEmplist.Visible = true;
            cmdOk.Visible = true;
            string strtxtOwner = txtOwner.Text.Trim().ToLower();

            try
            {
                // exclude people already in the .ist
                _sql = @"select distinct p.key, p.name, dept.description as dept_name from persons.person p join SID.organizations dept on p.dept_id=dept.org_id 
                                 join  but b on b.but_sid=p.key and b.But_ldt='win'  where p.gonet='ACTIVE' and p.status='EMP' and LOWER(p.name) LIKE LOWER(:PName) || '%' 
                                and p.key not in (select VIEWER_SID from SIIMS_RIR_VIEW where is_active='Y' and issue_id=:IID)   ORDER BY p.name";

                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;

                _cmd.Parameters.Add(":PName", OracleDbType.Varchar2).Value = strtxtOwner;
                _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

                ddlEmplist.Items.Clear();
                if (_reader.HasRows)
                {
                    ddlEmplist.Visible = true;
                    PanelFound.Visible = true;

                    lblError.Visible = false;
                    while (_reader.Read())
                    {
                        ListItem NewItem = new ListItem();
                        string _name = Convert.ToString(_reader["name"]);
                        string _slac_id = Convert.ToString(_reader["key"]);
                        string _dept = Convert.ToString(_reader["dept_name"]);
                        NewItem.Text = _name + " - " + _slac_id + " - " + _dept;
                        NewItem.Value = _slac_id;
                        ddlEmplist.Items.Add(NewItem);
                        cmdOk.Enabled = true;
                    }
                }
                else
                {
                    PanelFound.Visible = false;
                    lblError.Visible = true;
                    lblError.Text = "Name misspelled or already in the list.";
                }
            }
            finally
            {
                if (_reader != null) { _reader.Close(); _reader.Dispose(); }
                if (_conn != null) { _conn.Close(); }
            }


        }

        protected void prepareInitialList()
        {

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_SET_DISTLIST ";

                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                    btnEmail.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.Error("prepareInitialList", ex);

            }
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

        private int checkUserType(int issue_id, out int is_emailed)
        {
            is_emailed = 0;
            int userType = 0;
            string _sql = @"select  case when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='Y' and IS_ACTIVE='Y')=1 then '3'
                            when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y')=1 then '2'
                            when (select count(*) as type1 from siims_issue where created_by=:loginSID and issue_id=:IID)=1 then '1'
                            ELSE '0'   END as type 
                            , (select count(*) from siims_rir_view v join siims_rir_report re on re.issue_id=v.issue_id and re.is_dist_saved='Y' where re.issue_id=:IID and v.is_active='Y' ) as is_emailed
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
                    is_emailed = reader.GetInt32(1);
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

        protected void btnEmail_Click(object sender, EventArgs e)
        {
            markAsDone();
            Log.Debug("IID:" + _Issue_IID);
            Response.Redirect("report_view.aspx?iid="+_Issue_IID);
        }

        protected void markAsDone()
        {

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;

                _cmd1.CommandText = "UPDATE SIIMS_RIR_REPORT SET IS_DIST_SAVED='Y', DIST_SAVED_BY=:lastBy, DIST_SAVED_ON=sysdate where issue_id=:IID and exists ( SELECT 1 from SIIMS_RIR_VIEW where issue_id=:IID and is_active='Y') ";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":lastBy", OracleDbType.Varchar2).Value = _loginSID;
                _cmd1.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _Issue_IID;

                _cmd1.ExecuteNonQuery();

                _cmd1.Dispose();
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            //bindGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("rir.aspx");
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            LinkButton lnkbtn = sender as LinkButton;
            GridViewRow grdRow = lnkbtn.NamingContainer as GridViewRow;
            string smt_id = gvwDistList.DataKeys[grdRow.RowIndex].Value.ToString();
            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;

                _cmd1.CommandText = "UPDATE SIIMS_RIR_VIEW SET IS_ACTIVE='N', LAST_BY=:lastBy, LAST_ON=sysdate where view_id=:smtID and IS_ACTIVE='Y' ";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":lastBy", OracleDbType.Varchar2).Value = _loginSID;
                _cmd1.Parameters.Add(":smtID", OracleDbType.Varchar2).Value = smt_id;

                _cmd1.ExecuteNonQuery();

                _cmd1.Dispose();
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            bindGrid();
        }
    }
}