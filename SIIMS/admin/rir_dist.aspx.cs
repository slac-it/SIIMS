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
    public partial class rir_dist : System.Web.UI.Page
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
                Response.Redirect("../default.aspx", true);
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
                    sqlCmd.CommandText = @"SELECT dist_id, p.key as SID, p.name as viewer, vw.DIST_TITLE , dept.description as dept_name 
                                             from SIIMS_RIR_DISTRIBUTOR vw
                                            join PERSONS.PERSON p on p.key = vw.DIST_SID 
                                            join SID.organizations dept on p.dept_id = dept.org_id
                                            where  vw.IS_ACTIVE = 'Y'
                                            order by p.name ";
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            gvwDistList.DataSource = ds;
            gvwDistList.DataBind();

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
                        _cmd1.CommandText = "INSERT INTO SIIMS_RIR_DISTRIBUTOR  (DIST_SID, DIST_TITLE,LAST_BY) values ( :M_SID, :Title, :login_SID) ";
                        _cmd1.CommandType = CommandType.Text;
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
                                and p.key not in (select DIST_SID from SIIMS_RIR_DISTRIBUTOR where is_active='Y' )   ORDER BY p.name";

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("rir_admin.aspx");
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

                _cmd1.CommandText = "UPDATE SIIMS_RIR_DISTRIBUTOR SET IS_ACTIVE='N', LAST_BY=:lastBy, LAST_ON=sysdate where dist_id=:smtID and IS_ACTIVE='Y' ";
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