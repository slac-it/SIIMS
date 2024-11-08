using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using SIIMS.App_Code;

namespace SIIMS
{
    public partial class action_take : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Action_AID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("action_take");

        protected void Page_Load(object sender, EventArgs e)
        {

            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);



            if (Request["aid"] != null)
            {
                _Action_AID = int.Parse(Request["aid"].ToString());
            }
            else
            {
                Response.Redirect("default.aspx", true);
            }

          

            if (!Page.IsPostBack && _Action_AID > 0)
            {
                if (!bindControls())
                {
                    Response.Redirect("action_view.aspx?aid=" + _Action_AID, true);
                }
                BindFileGrid();
            }

        }

        private bool bindControls()
        {
            bool isOwner = false;

            lblAID2.Text = "A" + _Action_AID.ToString();

            string _sql = " select   action.TITLE as title, action.description, p.name,to_char(action.due_date, 'MM/DD/YYYY') as due_date " +
                        " from siims_action action left join persons.person p on p.key=action.created_by where action.action_id=:AID and action.owner_sid=:loginSID " +
                        "  union select action.TITLE as title, action.description, p.name,to_char(action.due_date, 'MM/DD/YYYY') as due_date  " +
                        " from siims_action action join siims_action_change ch  on ch.action_id=action.action_id and ch.sub_status_id=210 and ch.is_active='Y' and ch.NEW_OWNER_SID=:loginSID " +
                        " left join persons.person p on p.key=action.LAST_BY where action.action_id=:AID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    isOwner = true;
                    lblATitle.Text = reader.GetString(0);
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);

                    lblADesc.Text = desc.Replace("\r", "<br />");

                    lblAOwner.Text = reader.GetString(2);
                    lblDue.Text = reader.GetString(3);    
                    
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return isOwner;

        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            updateDB("Y");
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }

        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            updateDB("N");
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        private void updateDB(string p_isApproved)
        {
            string _note = txtNote.Text.Trim();

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTION_PKG.PROC_ACTION_TAKE";

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Action_AID;

                    OracleParameter inISApproved = _cmd.Parameters.Add("PI_IS_APPROVED", OracleDbType.Varchar2);
                    inISApproved.Direction = ParameterDirection.Input;
                    inISApproved.Value = p_isApproved;

                    OracleParameter inNote = _cmd.Parameters.Add("PI_REASON", OracleDbType.Varchar2);
                    inNote.Direction = ParameterDirection.Input;
                    inNote.Value = _note;

                    OracleParameter inLogin = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogin.Direction = ParameterDirection.Input;
                    inLogin.Value = _loginSID;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateDB", ex);

            }
        }

        private void BindFileGrid()
        {
            // TBD: 

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = "select ACATT_ID,FILE_NAME, CREATED_ON from SIIMS_ACTION_ATT where action_id=:AID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            lv_Files.DataSource = ds;
            lv_Files.DataBind();

        }

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {

        }

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                FileData(_attachmentId);
            }

        }

        private void FileData(int _attachmentId)
        {

            FileUtil objFile = new FileUtil();
            bool isError = objFile.downLoadAttachment(_attachmentId, 2);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }
    }
}