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
    public partial class issue_level : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = 0;
        protected static readonly ILog Log = LogManager.GetLogger("issue_level");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

          
            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);

            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("~/default.aspx", true);
            }

            Helpers _helper = new Helpers();
            if (_helper.IsP1Closue(_Issue_IID))
            {
                Response.Redirect("issue_view.aspx?iid=" + _Issue_IID, true);
            }
            if (!_helper.IsIOwner(_loginSID, _Issue_IID) && !_helper.IsPOC(_loginSID, _Issue_IID))
            {
                // only IIP or Deputy director can do issue approval
                Response.Redirect("issue_view.aspx?iid=" + _Issue_IID, true);
            }

            if (!Page.IsPostBack && _Issue_IID > 0)
            {
                int _sub_sttsu_id = bindControls();
                if (_sub_sttsu_id > 0)
                {
                    btnCancel.Visible = false;
                    btnSubmit.Visible = false;
                }

                if (_sub_sttsu_id == 110)
                {
                    lblLockout.Visible = true;
                    lblLockout.Text = "The issue is locked out since there is a pending request for modification. ";
                }
                else if (_sub_sttsu_id == 111)
                {
                    lblLockout.Visible = true;
                    lblLockout.Text = "The issue is locked out since there is a pending request for deletion. ";
                }

                BindFileGrid();
            }
          
        }

        private int bindControls()
        {
            int _sub_Sattus_ID = 0;

            string _sql = "   select TITLE,DESCRIPTION,ORG_ID,OWNER_SID,SIG_LEVEL,STYPE_ID,  STitle, FY, QUARTER, owner_name " +
                    " , status_id, status, sub_status, sub_status_id, TYPE , NAME " +
                    " from VW_SIIMS_ISSUE_VIEW where issue_id=:IID ";
            try
            {
                lblIssueID.Text = _Issue_IID.ToString();
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTitle.Text = reader.GetString(0);
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    lblDesc.Text = desc.Replace("\r", "<br />");
                    //lblDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        //lblOrg.Text = reader.GetInt32(2).ToString();
                        lblOrg.Text = reader.GetString(15);
                    }


                    string status = "";
                    if (!reader.IsDBNull(11))
                    {
                        status = reader.GetString(11);

                        if (!reader.IsDBNull(13) && status == "Open")
                        {
                            status += ": " + reader.GetString(12);
                            _sub_Sattus_ID = reader.GetInt32(13);
                        }
                        lblStatus.Text = status;
                    }



                    if (!reader.IsDBNull(4))
                    {
                        lblLevel.Text = reader.GetString(4);
                    }

                    string _sType = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    if (_sType.Length > 0)
                    {
                        lblSType.Text = reader.GetString(14);

                        lblSourceTitle.Text = reader.GetString(6);
                        lblSourceFY.Text = reader.GetString(7);
                        lblSourceQtr.Text = reader.GetString(8);
                    }


                    lblOwner.Text = reader.GetString(9);



                }
                else
                {
                    Response.Redirect("permission.aspx");
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return _sub_Sattus_ID;
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
                    sqlCmd.CommandText = "select ISATT_ID,FILE_NAME, CREATED_ON from SIIMS_ISSUE_ATT where issue_id=:IID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            lv_Files.DataSource = ds;
            lv_Files.DataBind();

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
          
            string _level = drwLevel.SelectedValue;
            if (_level == lblLevel.Text)
            {
                lblLevelCheck.Text = "The new level must be different from the existing level.";
                lblLevelCheck.Visible = true;
                return;
            }
           

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ISSUE_PKG.PROC_ISSUE_LEVELCHANGE";

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Issue_IID;    

                    OracleParameter inLevel = _cmd.Parameters.Add("PI_LEVEL", OracleDbType.Varchar2);
                    inLevel.Direction = ParameterDirection.Input;
                    inLevel.Value = _level;

                    OracleParameter inLogin = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogin.Direction = ParameterDirection.Input;
                    inLogin.Value = _loginSID;                

                    

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("btnSubmit_Click", ex);

            }

            if (Request["from"] != null && Request["from"].ToString() == "org")
            {
                Response.Redirect("issue_org.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
         
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            if (Request["from"] != null && Request["from"].ToString() == "org")
            {
                Response.Redirect("issue_org.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        public string FORMATACTIONStatus(string status, string due_date)
        {
            string msg = "";
            if (status == "Open")
            {
                msg = due_date;
            }
            else
            {
                msg = status;
            }

            return msg;
        }


        protected void GVActions_RowDataBound(object sender, GridViewRowEventArgs e)
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
            bool isError = objFile.downLoadAttachment(_attachmentId, 1);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }
     
    }
}