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
    public partial class rir_admin : System.Web.UI.Page
    {
        int _userSID=0;
        string _connStr;

        protected static readonly ILog Log = LogManager.GetLogger("rir_admin");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            string adminList = System.Web.Configuration.WebConfigurationManager.AppSettings["AdminList"];


            if (Session["LoginSID"] == null)
            {
                Response.Redirect("../default.aspx", true);
            }

            int _userSID = int.Parse(Session["LoginSID"].ToString());


            if (Session["ISRIRADMIN"] == null || Session["ISRIRADMIN"].ToString()!="Y")
            {
                Response.Redirect("../default.aspx", true);
            }
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