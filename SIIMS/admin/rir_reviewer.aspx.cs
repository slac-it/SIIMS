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
    public partial class rir_reviewer : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("rir_dist");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (Session["ISRIRADMIN"] == null || Session["ISRIRADMIN"].ToString() != "Y")
            {
                Response.Redirect("../rir/rir.aspx", true);
            }

            if (!Page.IsPostBack)
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
                    sqlCmd.CommandText = @"SELECT p.name,p.key,vw.title from SIIMS_RIR_REVIEWER vw
                                             join PERSONS.PERSON p on p.key = vw.reviewer_sid
                                             where  vw.IS_ACTIVE = 'Y' order by p.name";
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            gvwReviewerList.DataSource = ds;
            gvwReviewerList.DataBind();

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
            string reviewerID = drwReviewer.SelectedValue;

            try
            {
                // first test data validation: We do not allow one person to be duplicated reviewers.
                // But we do allow one person to be both reviewer and coordinator
                bool isPassed=checkDataDuplication(reviewerID, int.Parse(_smt_sid));
                if (isPassed)
                {
                    // add the delegate to the DB table
                    using (OracleConnection _conn = new OracleConnection(_connStr))
                    {


                        using (OracleCommand _cmd1 = new OracleCommand("", _conn))
                        {
                            _cmd1.BindByName = true;
                            _cmd1.CommandText = "APPS_ADMIN.SIIMS_RIRADMIN_PKG.PROC_RIRADMIN_CHANGEREVIEWER";
                            _cmd1.CommandType = CommandType.StoredProcedure;
                            OracleParameter inLogSID = _cmd1.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                            inLogSID.Direction = ParameterDirection.Input;
                            inLogSID.Value = _loginSID;

                            OracleParameter inRID= _cmd1.Parameters.Add("PI_RID", OracleDbType.Int32);
                            inRID.Direction = ParameterDirection.Input;
                            inRID.Value = int.Parse(reviewerID);

                            OracleParameter newSID= _cmd1.Parameters.Add("PI_NEW_SID", OracleDbType.Int32);
                            newSID.Direction = ParameterDirection.Input;
                            newSID.Value = int.Parse(_smt_sid);

                            _conn.Open();
                            _cmd1.ExecuteNonQuery();
                            _conn.Close();
                            PnlPopup.Visible = false;
                            txtOwner.Text = "";


                        }

                    }
                } else
                {
                    lblError.Visible = true;
                    lblError.Text = "Error: this is a duplicated assignment.";
                    cmdOk.Enabled = true;
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

        protected bool checkDataDuplication(string _rid, int _sid)
        {

            bool isPassed = false;

            lblError.Visible = false;

            OracleConnection _conn = null;
            OracleDataReader _reader = null;

            string _sql = "";

            try
            {
                // exclude people already in the .ist
                _sql = @"select (select is_owner from siims_rir_reviewer where reviewer_id =:RID) as is_owner,
                        (select count(*) from siims_rir_reviewer where is_owner='N' and reviewer_id<>:RID and reviewer_sid=:SID and is_active='Y') as is_reviewer
                        from dual";
                //       and p.key not in (select REVIEWER_SID from SIIMS_RIR_REVIEWER where is_active='Y' )   ORDER BY p.name";

                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;
                _cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = _sid;
                _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _rid;

                _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (_reader.Read())
                {
                    string is_owner = _reader.GetString(0);
                    int is_reviewer = _reader.GetInt32(1);
                    if (is_owner == "Y" || is_reviewer == 0) isPassed = true;
                }
             
            }
            finally
            {
                if (_reader != null) { _reader.Close(); _reader.Dispose(); }
                if (_conn != null) { _conn.Close(); }
            }

            return isPassed;
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
                               ORDER BY p.name";
                //       and p.key not in (select REVIEWER_SID from SIIMS_RIR_REVIEWER where is_active='Y' )   ORDER BY p.name";

                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;

                _cmd.Parameters.Add(":PName", OracleDbType.Varchar2).Value = strtxtOwner;

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

    }
}