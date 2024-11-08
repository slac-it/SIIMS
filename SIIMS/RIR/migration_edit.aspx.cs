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
    public partial class migration_edit : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        protected static readonly ILog Log = LogManager.GetLogger("migration_edit");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("../permission.aspx", true);
            }

            if (!Page.IsPostBack)
            {
                bindControls();
            }
        }

        private void bindControls()
        {
            string _sql = @" select  TITLE,statement
                            from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION 
                             where issue_id=:IID ";
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
                    lblIssueID.Text = _Issue_IID.ToString();
                    lblTitle.Text = reader.GetString(0);
                    txtImage1.Text = reader.GetString(1);
                }

                reader.Close();
                con.Close();

            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            updateStatement();
            Response.Redirect("migration_list.aspx?iid=" + _Issue_IID);
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
                    _cmd.CommandType = CommandType.Text;
                    _cmd.CommandText = "UPDATE APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION set STATEMENT=:state where issue_id=:IID";
                    _cmd.Parameters.Add(":state", OracleDbType.Varchar2).Value = _statement;
                    _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;


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
            Response.Redirect("migration_list.aspx?iid=" + _Issue_IID);
        }
    }
}