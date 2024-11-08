using log4net;
using Oracle.ManagedDataAccess.Client;
using SIIMS.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class action1 : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        int _Action_AID = -1;
        int _ActionType = 1;
        protected static readonly ILog Log = LogManager.GetLogger("action1");

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isDraftAndInitiator = false;
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

        

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            txtDueDate.Attributes.Add("readonly", "readonly");
            if (Request["type"] != null)
            {
                _ActionType = int.Parse(Request["type"].ToString());
                if(_ActionType==2)
                {
                    PanelType2.Visible = true;
                    litHeader.Text = "Recommended Action";
                }
            }
            else
            {
                Response.Redirect("~/permission.aspx", true);
            }

            if (Request["aid"] != null)
            {
                _Action_AID = int.Parse(Request["aid"].ToString());
                // check if the report is Draft. If it is, the report initiatior could edit the action.
                isDraftAndInitiator = checkIfInitiator(_Action_AID);
            }
            else
            {
                if (Request["iid"] != null)
                {
                    _Issue_IID = int.Parse(Request["iid"].ToString());                   
                }
                else
                {
                    Response.Redirect("~/permission.aspx", true);
                }
            }


            int userType = 0;

            if (!Page.IsPostBack)
            {
                if (_Action_AID > 0)
                {
                    // edit action
                    userType = checkUserTypebyActionID(_Action_AID);
                    btnDelete.Visible = true;

                    _Issue_IID = bindActionControls();
                    Log.Debug("Action ID:" + _Action_AID + "    Issue ID: " + _Issue_IID);

                }
                else if (_Issue_IID > 0)
                {
                    // add new action
                    userType = checkUserType(_Issue_IID);
                    btnDelete.Visible = false;
                }

                if (userType == 0 & !isDraftAndInitiator)
                {
                    Response.Redirect("../permission.aspx");
                }
                bindIssueControls(_Issue_IID);

                BindFileGrid();
                //btnSave.Attributes.Add("onclick", "javascript: if(Page_ClientValidate('allFields')) " + btnSave.ClientID + ".disabled = true; " + ClientScript.GetPostBackEventReference(btnSave, ""));

            }

            //Log.Debug("Issue_id: " + _Issue_IID + "    Login_SID: " + _loginSID + "     UserType:" + userType)
        }

        private bool checkIfInitiator(int _aid)
        {
            bool isEnabled = false;

            string _sql = @"select count(*) from siims_action act
                                join siims_issue issue on issue.issue_id=act.issue_id
                                join siims_rir_report rep on rep.issue_id=act.issue_id
                                where act.action_id=:AID and rep.rir_status='D' and issue.created_by=:SID";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _aid;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = _loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
               if(reader.HasRows)
                {
                    isEnabled = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkIfInitiator", ex);
            }

            return isEnabled;
        }


        private void bindIssueControls(int _Issue_IID)
        {
            lblIssueID.Text = _Issue_IID.ToString();
            string _sql = @"select issue.issue_id, issue.TITLE, p.name as initiator,to_char(rir.date_discovered, 'MM/DD/YYYY') as date_disc , org.NAME as org, issue.dept_name
                            , to_char(issue.created_on,'MM/DD/YYYY') as date_initiated, poc.name as POC
                                      from siims_issue issue 
                                      join siims_rir_report rir on rir.issue_id=issue.issue_id
                                      join persons.person p on p.key=issue.created_by  
                                      left join persons.person poc on poc.key=rir.POC_SID
                                    left join siims_org org on org.org_id=issue.org_id 
                                    where issue.issue_id=:IID";
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
                    lblIssueID.Text = reader.GetInt32(0).ToString();
                    lblITitle.Text = reader.GetString(1);
                    lblInitiator.Text = reader.GetString(2);
                    lblDate_Disc.Text= reader.IsDBNull(3) ? "" : reader.GetString(3);
                    lblOrg.Text= reader.IsDBNull(4) ? "" : reader.GetString(4);
                    lblDept.Text= reader.IsDBNull(5) ? "" : reader.GetString(5);
                    lblDateInit.Text = reader.GetString(6);
                    lblPOC.Text= reader.IsDBNull(7) ? "" : reader.GetString(7);
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

        private int bindActionControls()
        {
            int issue_ID = 0;
            string _sql = @"  select  action.issue_id ,  action.description,to_char(action.due_date, 'MM/DD/YYYY') as due_date ,p.name,action.owner_sid 
                                            from siims_action action 
                                        left join persons.person p on p.key=action.owner_sid 
                                        where action.action_id=:AID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Varchar2).Value = _Action_AID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    issue_ID = reader.GetInt32(0);

                    txtDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    string dueDate = reader.IsDBNull(2) ? "" : reader.GetString(2).ToString();
                    txtDueDate.Text = dueDate;

                    string poc_name = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    txtPOCName.Text = poc_name;
                    int poc_sid = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                    //Log.Debug("POC Name:" + poc_name + "   SID:" + poc_sid);
                    txtPOC_SID.Value = poc_sid.ToString();

                    if (poc_sid != -1)
                    {
                        PnlNamePOC.Visible = true;
                        PnlPopupPOC.Visible = false;
                        BtnChangePOC.Visible = true;
                        divtrPOC.Visible = true;
                    }


                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindActionControls", ex);
            }

            return issue_ID;
        }

        /// <summary>
        ///  The input is the issue_id of a report
        ///  The ooutput is the type of the user for this report. Type could be
        ///  0: regular user with any view permission
        ///  1: the report initiator, can edit during Draft and Edit report_status
        ///  2: reviewer, could edit and review the report
        ///  3: RIR coordinator: could do anything.
        ///  /// </summary>
        /// <param name="issue_id"></param>
        /// <returns></returns>

        private int checkUserTypebyActionID(int action_id)
        {
            int userType = 0;
            string _sql = @"select  case when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='Y' and IS_ACTIVE='Y')=1 then '3'
                                            when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y')=1 then '2'
                                            when (select count(*) as type1 from siims_action where created_by=:loginSID and action_id=:AID)=1 then '1'
                                            ELSE '0'
                                            END as type, (select issue_id from siims_action where action_id=:AID) as issue_id
                                            from dual ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = action_id;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userType = int.Parse(reader.GetString(0));
                    _Issue_IID = reader.GetInt32(1);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkUserTypebyActionID", ex);
            }

            return userType;
        }

        private int checkUserType(int issue_id)
        {
            int userType = 0;
            string _sql = @"select  case when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='Y' and IS_ACTIVE='Y')=1 then '3'
                                            when (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y')=1 then '2'
                                            when (select count(*) as type1 from siims_issue where created_by=:loginSID and issue_id=:IID)=1 then '1'
                                            ELSE '0'
                                            END as type
                                            from dual";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = issue_id;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userType = int.Parse(reader.GetString(0));
                    Log.Debug("UserType in DB:" + userType);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkUserType", ex);
            }

            return userType;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_Action_AID > 0)
            {
                updateAction(_Action_AID);
            }
            else
            {
                save2DB();
            }
            //System.Threading.Thread.Sleep(2000);
            goBackURL();
        }

        private void save2DB()
        {
            string _description = txtDesc.Text.Trim();
            string _owner_sid = "-1";
            string _dueDate = "";
            string _is_Imm = "Y";
            if (_ActionType==2)
            {
                _owner_sid = txtPOC_SID.Value;
                _dueDate = Request.Form[txtDueDate.UniqueID];
                _is_Imm = "N";
            }


            Log.Debug("Owner SID:" + _owner_sid + "    Due Date:" + _dueDate + "    IS_IMMEDIATE:" + _is_Imm);

            string session_ID = HiddenField_ATTSESSIONID.Value;

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_CREATE_ACTION";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Issue_IID;

                 
                    OracleParameter inDesc = _cmd.Parameters.Add("PI_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;

                    OracleParameter inOwnerSID = _cmd.Parameters.Add("PI_OWNER_SID", OracleDbType.Int32);
                    inOwnerSID.Direction = ParameterDirection.Input;
                    if (_owner_sid == "-1")
                    {
                        inOwnerSID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inOwnerSID.Value = int.Parse(_owner_sid);
                    }

                    OracleParameter inDuedate = _cmd.Parameters.Add("PI_DUEDATE", OracleDbType.Date);
                    inDuedate.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_dueDate))
                    {
                        inDuedate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDuedate.Value = Convert.ToDateTime(_dueDate);
                    }

                    OracleParameter inIsImm = _cmd.Parameters.Add("PI_IS_IMM", OracleDbType.Varchar2);
                    inIsImm.Direction = ParameterDirection.Input;
                    inIsImm.Value = _is_Imm;

                    OracleParameter inSessionID = _cmd.Parameters.Add("PI_ATT_SESSIONID", OracleDbType.Varchar2);
                    inSessionID.Direction = ParameterDirection.Input;
                    inSessionID.Value = session_ID;

                    OracleParameter outIssue_ID = _cmd.Parameters.Add("PO_ACTION_ID", OracleDbType.Int32);
                    outIssue_ID.Direction = ParameterDirection.Output;

                    //_Action_AID = int.Parse(outIssue_ID.Value.ToString());
                    //Log.Debug("New Created Action ID:" + _Action_AID);
                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("save2DB", ex);

            }
        }


        private void updateAction(int _Action_AID)
        {

            string _description = txtDesc.Text.Trim();
            string _owner_sid = "-1";
            string _dueDate = "";
            string _is_Imm = "Y";
            if (_ActionType == 2)
            {
                _owner_sid = txtPOC_SID.Value;
                _dueDate = Request.Form[txtDueDate.UniqueID];
                _is_Imm = "N";
            }


            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_UPDATE_ACTION";

                    OracleParameter inAID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAID.Direction = ParameterDirection.Input;
                    inAID.Value = _Action_AID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inDesc = _cmd.Parameters.Add("PI_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;

                    OracleParameter inOwnerSID = _cmd.Parameters.Add("PI_OWNER_SID", OracleDbType.Int32);
                    inOwnerSID.Direction = ParameterDirection.Input;
                    if (_owner_sid == "-1")
                    {
                        inOwnerSID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inOwnerSID.Value = int.Parse(_owner_sid);
                    }

                    OracleParameter inDuedate = _cmd.Parameters.Add("PI_DUEDATE", OracleDbType.Date);
                    inDuedate.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_dueDate))
                    {
                        inDuedate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDuedate.Value = Convert.ToDateTime(_dueDate);
                    }

                    OracleParameter inIsImm = _cmd.Parameters.Add("PI_IS_IMM", OracleDbType.Varchar2);
                    inIsImm.Direction = ParameterDirection.Input;
                    inIsImm.Value = _is_Imm;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

            }
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string _sql = "Update SIIMS_ACTION set IS_ACTIVE='N', LAST_ON=sysdate, LAST_BY=:LoginSID where action_id=:AID";
            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.CommandText = _sql;
                    _cmd.BindByName = true;
                    _cmd.Parameters.Add(":LoginSID", OracleDbType.Int32).Value = _loginSID;
                    _cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("btnDelete_Click", ex);

            }
            goBackURL();
        }

        private int getIssueID()
        {
            int issue_ID = 0;
            string _sql = @"  select  issue_id  from siims_action where action_id=:AID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Varchar2).Value = _Action_AID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    issue_ID = reader.GetInt32(0);

                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("getIssueID", ex);
            }

            return issue_ID;
        }
        protected void goBackURL()
        {
            if (_Issue_IID <= 0) _Issue_IID = getIssueID();

            string fromURL = "d";
            if (Request.QueryString["from"] != null)
            {
                fromURL = Request.QueryString["from"].ToString();
            }
            if (fromURL == "e")
            {
                Response.Redirect("create.aspx?from=a&iid=" + _Issue_IID);
            }
            else if (fromURL == "v")
            {
                Response.Redirect("report_view.aspx?iid=" + _Issue_IID);
            } else
            {
                Response.Redirect("rir.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            goBackURL();
        }


        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ImageButton lnkDelete = (ImageButton)e.Item.FindControl("ImgBtnDelete");
                if (lnkDelete != null)
                {
                    //if (isActionLocked)
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

                        ErrorMsg = "Error: File size exceeded the allowed limit of 30MB. Please pick a different file and try again. ";
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

                        string sessionID = HiddenField_ATTSESSIONID.Value;
                        if (sessionID == "-1")
                        {
                            string ATTSession_ID = DateTime.Now.ToString("MMddHHmmss");
                            HiddenField_ATTSESSIONID.Value = ATTSession_ID;
                            sessionID = ATTSession_ID;
                        }

                        FileUtil objFile = new FileUtil();
                        Byte[] _filedata = objFile.GetByteArrayFromFileField(FileUploadControl.PostedFile);

                        OracleConnection _conn = null;
                        try
                        {
                            _conn = new OracleConnection(_connStr);
                            _conn.Open();
                            OracleCommand _cmd = new OracleCommand("", _conn);
                            _cmd.CommandText = " Insert Into SIIMS_ACTION_ATT ( ACTION_ID, FILE_NAME,FILE_SIZE ,CONTENT_TYPE,FILE_DATA,CREATED_BY,TEMP_ID)  Values(:ACTID,:FILE_NAME,:FILE_SIZE,:CONTENT_TYPE,:FILE_DATA,:loginSID,:TempID)";
                            _cmd.BindByName = true;
                            if (_Action_AID == -1)
                            {
                                _cmd.Parameters.Add(":ACTID", OracleDbType.Int32).Value = System.DBNull.Value;
                            }
                            else
                            {
                                _cmd.Parameters.Add(":ACTID", OracleDbType.Int32).Value = _Action_AID;
                            }

                            _cmd.Parameters.Add(":FILE_NAME", OracleDbType.Varchar2).Value = filename;
                            _cmd.Parameters.Add(":FILE_SIZE", OracleDbType.Int32).Value = file_size;
                            _cmd.Parameters.Add(":CONTENT_TYPE", OracleDbType.Varchar2).Value = content_type;
                            _cmd.Parameters.Add(":FILE_DATA", OracleDbType.Blob).Value = _filedata;
                            _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                            _cmd.Parameters.Add(":TempID", OracleDbType.Varchar2).Value = sessionID;
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
                            ErrorMsg = "Error: the uploaded file format is not support.";
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
            if (_Action_AID > 0)
            {

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
            }
            else
            {
                string sessionID = HiddenField_ATTSESSIONID.Value;
                using (OracleConnection _conn = new OracleConnection(_connStr))
                {
                    using (OracleCommand sqlCmd = new OracleCommand())
                    {
                        sqlCmd.Connection = _conn;
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.BindByName = true;
                        sqlCmd.CommandText = "select ACATT_ID,FILE_NAME, CREATED_ON from SIIMS_ACTION_ATT where temp_id=:TempID and IS_ACTIVE='Y' ";
                        sqlCmd.Parameters.Add(":TempID", OracleDbType.Int32).Value = sessionID;
                        using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                        {
                            sqlAdp.Fill(ds);
                        }
                    }
                }
            }


            lv_File.DataSource = ds;
            lv_File.DataBind();

            imgResume.Visible = true;

        }

        protected void BtnChangePOC_Click(object sender, EventArgs e)
        {
            changeButtonClickGen(PnlPopupPOC, txtPOCName, cmdokPOC, ddlEmplistPOC, txtPOC, CmdCancelPOC, PnlNamePOC);
        }

        protected void changeButtonClickGen(Panel pnlPopup, TextBox txtContactName, Button cmdOK, DropDownList DDlEmplist, TextBox txtContact, Button cmdCancel, Panel pnlName)
        {
            pnlPopup.Visible = true;
            if (pnlName != null)
            { pnlName.Visible = false; }

            //can be removed if the panel is made visible
            txtContactName.Visible = true;
            cmdOK.Visible = false;
            DDlEmplist.Visible = false;
            txtContact.Text = "";
            txtContact.Focus();
            cmdCancel.Visible = true;
        }



        protected void cmdokPOC_Click(object sender, EventArgs e)
        {
            okButtonClickGen(cmdokPOC, PnlPopupPOC, txtPOC_SID, txtPOCName, LblerrorPOC, BtnChangePOC, ddlEmplistPOC);
            divtrPOC.Visible = true;
            PnlNamePOC.Visible = true;
        }

        protected void okButtonClickGen(Button cmdOk, Panel pnlPopup, HiddenField txtSid, TextBox txtName, Label lblError, Button btnChange, DropDownList ddlEmplist)
        {
            cmdOk.Enabled = false;
            string _matrix_name = splitname(ddlEmplist.SelectedItem.Text);
            string _matrix_sid = ddlEmplist.SelectedValue;

            if (string.IsNullOrEmpty(_matrix_sid))
            {
                lblError.Visible = true;
                lblError.Text = "You have to select one person from the dropdown list.";
                return;
            }

            setContact(pnlPopup, btnChange, txtName, txtSid, _matrix_name, _matrix_sid);

        }

        public static string splitname(string name)
        {
            string[] mAr = null;
            string temp = null;


            mAr = name.Split('-');
            temp = mAr[0].Trim();
            return temp;
        }

        protected void setContact(Panel pnlpopup, Button btnChange, TextBox txtname, HiddenField txtvalue, string name, string sidVal)
        {
            pnlpopup.Visible = false;
            //can be replaced by panel

            btnChange.Visible = true;
            txtname.Visible = true;
            txtname.Text = name;
            txtvalue.Value = sidVal;
        }

        protected void CmdCancelldr_Click(object sender, EventArgs e)
        {
            PnlPopupPOC.Visible = false;
            PnlNamePOC.Visible = true;
            SetFocus(txtPOCName.ClientID);
        }

        protected void CmdFindPOC_Click(object sender, EventArgs e)
        {
            findButtonClickGen(txtPOC, LblerrorPOC, ddlEmplistPOC, cmdokPOC);
        }

        protected void findButtonClickGen(TextBox TxtName, Label lblError, DropDownList ddlEmpList, Button cmdOk)
        {
            string errorMsg = "Please enter the first few characters to start your search";
            string lastName = TxtName.Text.Trim();

            if (string.IsNullOrEmpty(lastName))
            {
                lblError.Text = errorMsg;
                lblError.Visible = true;
                ddlEmpList.Visible = false;
                cmdOk.Visible = false;
                return;
            }
            lblError.Visible = false;
            ddlEmpList.Visible = true;
            cmdOk.Visible = true;

            string _sql = "select distinct p.key, p.name, dept.description as dept_name from persons.person p join SID.organizations dept on p.dept_id=dept.org_id " +
             " join  but b on b.but_sid=p.key and b.But_ldt='win' " +
              " where p.gonet='ACTIVE' and p.status='EMP' and LOWER(p.name) LIKE LOWER(:PName) || '%'  ORDER BY p.name";

            OracleConnection con = new OracleConnection();
            con.ConnectionString = _connStr;
            con.Open();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":PName", OracleDbType.Varchar2).Value = lastName;
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    ddlEmpList.Items.Clear();
                    try
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ListItem NewItem = new ListItem();
                                string _name = Convert.ToString(reader["name"]);
                                string _slac_id = Convert.ToString(reader["key"]);
                                string _dept = Convert.ToString(reader["dept_name"]);
                                NewItem.Text = _name + " - " + _slac_id + " - " + _dept;
                                NewItem.Value = _slac_id;
                                ddlEmpList.Items.Add(NewItem);
                                cmdOk.Enabled = true;
                            }
                        }
                        else
                        {
                            lblError.Visible = true;
                            lblError.Text = "Name is not in directory.";
                            ddlEmpList.Visible = false;
                            cmdOk.Visible = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.Error(" Error in findButtonClickGen: ", ex);
                    }
                }

            }

        }
    }
}