using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class migration_delete : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int FID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("migration_upload");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            if (Request["iid"] != null)
            {
                FID = int.Parse(Request["iid"].ToString());
                deletePDF(FID);
            }

            Response.Redirect("migration_list.aspx", true);

        }

        protected void deletePDF(int _fid)
        {


            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd = new OracleCommand("", _conn);
                _cmd.CommandText = " UPDATE APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION SET PDF_DATA=null,PDF_FILENAME=null, PDF_ON=sysdate,PDF_BY=:loginSID where issue_id=:IID";
                _cmd.BindByName = true;
                _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = FID;
                _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error("deletePDF:", ex);
            }


        }
    }
}