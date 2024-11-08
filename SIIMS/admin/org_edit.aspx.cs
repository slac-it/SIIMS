using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class org_edit : System.Web.UI.Page
    {

        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("org_edit");

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


            string orgID = Request["orgID"].ToString();

            if (!Page.IsPostBack)
            {
                bind_Controls(orgID);
            }

          
        }

        private void bind_Controls(string orgID)
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
                cmd.Parameters.Add(":orgID", OracleDbType.Int32).Value = int.Parse(orgID);

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txtOrgName.Text = reader.GetString(0);
                    txtManager_SID.Text = reader.GetInt32(1).ToString();
                    txtPOC_SID.Text = reader.GetInt32(2).ToString();
                    txtManager.Text = reader.GetString(3);
                    txtPOC.Text = reader.GetString(4);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bind_Controls", ex);
            }

           
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            string _sql = @"Update siims_org set name=:Org_Name, manager_sid=:Manager_SID,poc_sid=:POC_SID,last_by=:loginSID where org_ID=:OrgID";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":Org_Name", OracleDbType.Varchar2).Value = txtOrgName.Text;
                cmd.Parameters.Add(":Manager_SID", OracleDbType.Int32).Value = int.Parse(txtManager_SID.Text);
                cmd.Parameters.Add(":POC_SID", OracleDbType.Int32).Value = int.Parse(txtPOC_SID.Text);
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;

                string orgID = Request["orgID"].ToString();
                cmd.Parameters.Add(":OrgID", OracleDbType.Int32).Value = int.Parse(orgID);

               
                cmd.ExecuteNonQuery();

                con.Close();
                Response.Redirect("org_list.aspx");

            }
            catch (Exception ex)
            {
                Log.Error("btnSubmit_Click", ex);
            }
        }

        private string bindName(string _sid)
        {
            string _fname="";
            string _sql = @"select name from  persons.person  where key=:SID and key not in (select SID from SIIMS_SMT_DELEGATE where org_id=:orgID and is_active='Y')";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = int.Parse(_sid);
                cmd.Parameters.Add(":orgID", OracleDbType.Int32).Value = int.Parse(Request["orgID"].ToString());

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("org_list.aspx");
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            bool foundError = false;

           
            string manger_sid = txtManager_SID.Text;
            string poc_sid = txtPOC_SID.Text;
            string managerName = bindName(manger_sid);
            if (string.IsNullOrEmpty(managerName))
            {
                foundError = true;
                lblManager.Text = "Error: Invalid SLAC ID or this person is used as delegate of this SMT org!";
                txtManager.Text = "";
                lblManager.Visible = true;
            } else {
                txtManager.Text = managerName;
                lblManager.Visible = false;
            }
           
            string pocName = bindName(poc_sid);
             if (string.IsNullOrEmpty(pocName))
            {
                foundError = true;
                lblPoc.Text = "Error: Invalid SLAC ID or this person is used as delegate of this SMT org!";
                txtPOC.Text = "";
                lblPoc.Visible = true;
            } else {
                txtPOC.Text = pocName;
                lblPoc.Visible = false;
            }

             if (foundError)
             {
                 btnConfirm.Visible = true;
                 btnSubmit.Visible = false;
                 txtOrgName.ReadOnly = false;
                 txtManager_SID.ReadOnly = false;
                 txtPOC_SID.ReadOnly = false;
             }
             else
             {
                 btnConfirm.Visible = false;
                 btnSubmit.Visible = true;
                 txtOrgName.ReadOnly = true;
                 txtManager_SID.ReadOnly = true;
                 txtPOC_SID.ReadOnly = true;
             }
           
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string orgID = Request["orgID"].ToString();
            bind_Controls(orgID);
            txtOrgName.ReadOnly = false;
            txtManager_SID.ReadOnly = false;
            txtPOC_SID.ReadOnly = false;
            btnConfirm.Visible = true;
            btnSubmit.Visible = false;
          
        }


    }
}