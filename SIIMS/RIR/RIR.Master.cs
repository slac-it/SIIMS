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
    public partial class RIRUser : System.Web.UI.MasterPage
    {
        int _userSID;
        string _connStr;

        protected static readonly ILog Log = LogManager.GetLogger("RIRUser");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];
            string adminList = System.Web.Configuration.WebConfigurationManager.AppSettings["AdminList"];

            if (_isUAT == "1")
            {
                lblWarning.Text = "This is a test site and is not intended for actual use.";
            }

            if (Session["LoginSID"] == null || Session["LoginName"] ==null)
            {
                Response.Redirect("permission.aspx", true);
            }

            _userSID = int.Parse(Session["LoginSID"].ToString());

            if (_userSID == 0)
            {
                Response.Redirect("permission.aspx", true);
            }

            if (adminList.Contains(_userSID.ToString()) || checkISAdmin())
            {
                adminLink.Visible = true;
                Session["ISRIRADMIN"] = "Y";
            } else
            {
                Session["ISRIRADMIN"] = "N";
            }
            
            litName.Text = Session["LoginName"].ToString();
            //BindToDoCount();

        }

        private bool checkISAdmin()
        {
            bool isAdmin = false;
            string _sql = @"select  count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='Y' and IS_ACTIVE='Y' ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _userSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int _isAdmin = reader.GetInt32(0);
                    if (_isAdmin > 0) isAdmin = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkISAdmin", ex);
            }

            return isAdmin;
        }

    }
}