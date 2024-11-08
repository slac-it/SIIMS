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
    public partial class action : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = 0;
        int _Action_AID =-1;
        protected static readonly ILog Log = LogManager.GetLogger("action");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
          

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);
    
             Helpers _helper = new Helpers();

            if (Request["aid"] != null)
            {
                _Action_AID = int.Parse(Request["aid"].ToString());
                if (!_helper.IsIOwnerOfAction(_loginSID, _Action_AID) && !_helper.IsPOCOfAction(_loginSID, _Action_AID))
                {
                    Response.Redirect("permission.aspx");
                }

            }
            else
            {
                if (Request["iid"] != null)
                {
                    _Issue_IID = int.Parse(Request["iid"].ToString());
                    // if the issue is pending for closure, no action creations are allowed.
                    if(_helper.IsP1Closue(_Issue_IID))
                    {
                        Response.Redirect("default.aspx", true);
                    }
                    if (!_helper.IsIOwner(_loginSID, _Issue_IID) && !_helper.IsPOC(_loginSID, _Issue_IID))
                    {
                        // only IIP or Deputy director can do issue approval
                        Response.Redirect("~/permission.aspx", true);
                    }
                }
                else
                {
                    Response.Redirect("~/permission.aspx", true);
                }
            }


            txtDate.Attributes.Add("readonly", "readonly");

            if (!Page.IsPostBack)
            {
                if (_Action_AID > 0)
                {
                  
                   _Issue_IID= bindActionControls();
                   bindIssueControls(_Issue_IID);
                }
                else
                {
                    litHeader.Text = "Create Action";
                    PnlPopup.Visible = true;
                    //lblOwner.Visible = true;
                    //txtOwner_Name.Visible = true;
                    btnChangeOwner.Visible = false;
                    btnDelete.Visible = false;
                    bindIssueControls(_Issue_IID);
                }

                BindFileGrid();
               
            }
            else
            {
                btnDelete.Visible = false;
            }


          
        }

        private bool checkActionAuthorized(int item_id, int type)
        {
            bool isAuthorized = false;
            string _sql;

            if (type == 1)
            {
                _sql = "select action_id from siims_action where created_by= :loginSID and action_id=:ITEMID ";
            }
            else
            {
                _sql = "select issue_id from siims_issue where owner_SID=:loginSID and issue_id=:ITEMID";
            }
           

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                cmd.Parameters.Add(":ITEMID", OracleDbType.Int32).Value = item_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    isAuthorized = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkActionAuthorized", ex);
            }

            return isAuthorized;
        }

        private void bindIssueControls(int _Issue_IID)
        {
            lblIssueID.Text = _Issue_IID.ToString();
            string _sql = "   select TITLE,DESCRIPTION,ORG_ID,OWNER_SID,SIG_LEVEL,STYPE_ID,  STitle, FY, QUARTER, owner_name " +
                " , status_id, status, sub_status, sub_status_id, TYPE , NAME " +
                " from VW_SIIMS_ISSUE_VIEW where issue_id=:IID ";
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
                    lblITitle.Text = reader.GetString(0);
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

        private int bindActionControls()
        {
            litHeader.Text = "Complete Action";
            int issue_ID = 0;
            string _sql = "  select  action.issue_id as issue_id, issue.title as ititle, action.TITLE as atitle, action.description,p.key, p.name,to_char(action.due_date, 'MM/DD/YYYY') as due_date " +
                            ", action.status_id, action.sub_status_id from siims_action action join siims_issue issue on action.issue_id=issue.issue_id and issue.is_active='Y' " +
                            " left join persons.person p on p.key=action.owner_sid where action.action_id=:AID ";
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
                    //string _title = "ID: " + reader.GetInt32(0) + "   Title: " + reader.GetString(1);
                    //lblIssue.Text = _title;

                    txtTitle.Text = reader.GetString(2);
                    txtDesc.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);

                    txtOwner_SID.Value = reader.IsDBNull(4) ? "-1" : reader.GetInt32(4).ToString();

                    string ownerName = reader.IsDBNull(5) ? "" : reader.GetString(5).ToString();
                    txtOwner_Name.Text= ownerName;

                    if (string.IsNullOrEmpty(ownerName))
                    {
                        PnlPopup.Visible = true;
                        txtOwner_Name.Visible = true;
                        btnChangeOwner.Visible = false;
                    }
                    else
                    {
                        PnlPopup.Visible = false;
                        txtOwner_Name.Visible = true;
                        btnChangeOwner.Visible = true;
                    }

                    string dueDate = reader.IsDBNull(6) ? "" : reader.GetString(6).ToString();
                    txtDate.Text= dueDate;

                    int status_id=reader.GetInt32(7);
                    int sub_status_id = reader.IsDBNull(8) ? -1 : reader.GetInt32(8);

                    if (status_id == 20)
                    {
                        btnSave.Visible = true;
                        btnDelete.Visible = true;
                    }
                    else if (status_id == 21)
                    {
                        btnSave.Visible = false;
                        btnDelete.Visible = false;
                        if (sub_status_id == 210)
                        {
                            litHeader.Text = "Edit Action";
                            btnChangeOwner.Visible = false;
                        }

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_Action_AID > 0)
            {
                updateAction(_Action_AID,"Y");
            }
            else
            {
                save2DB("Y");
            }

            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }

        private void updateAction(int _Action_AID, string is_Draft)
        {
            string _title = txtTitle.Text.Trim();
            string _description = txtDesc.Text.Trim();
            string _owner_sid = txtOwner_SID.Value;
            string _dueDate = Request.Form[txtDate.UniqueID];


            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ACTION_PKG.PROC_ACTION_DRAFT_UPDATE";

                    OracleParameter inAID= _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inAID.Direction = ParameterDirection.Input;
                    inAID.Value = _Action_AID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inIsDraft = _cmd.Parameters.Add("PI_IS_DRAFT", OracleDbType.Varchar2);
                    inIsDraft.Direction = ParameterDirection.Input;
                    inIsDraft.Value = is_Draft;

                    OracleParameter inTitle = _cmd.Parameters.Add("PI_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    inTitle.Value = _title;
                    OracleParameter inDesc = _cmd.Parameters.Add("PI_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;


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

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateAction", ex);

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_Action_AID > 0)
            {
                updateAction(_Action_AID, "N");
            }
            else
            {
                save2DB("N");
            }

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

        private void save2DB(string is_Draft)
        {
            string _title = txtTitle.Text.Trim();
            string _description = txtDesc.Text.Trim();
            string _owner_sid = txtOwner_SID.Value;
            string _dueDate = Request.Form[txtDate.UniqueID];
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
                    _cmd.CommandText = "SIIMS_ACTION_PKG.PROC_ACTION_CREATION";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                     OracleParameter inIssueID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Issue_IID;

                    OracleParameter inIsDraft = _cmd.Parameters.Add("PI_IS_DRAFT", OracleDbType.Varchar2);
                    inIsDraft.Direction = ParameterDirection.Input;
                    inIsDraft.Value = is_Draft;

                    OracleParameter inTitle = _cmd.Parameters.Add("PI_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    inTitle.Value = _title;
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

                    OracleParameter inSessionID = _cmd.Parameters.Add("PI_ATT_SESSIONID", OracleDbType.Varchar2);
                    inSessionID.Direction = ParameterDirection.Input;
                    inSessionID.Value = session_ID;

                    OracleParameter outIssue_ID = _cmd.Parameters.Add("PO_ACTION_ID", OracleDbType.Int32);
                    outIssue_ID.Direction = ParameterDirection.Output;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("save2DB", ex);

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


        protected void cmdFind_Click(object sender, EventArgs e)
        {
            string errorMsg = "Please enter the first few characters to start your search";

            string lastName = txtOwner.Text.Trim();

            if (string.IsNullOrEmpty(lastName))
            {
                lblError.Text = errorMsg;
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;
            ddlEmplist.Visible = true;
            cmdOk.Visible = true;

            string _sql = "select distinct p.key, p.name, dept.description as dept_name from persons.person p join SID.organizations dept on p.dept_id=dept.org_id " +
                   " join sid.person sp on p.key=sp.person_id join but b on b.but_sid=p.key and b.But_ldt='win' " +
                    " where p.gonet='ACTIVE' and p.status in ('CONTRACTOR','EMP','CONSULTANT','SU') and LOWER(p.name) LIKE LOWER(:PName) || '%'  ORDER BY p.name";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":PName", OracleDbType.Varchar2).Value = lastName;

                OracleDataReader _reader = cmd.ExecuteReader();

                ddlEmplist.Items.Clear();
                if (_reader.HasRows)
                {
                    ddlEmplist.Visible = true;
                    cmdOk.Visible = true;

                    lblError.Visible = false;
                    while (_reader.Read())
                    {
                        ListItem NewItem = new ListItem();
                        string _name = Convert.ToString(_reader["name"]);
                        string _slac_id = Convert.ToString(_reader["key"]);
                        string _dept = Convert.ToString(_reader["dept_name"]);
                        //NewItem.Text = _name + " : " + _slac_id + " @ " + _dept;
                        NewItem.Text = _name + " - " + _slac_id + " - " + _dept;
                        //NewItem.Text = _name;
                        NewItem.Value = _slac_id;
                        ddlEmplist.Items.Add(NewItem);
                        cmdOk.Enabled = true;
                    }
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Name is not in directory.";
                    ddlEmplist.Visible = false;
                    cmdOk.Visible = false;
                }

                _reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("cmdFind_Click", ex);
            }
        }

        protected void cmdOk_Click(object sender, EventArgs e)
        {
            cmdOk.Enabled = false;
            //btnBack.Visible = true;
            string _matrix_name = ddlEmplist.SelectedItem.Text;
            string _matrix_sid = ddlEmplist.SelectedValue;

            if (string.IsNullOrEmpty(_matrix_sid))
            {
                lblError.Visible = true;
                lblError.Text = "You have to select one person from the dropdown list.";
                return;
            }

            PnlPopup.Visible = false;
            txtOwner_SID.Value = _matrix_sid;
            txtOwner_Name.Visible = true;
            btnChangeOwner.Visible = true;
            txtOwner_Name.Text = _matrix_name;
        }

       

        protected void btnChangeOwner_Click(object sender, EventArgs e)
        {
            PnlPopup.Visible = true;
            txtOwner_Name.Visible = true;
            btnChangeOwner.Visible = false;
            cmdOk.Visible = false;
            ddlEmplist.Visible = false;
            //txtOwner_SID.Value = "";
            //txtOwner_Name.Text = "";
            txtOwner_Name.Visible = true;
            txtOwner.Focus();
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
    }
}