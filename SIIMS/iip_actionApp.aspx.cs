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
    public partial class iip_actionApp : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Action_AID = 0;
        int _sub_status_ID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("iip_actionApp");

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

            Helpers _helper = new Helpers();
            if (!_helper.IsLabManager(_loginSID))
            {
                // only IIP or Deputy director can do issue approval
                Response.Redirect("action_view.aspx?iid=" + _Action_AID, true);
            }


            if (Request["subid"] != null)
            {
                _sub_status_ID = int.Parse(Request["subid"].ToString());
            }
            else
            {
                Response.Redirect("default.aspx", true);
            }

            if (!Page.IsPostBack && _Action_AID > 0)
            {
                bindControls();

                if (_sub_status_ID == 223)
                {
                    PanelOwnerChange.Visible = true;
                    bindNewOwner(_sub_status_ID);
                }

                if (_sub_status_ID == 220)
                {
                    PanelEdit.Visible = true;
                    bindNewEdit(_sub_status_ID,false);
                }

                if (_sub_status_ID == 221)
                {
                    PanelDelete.Visible = true;
                }

                if (_sub_status_ID == 222)
                {
                    BindFileGrid();
                    PanelClose.Visible = true;
                }

                if (_sub_status_ID == 224)
                {
                    bindNewEdit(_sub_status_ID, true);
                    PanelDueChange.Visible = true;
                }
            }

        }


        private void bindNewEdit(int _sub_status_ID,bool isDuedate)
        {
            string _sql = " select new_title, new_desc, to_char(NEW_DUE_DATE, 'MM/DD/YYYY') as due_date " +
                       " from siims_action_change  action where action.action_id=:AID and sub_status_id=:SubID and is_active='Y' ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
                cmd.Parameters.Add(":SubID", OracleDbType.Int32).Value = _sub_status_ID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if(isDuedate) {
                        lblNewDue.Text = reader.GetString(2);
                    } else {
                         lblNewTitle.Text = reader.GetString(0);
                         string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                         lblNewDesc.Text = desc.Replace("\r", "<br />");
                         //lblNewDesc.Text = reader.GetString(1);
                    }
                 
                    //lblNewDueDate.Text = reader.GetString(2);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindNewEdit", ex);
            }
        }

        private void bindNewOwner(int _sub_status_ID)
        {
            string _sql = " select  p.name,p.key " +
                       " from siims_action_change  action join persons.person p on p.key=action.new_owner_sid where action.action_id=:AID and sub_status_id=223 and is_active='Y' ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lblNewOwner.Text = reader.GetString(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindNewOnwer", ex);
            }
        }

        private void bindControls()
        {

            lblAID.Text = "A" + _Action_AID;

            string _sql = " select   action.TITLE as title, action.description, p.name,to_char(action.due_date, 'MM/DD/YYYY') as due_date " +
                        " from siims_action action left join persons.person p on p.key=action.owner_sid where action.action_id=:AID ";
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
                    lblTitle.Text = reader.GetString(0);
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    lblDesc.Text = desc.Replace("\r", "<br />");

                    lblOwner.Text = reader.GetString(2);
                    lblDueDate.Text = reader.GetString(3);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }


        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            updateDB("Y");
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else if (Request["from"] != null && Request["from"].ToString() == "todo")
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
            else if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
           
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else if (Request["from"] != null && Request["from"].ToString() == "todo")
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
                    _cmd.CommandText = "SIIMS_ACTIONEDIT_PKG.PROC_ACTION_APPROVE";

                    OracleParameter inLogin = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogin.Direction = ParameterDirection.Input;
                    inLogin.Value = _loginSID;

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Action_AID;

                    OracleParameter inSubStatusID = _cmd.Parameters.Add("PI_SUB_STATUS_ID", OracleDbType.Int32);
                    inSubStatusID.Direction = ParameterDirection.Input;
                    inSubStatusID.Value = _sub_status_ID;

                    OracleParameter inISApproved = _cmd.Parameters.Add("PI_IS_APPROVED", OracleDbType.Varchar2);
                    inISApproved.Direction = ParameterDirection.Input;
                    inISApproved.Value = p_isApproved;

                    OracleParameter inNote = _cmd.Parameters.Add("PI_NOTE", OracleDbType.Varchar2);
                    inNote.Direction = ParameterDirection.Input;
                    inNote.Value = _note;



                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateDB", ex);

            }
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
                    sqlCmd.CommandText = "select ACATT_ID,FILE_NAME, CREATED_ON from SIIMS_ACTION_ATT where action_id=:IID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Action_AID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            lv_File.DataSource = ds;
            lv_File.DataBind();


        }
    }
}