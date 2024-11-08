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
using System.IO;

namespace SIIMS
{
    public partial class action_edit : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Action_AID = 0;
        bool isActOwner = false;
        bool isIOwner = false;
        bool isActionLocked = false;
        protected static readonly ILog Log = LogManager.GetLogger("action_edit");

        protected void Page_Load(object sender, EventArgs e)
        {
            string action_type = "e";

            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
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

            if (Request["type"] != null)
            {
                action_type = Request["type"].ToString();
            }
            else
            {
                action_type="e";
            }

           

            Helpers _helper = new Helpers();
            isIOwner = _helper.IsIOwnerOfAction(_loginSID, _Action_AID) || _helper.IsPOCOfAction(_loginSID, _Action_AID) ;
            isActOwner = _helper.IsActionOwner(_loginSID, _Action_AID);

            if (!isIOwner && !isActOwner )
            {
                // only IIP or Deputy director can do issue approval
                Response.Redirect("action_view.aspx?aid=" + _Action_AID, true);
            }

            //if (isActOwner)
            //{
            //    PanelOwnerNote.Visible=true;
            //}

           

            string sub_status = checkPendingRequest();

            if (sub_status.Contains("222"))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "The action is locked out since there is a pending request for closure. ";
                btnSubmit.Visible = false;
                isActionLocked = true;
            }
            else if (sub_status.Contains("221"))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "The action is locked out since there is a pending request for deletion. ";
                btnSubmit.Visible = false;
                isActionLocked = true;
            } else if (sub_status.Contains("220"))
            {
                lblMsg.Visible = true;
                lblMsg.Text = "There is a pending request for modification already. ";
                btnSubmit.Visible = false;
                btnCancel.Visible = false;
            }
          

          

            if (isActionLocked)
            {
                imgResume.Visible = false;
                txtTitle.ReadOnly = true;
                txtDesc.ReadOnly = true;
                btnCancel.Visible = false;
            }

            if (!Page.IsPostBack && _Action_AID > 0)
            {
                int issue_ID = bindControls();
                bindIssueControls(issue_ID);
                BindFileGrid();

            }

        }

        private string checkPendingRequest()
        {

            string sub_status = ",";

            string _sql = " select sub_status_id from siims_action_change where is_active='Y' and sub_status_id in (220,221,222,223,224)  and action_id=:AID ";

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

                    sub_status += reader.GetInt32(0) + ",";

                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkPendingRequest", ex);
            }

            return sub_status;

        }

        private void bindIssueControls(int _Issue_IID)
        {
            lblIssueID.Text = _Issue_IID.ToString();
            string _sql = "   select issue.TITLE,issue.DESCRIPTION,issue.ORG_ID,issue.OWNER_SID,issue.SIG_LEVEL,issue.STYPE_ID, s.TITLE as STitle, s.FY, s.QUARTER,  p.name as owner_name " +
                " , issue.status_id, sta.status, substa.status as sub_status, issue.sub_status_id, stype.TYPE , org.NAME " +
                " from siims_issue issue left join siims_source s on issue.issue_id = s.issue_id left join persons.person p on p.key=owner_sid " +
                "  left join siims_status sta on issue.status_id=sta.status_id and sta.object_id=1 left join siims_status substa on issue.sub_status_id=substa.status_id and substa.object_id=1  " +
                 "   left join SIIMS_SOURCE_TYPE stype on stype.STYPE_ID=issue.stype_id left join siims_org org on org.org_id=issue.org_id where issue.issue_id=:IID ";
            try
            {
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
                    lblITitle.Text = "<a href='issue_view.aspx?iid=" + _Issue_IID + "'>" + reader.GetString(0) + "</a>";
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    desc = desc.Replace("\r", "<br />");
                    lblIDesc.Text = desc;
                    if (desc.Length > 80)
                    {
                        string desc1 = desc.Substring(0, Math.Min(desc.Length, 80));
                        desc1 = desc1 + " ... &nbsp; &nbsp; <a href='javascript:showFull();'>Read More</a>";

                        lblIDescShort.Text = desc1;
                        lblIDescShort.Visible = true;
                        lblIDesc.CssClass = "invisibeText";
                    }
                    else
                    {
                        lblIDescShort.Visible = false;

                    }

                    //lblDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        //lblOrg.Text = reader.GetInt32(2).ToString();
                        lblOrg.Text = reader.GetString(15);
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


                    lblIOwner.Text = reader.GetString(9);



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
                Log.Error("bindIssueControls", ex);
            }
        }

        private int bindControls()
        {

            int issue_ID = 0;
            lblAID2.Text = "A" + _Action_AID.ToString();

            string _sql = " select action.issue_id, action.TITLE as title, action.description, p.name as owner_name,to_char(action.due_date, 'MM/DD/YYYY') as due_date, issue.sig_level, action.note " +
                        " from siims_action action left join persons.person p on p.key=action.owner_sid  join siims_issue issue on issue.issue_id=action.issue_id where action.action_id=:AID and action.is_active='Y' ";
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
                    issue_ID = reader.GetInt32(0);
                    txtTitle.Text = reader.GetString(1);
                    txtDesc.Text = reader.GetString(2);
                    lblAOwner.Text = reader.GetString(3);
                    lblDue.Text = FormatDueDate(reader.GetString(4));
                    lblLevel.Text = reader.GetString(5);
                    //txtOwnerNote.Text = reader.GetString(6);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return issue_ID;
        }

        public string FormatDueDate(string _dueDate)
        {
            DateTime dueDate = DateTime.Parse(_dueDate);
            TimeSpan ts = DateTime.Now.Date - dueDate;
            // Difference in days.
            int differenceInDays = ts.Days;
            if (differenceInDays > 0)
            {
                return " <span style='color:red; font-weight:bold;'>" + _dueDate + " </span>";
            }
            else
            {
                return _dueDate;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (isIOwner)
            {
                updateAction(_Action_AID);
            }
            else if (isActOwner)
            {
                requestChangeAction(_Action_AID);
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

        private void requestChangeAction(int _Action_AID)
        {
            string _title = txtTitle.Text.Trim();
            string _description = txtDesc.Text.Trim();

            //string _dueDate = Request.Form[txtDueDate.UniqueID];

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTIONEDIT_PKG.PROC_ACTIONOWNER_EDIT";


                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inAction_ID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAction_ID.Direction = ParameterDirection.Input;
                    inAction_ID.Value = _Action_AID;


                    OracleParameter inTitle = _cmd.Parameters.Add("PI_NEW_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    inTitle.Value = _title;

                    OracleParameter inDesc = _cmd.Parameters.Add("PI_NEW_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;

                    //OracleParameter inDuedate = _cmd.Parameters.Add("PI_NEW_DUEDATE", OracleDbType.Date);
                    //inDuedate.Direction = ParameterDirection.Input;
                    //if (string.IsNullOrEmpty(_dueDate))
                    //{
                    //    inDuedate.Value = System.DBNull.Value;
                    //}
                    //else
                    //{
                    //    inDuedate.Value = Convert.ToDateTime(_dueDate);
                    //}


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

            }
        }



        private void updateAction(int _Action_AID)
        {
            string _title = txtTitle.Text.Trim();
            string _description = txtDesc.Text.Trim();

            //string _dueDate = Request.Form[txtDueDate.UniqueID];

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTIONEDIT_PKG.PROC_ACTION_EDIT";
                  

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inAction_ID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAction_ID.Direction = ParameterDirection.Input;
                    inAction_ID.Value = _Action_AID;


                    OracleParameter inTitle = _cmd.Parameters.Add("PI_NEW_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    inTitle.Value = _title;

                    OracleParameter inDesc = _cmd.Parameters.Add("PI_NEW_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;

                    //OracleParameter inDuedate = _cmd.Parameters.Add("PI_NEW_DUEDATE", OracleDbType.Date);
                    //inDuedate.Direction = ParameterDirection.Input;
                    //if (string.IsNullOrEmpty(_dueDate))
                    //{
                    //    inDuedate.Value = System.DBNull.Value;
                    //}
                    //else
                    //{
                    //    inDuedate.Value = Convert.ToDateTime(_dueDate);
                    //}

  
                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

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

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ImageButton lnkDelete = (ImageButton)e.Item.FindControl("ImgBtnDelete");
                if (lnkDelete != null)
                {
                    if (isActionLocked)
                    {
                        lnkDelete.Visible = false;
                    }
                    //if (isManager || (!isManager && isMgrLocked))
                    //{
                    //    lnkDelete.Visible = false;
                    //}
                }
            }
        }

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                FileData(_attachmentId);
            }
            if (e.CommandName.ToLower() == "delete")
            {
                DeleteAttachment(_attachmentId);

            }
        }

        // Invoked when the Delete Link is clicked
        protected void DeleteList(Object sender, ListViewDeleteEventArgs e)
        {
            BindFileGrid();
        }



        private void DeleteAttachment(int _attachmentId)
        {
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                string _sql = "UPDATE SIIMS_ACTION_ATT SET IS_ACTIVE='N', LAST_ON=SysDate, LAST_BY=:loginSID WHERE acatt_id=:ACATT_ID";

                try
                {
                    OracleCommand _cmd = new OracleCommand(_sql, _conn);
                    _cmd.BindByName = true;
                    _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                    _cmd.Parameters.Add(":ACATT_ID", OracleDbType.Int32).Value = _attachmentId;
                    _conn.Open();
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.Error("DeleteAttachment", ex);
                }


            }

            imgResume.Focus();
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblMsg.Visible = false;
            if (FileUploadControl.HasFile)
            {
                string[] _allowedextn = { ".doc", ".docx", ".jpg", ".bmp", ".pdf", ".xls", ".xlsx", ".ppt", "pptx", ".png", ".txt" };
                try
                {

                    string filename = Path.GetFileName(FileUploadControl.FileName);
                    int file_size = FileUploadControl.PostedFile.ContentLength;
                    string content_type = FileUploadControl.PostedFile.ContentType;

                    bool isSupportedFile = false;
                    string ErrorMsg = "";
                    ;
                    if (filename.Length > 90)
                    {

                        ErrorMsg = "Error: File Name is too long. Please pick a shorter name and try attaching the file again. ";
                    }


                    if (file_size / 1024 > 31240)
                    {

                        ErrorMsg = "Error: File size exceeded the allowed limit of 30MB. Please pick a different file and try again.  ";
                    }

                    string _lowName = filename.ToLower();
                    if (_lowName.EndsWith(".pdf") || _lowName.EndsWith(".jpg") || _lowName.EndsWith(".bmp") || _lowName.EndsWith(".png") || _lowName.EndsWith(".txt"))
                    {
                        isSupportedFile = true;
                    }


                    if (_lowName.EndsWith(".doc") || _lowName.EndsWith(".docx") || _lowName.EndsWith(".ppt") || _lowName.EndsWith(".pptx") || _lowName.EndsWith(".xls") || _lowName.EndsWith(".xlsx"))
                    {
                        isSupportedFile = true;
                    }


                    if (isSupportedFile && string.IsNullOrEmpty(ErrorMsg))
                    {

                        FileUtil objFile = new FileUtil();
                        Byte[] _filedata = objFile.GetByteArrayFromFileField(FileUploadControl.PostedFile);

                        OracleConnection _conn = null;
                        try
                        {
                            _conn = new OracleConnection(_connStr);
                            _conn.Open();
                            OracleCommand _cmd = new OracleCommand("", _conn);
                            _cmd.CommandText = " Insert Into SIIMS_ACTION_ATT ( ACTION_ID, FILE_NAME,FILE_SIZE ,CONTENT_TYPE,FILE_DATA,CREATED_BY)  Values(:ACTID,:FILE_NAME,:FILE_SIZE,:CONTENT_TYPE,:FILE_DATA,:loginSID)";
                            _cmd.BindByName = true;
                            _cmd.Parameters.Add(":ACTID", OracleDbType.Int32).Value = _Action_AID;
                            _cmd.Parameters.Add(":FILE_NAME", OracleDbType.Varchar2).Value = filename;
                            _cmd.Parameters.Add(":FILE_SIZE", OracleDbType.Int32).Value = file_size;
                            _cmd.Parameters.Add(":CONTENT_TYPE", OracleDbType.Varchar2).Value = content_type;
                            _cmd.Parameters.Add(":FILE_DATA", OracleDbType.Blob).Value = _filedata;
                            _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                            _cmd.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            Log.Error("btnUpload_Click: DB Insertion", ex);
                        }


                        BindFileGrid();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ErrorMsg))
                        {
                            ErrorMsg = "Error: the uploaded file format is not supported.";
                        }
                        lblMsg.Visible = true;
                        lblMsg.Text = ErrorMsg;
                        imgResume.Focus();
                    }


                }
                catch (Exception ex)
                {
                    // do something
                    lblMsg.Text = "Error: " + ex.Message;
                    lblMsg.Visible = true;
                    Log.Error("btnUpload_Click: File Uploading", ex);
                }
            }
            else
            {
                lblMsg.Text = "Error: Please select a file!";
                lblMsg.Visible = true;
                imgResume.Focus();
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

        //protected void btnNote_Click(object sender, EventArgs e)
        //{
        //    string note = txtOwnerNote.Text;


        //    using (OracleConnection _conn = new OracleConnection(_connStr))
        //    {
        //        string _sql = "UPDATE SIIMS_ACTION SET NOTE=:Note, LAST_ON=SysDate, LAST_BY=:loginSID WHERE action_id=:AID";

        //        try
        //        {
        //            OracleCommand _cmd = new OracleCommand(_sql, _conn);
        //            _cmd.BindByName = true;
        //            _cmd.Parameters.Add(":Note", OracleDbType.Varchar2).Value = note;
        //            _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
        //            _cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
        //            _conn.Open();
        //            _cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error("btnNote_Click", ex);
        //        }


        //    }

        //}

      
    }
}