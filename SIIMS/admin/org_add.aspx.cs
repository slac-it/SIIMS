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
    public partial class org_add : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("org_add");

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
        }

     

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            string _sql = @"Insert into siims_org (name, manager_sid,poc_sid,created_by) values (:Org_Name, :Manager_SID,:POC_SID,:loginSID)";
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
            string _fname = "";
            string _sql = @"select name from  persons.person  where key=:SID";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = int.Parse(_sid);

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
                lblManager.Text = "Error: Invalid SLAC ID!";
                txtManager.Text = "";
                lblManager.Visible = true;
            }
            else
            {
                txtManager.Text = managerName;
                lblManager.Visible = false;
            }

            string pocName = bindName(poc_sid);
            if (string.IsNullOrEmpty(pocName))
            {
                foundError = true;
                lblPoc.Text = "Error: Invalid SLAC ID!";
                txtPOC.Text = "";
                lblPoc.Visible = true;
            }
            else
            {
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
    }
}