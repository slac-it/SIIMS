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
    public partial class delegate_edit : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int OrgID;

        protected static readonly ILog Log = LogManager.GetLogger("delegate_edit");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            // make sure the login is Admin
            if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            {
                Response.Redirect("../permission.aspx");
            }
            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);

            OrgID = int.Parse(Request["orgID"].ToString());

            BindData();
            if (!Page.IsPostBack)
            {
                bind_Controls(OrgID);
            }
        }

        private void bind_Controls(int orgID)
        {
            string _sql = @"select org.name,manager_sid,poc_sid, mgr.name as manager_name, poc.name as poc_name from siims_org org
                                join persons.person mgr on org.manager_sid=mgr.key join persons.person poc on org.poc_sid=poc.key
                                where org.is_active='Y' and org.org_id=:orgID";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":orgID", OracleDbType.Int32).Value = orgID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                   lblOrgName.Text = reader.GetString(0);

                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bind_Controls", ex);
            }


        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            LinkButton lnkbtn = sender as LinkButton;
            GridViewRow grdRow = lnkbtn.NamingContainer as GridViewRow;
            string smt_id = gvw_editSMT.DataKeys[grdRow.RowIndex].Value.ToString();
            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;

                _cmd1.CommandText = "UPDATE SIIMS_SMT_DELEGATE set  IS_ACTIVE='N', LAST_BY=:lastBy where DELEGATE_ID=:smtID and IS_ACTIVE='Y' and ORG_ID=:OrgID ";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":lastBy", OracleDbType.Varchar2).Value = _loginSID;
                _cmd1.Parameters.Add(":smtID", OracleDbType.Varchar2).Value = smt_id;

                _cmd1.Parameters.Add(":OrgID", OracleDbType.Int32).Value = OrgID;
                

                _cmd1.ExecuteNonQuery();

                _cmd1.Dispose();
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            BindData();
        }

        private void BindData()
        {
            DataSet ds = new DataSet();
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.BindByName = true;
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText =@"select dele.DELEGATE_ID, dele.SID, p.name, TO_CHAR(dele.CREATED_ON, 'MM/DD/YYYY') CREATED_ON  
                                         from SIIMS_SMT_DELEGATE dele join persons.person p on dele.sid=p.key where dele.IS_ACTIVE='Y' and org_id=:DID  order by p.name ";

                    sqlCmd.Parameters.Add(":DID", OracleDbType.Int32).Value = OrgID;

                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

           
            gvw_editSMT.DataSource = ds;
            gvw_editSMT.DataBind();

        }

        private string bindName(string _sid)
        {
            string _fname = "";
            string _sql = @"select name from  persons.person  where key=:SID and key not in (select SLACID from VW_SIIMS_ORG_STAKEHOLDERS where org_id=:orgID)";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = int.Parse(_sid);
                cmd.Parameters.Add(":orgID", OracleDbType.Int32).Value = OrgID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _fname = reader.GetString(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindName", ex);
            }

            return _fname;
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {

            string dele_sid = txtDelegate.Text;
            string delegateName = bindName(dele_sid);
            if (string.IsNullOrEmpty(delegateName))
            {
                lblDelegateError.Text = "Invalid SLAC ID or this person is used already in this SMT org!";
                lblDelegateError.Visible = true;
                txtDelegateName.Visible = false;
                btnConfirm.Visible = true;
                btnAdd.Visible = false;
            }
            else
            {
                lblDelegateError.Visible = false;
                txtDelegateName.Visible = true;
                txtDelegateName.Text = delegateName;
                btnConfirm.Visible = false;
                btnAdd.Visible = true;
                txtDelegate.ReadOnly = true;
            }

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int SID = int.Parse(txtDelegate.Text);
            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;

                _cmd1.CommandText = "INSERT INTO SIIMS_SMT_DELEGATE (ORG_ID, SID) VALUES (:OrgID, :SID) ";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":OrgID", OracleDbType.Int32).Value = OrgID;
                _cmd1.Parameters.Add(":SID", OracleDbType.Int32).Value = SID;


                _cmd1.ExecuteNonQuery();

                _cmd1.Dispose();
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            BindData();
            txtDelegate.ReadOnly = false;
            txtDelegate.Text = "";
            txtDelegateName.Text = "";
            btnConfirm.Visible = true;
            btnAdd.Visible = false;

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("org_list.aspx");
        }
       
    }
}