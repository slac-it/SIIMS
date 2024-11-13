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
    public partial class report_view : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        string _RIR_Status;
        bool is_Owner = false;
        protected static readonly ILog Log = LogManager.GetLogger("report_view");
        protected void Page_Load(object sender, EventArgs e)
        {
            int is_reviewer = 0;
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                //getUserSID();
                //getSSOSID();

                // login SSO info
                string _ssoTest = System.Web.Configuration.WebConfigurationManager.AppSettings["ssoTest"];
                getVar.AttSid = Request.ServerVariables["SSO_SID"];
                getVar.AttLoginName = Request.ServerVariables["SSO_FIRSTNAME"];
                getVar.AttEmail = Request.ServerVariables["SSO_EMAIL"];
                getVar.Visible = (_ssoTest == "1" ? true : false);

                getVar.getSSOSID1();

                // end login SSO info

            }

            if (Session["IS_OWNER"].ToString() == "1") is_Owner = true;
            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (_loginSID == 0)
            {
                Response.Redirect("../permission.aspx", true);
            }
  
          
            int is_done = 0;
            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
                is_reviewer = checkUserType(_Issue_IID, out is_done);

                //Log.Debug("Issue_id: " + _Issue_IID + "    Login_SID: " + _loginSID + "     UserType:" + userType);
                // only the original initiator can edit draft
      
            } else
            {
                Response.Redirect("../permission.aspx", true);
            }

            string is_Dist = "N";
            _RIR_Status = bindControls(out is_Dist);
            if(_RIR_Status != "D" && _RIR_Status != "E")
            {
                Panelpprover.Visible = true;
                if(_RIR_Status != "R" && is_Dist=="Y")
                {
                    PanelDist.Visible = true;
                }
            }

            if (!Page.IsPostBack )
            {
                BindFileGrid();
                BindImAction();
                BindReAction();

            }

            if(_RIR_Status == "E" && is_Owner)
            {
                btnReview.Visible = true;
                Panel2Reviewer.Visible = true;
            }

            if (_RIR_Status == "R" && is_reviewer == 1 && is_done==0)
            {
                btnReview.Visible = false;
                PanelApproveNote.Visible = true;
                btnApprove.Visible = true;
                btnReject.Visible = true;
            }

            if (_RIR_Status == "A" && is_Owner)
            {
                btnReview.Visible = false;
                PanelApproveNote.Visible = true;
                btnApprove.Visible = true;
                btnReject.Visible = true;
                linkDist.Visible = true;
                linkDist.NavigateUrl = "report_dist.aspx?from=v&iid=" + _Issue_IID;
            }

            if(_RIR_Status == "C")
            {
                PnlLevel.Visible = true;
            }

        }

        private void BindImAction()
        {
            string _sql = @"select ROW_NUMBER() OVER(ORDER BY created_on) AS  Imme_No  ,action_id, DESCRIPTION   from siims_action   where issue_id = :IID and is_active='Y' and IS_IMMEDIATE='Y' ";

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
                lv_Actions1.DataSource = reader;
                lv_Actions1.DataBind();
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("BindImAction", ex);
            }
        }

        private void BindReAction()
        {
            string _sql = @"select  ROW_NUMBER() OVER(ORDER BY created_on) AS  Imme_No  ,action_id, DESCRIPTION, to_char(due_date,'MM/DD/YYYY') as due_date, p.name owner
                                       from siims_action act left join persons.person p on p.key=act.owner_sid  where issue_id = :IID and act.is_active='Y' and IS_IMMEDIATE='N' ";

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
                lv_Actions2.DataSource = reader;
                lv_Actions2.DataBind();
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("BindImAction", ex);
            }
        }

        private string bindControls(out string is_dist)
        {
            string rir_status = "D";
            is_dist = "N";
            string _sql = @"  select report.rir_status, issue.TITLE,report.statement,org.NAME as Org,issue.DEPT_NAME,to_char(report.date_discovered, 'MM/DD/YYYY') as date_disc,
                                        report.LOCATION,cond.CONDITION,  p.name as poc_name , report.POC_SID,  p1.name as Initiator
                                         ,  to_char(issue.created_on, 'MM/DD/YYYY') as date_init
                                        ,issue.sig_level, (select listagg(code.CATEGORY || '-' || code.CODE, '; ') WITHIN GROUP(ORDER BY NULL)
                                       from SIIMS_RIR_REPORTCODE rcode join SIIMS_RIR_CODE code on code.RIRCODE_ID=rcode.RIRCODE_ID where rcode.is_active = 'Y' 
                                       and rcode.issue_id = issue.issue_id) as Code, app.date_closed,
                                        NVL(report.is_dist_saved,'N') as is_dist
                                        from siims_issue issue 
                                        join siims_rir_report report on issue.issue_id=report.issue_id
                                        left join(select issue_id, to_char(max(date_respond),'MM/DD/YYYY') as date_closed from siims_rir_report_approve  where is_active = 'Y' and IS_ACCEPTED = 'Y'
                            group by issue_id) app on app.issue_id = issue.issue_id
                                        left join SIIMS_ORG org on org.org_id=issue.ORG_ID
                                        left join SIIMS_RIR_CONDITION cond on cond.CONDITION_ID=report.CONDITION_ID
                                        left join persons.person p on p.key= report.POC_SID
                                         left join persons.person p1 on p1.key= issue.created_by
                                        where issue.issue_id=:IID and issue.is_rir='Y' ";
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
                    rir_status = reader.GetString(0);
                    //if (rir_status == "D")
                    //{
                    //    Response.Redirect("../permission.aspx", true);
                    //}
                    //else
                  
                    lblTitle.Text = reader.GetString(1);
                    lblStatement.Text = reader.IsDBNull(2) ? "&nbsp;" : reader.GetString(2).Replace("\r", "<br />");
                    lblOrg.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    lblDept.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    lblDateDisc.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    lblLocation.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    lblCondition.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);

                    lblPOC.Text = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    lblInitiator.Text= reader.GetString(10);
                    lblDateCreated.Text = reader.GetString(11);

                    if (rir_status == "R" || rir_status == "A" || rir_status == "C")
                    {
                        PnlLevel.Visible = true;
                        lblLevel.Text = reader.IsDBNull(12) ? "" : reader.GetString(12);
                        lblCode.Text = reader.IsDBNull(13) ? "" : reader.GetString(13);
                        btnApprove.Focus();
                    }

                    if ( rir_status == "C")
                    {
                        PnlIssueDate.Visible = true;
                        lblIssueDate.Text = reader.IsDBNull(14) ? "" : reader.GetString(14);
                    }

                    is_dist = reader.GetString(15);

                }

                reader.Close();
                con.Close();
            
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return rir_status;
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

        private int checkUserType(int issue_id, out int is_done)
        {
            is_done = 0;
            int userType = 0;
            string _sql = @"select (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y') as reviewer
                                , ( select count(*) from 
                                            siims_rir_report_approve where issue_id=:IID and is_active='Y'  and reviewer_sid=:loginSID and is_respond='Y') as done
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
                    userType = reader.GetInt32(0);
                    is_done = reader.GetInt32(1);
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

        //private void getSSOSID()
        //{
        //    var _attSid = Request.ServerVariables["SSO_SID"];
        //    Response.Write(@"SID: " + _attSid + "  | ");
        //    //Log.Debug("Get SID:" + _attSid);
        //    if (_attSid != null)
        //    {
        //        if (_attSid.IndexOf(";") != -1)
        //        {
        //            _attSid = _attSid.Substring(0, _attSid.IndexOf(";"));
        //        }
        //        else
        //        {
        //            Session["LoginSID"] = _attSid;
        //        }
        //    }
        //    else
        //    {
        //        Session["LoginSID"] = "";
        //    }

        //    var loginName = Request.ServerVariables["SSO_UPN"];
        //    Response.Write(@"Login Name: " + loginName + "  | ");
        //    if (loginName != null)
        //    {
        //        if (loginName.IndexOf(";") != -1)
        //        {
        //            loginName = loginName.Substring(0, loginName.IndexOf(";"));
        //            if (loginName.IndexOf("@") != -1)
        //            {
        //                Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
        //            }
        //            else
        //            {
        //                Session["LoginName"] = loginName;
        //            }
        //        }
        //        else
        //        {
        //            if (loginName.IndexOf("@") != -1)
        //            {
        //                Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
        //            }
        //            else
        //            {
        //                Session["LoginName"] = loginName;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        Session["LoginName"] = "None";
        //    }

        //    var loginEmail = Request.ServerVariables["SSO_EMAIL"];
        //    Response.Write(@"Email: " + loginEmail);
        //    if (loginEmail != null)
        //    {
        //        if (loginEmail.IndexOf(";") != -1)
        //        {
        //            loginEmail = loginEmail.Substring(0, loginEmail.IndexOf(";"));
        //            Session["LoginEmail"] = loginEmail;
        //        }
        //        else
        //        {
        //            Session["LoginEmail"] = loginEmail;
        //        }
        //    }
        //    else
        //    {
        //        Session["LoginEmail"] = "None";
        //    }

        //    //Session["IS_OWNER"] = "1";
        //    getIsOwner(_attSid);
        //}

        //private void getIsOwner(string oSID)
        //{
        //    //string ownerSID = Session["LoginSID"].ToString();
        //    string ownerSID = oSID;

        //    string _sql = @"select count(*) as isOwner from siims_rir_reviewer where reviewer_sid=" + ownerSID + @" and is_active='Y' and is_owner='Y'";

        //    try
        //    {
        //        OracleConnection con = new OracleConnection();
        //        con.ConnectionString = _connStr;
        //        con.Open();

        //        OracleCommand cmd = con.CreateCommand();
        //        cmd.CommandText = _sql;
        //        OracleDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Session["IS_OWNER"] = reader.GetInt32(0).ToString();
        //        }

        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Is Owner for login=" + Session["LoginName"].ToString(), ex);
        //    }

        //}


        //private void getUserSID()
        //{
        //    string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];

        //    string _loginName = HttpContext.Current.Request.ServerVariables["LOGON_USER"];
        //    string _UserWinName = _loginName.Substring(_loginName.LastIndexOf("\\") + 1);


        //    if (_isUAT == "1")
        //    {
        //        if (Request["user"] != null && Request["user"] != "")
        //        {
        //            winLogin2SID(Request["user"]);
        //        }
        //        else
        //        {
        //            winLogin2SID(_UserWinName);
        //        }

        //    }
        //    else
        //    {
        //        winLogin2SID(_UserWinName);
        //    }

        //}

        //public void winLogin2SID(string winlogin)
        //{

        //    string _sql = @"SELECT b.but_sid,INITCAP(p.fname) as fname,p.maildisp,(select count(*) from siims_rir_reviewer where reviewer_sid=b.but_sid and is_active='Y' and is_owner='Y') as is_owner
        //                    FROM but b join  persons.person p on b.but_sid = p.key and b.But_ldt = 'win'
        //                        where p.gonet = 'ACTIVE'  and b.BUT_LID = lower(:Login)";

        //    try
        //    {
        //        OracleConnection con = new OracleConnection();
        //        con.ConnectionString = _connStr;
        //        con.Open();

        //        OracleCommand cmd = con.CreateCommand();
        //        cmd.CommandText = _sql;
        //        cmd.BindByName = true;
        //        cmd.Parameters.Add(":Login", OracleDbType.Varchar2).Value = winlogin;

        //        OracleDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {

        //            Session["LoginSID"] = reader.GetInt32(0).ToString();
        //            Session["LoginName"] = reader.GetString(1);
        //            Session["LoginEmail"] = reader.GetString(2);
        //            Session["IS_OWNER"] = reader.GetInt32(3).ToString();
        //        }

        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("winLogin2SID", ex);
        //    }
        //}

        protected void action1_DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                string aid = lv_Actions1.DataKeys[e.Item.DisplayIndex].Value.ToString();
                Repeater repeater = (Repeater)e.Item.FindControl("Repeater1");
                if (repeater != null && !string.IsNullOrEmpty(aid))
                {
                    bindActionAtt(repeater, aid);
                }
            }
        }

        protected void action2_DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                string aid = lv_Actions2.DataKeys[e.Item.DisplayIndex].Value.ToString();
                Repeater repeater = (Repeater)e.Item.FindControl("Repeater2");
                if (repeater != null && !string.IsNullOrEmpty(aid))
                {
                    bindActionAtt(repeater, aid);
                }
            }
        }

        private void bindActionAtt(Repeater repeater, string _aid)
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
                    sqlCmd.CommandText = "select ACATT_ID,FILE_NAME from SIIMS_ACTION_ATT where action_id=:AID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":AID", OracleDbType.Int32).Value = int.Parse(_aid);
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            repeater.DataSource = ds;
            repeater.DataBind();

        }

        protected void action1_CommandList(Object sender, RepeaterCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                action_FileData(_attachmentId);
            }
           

        }


        private void action_FileData(int _attachmentId)
        {

            FileUtil objFile = new FileUtil();
            bool isError = objFile.downLoadAttachment(_attachmentId, 2);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string status = "";
            if(Session["RIR_STATUS"] != null )
            {
                status = Session["RIR_STATUS"].ToString();
            }
            Response.Redirect("rir.aspx?s="+ status);
        }

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                FileData(_attachmentId);
            }
            //if (e.CommandName.ToLower() == "delete")
            //{
            //    DeleteAttachment(_attachmentId);

            //}

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

            lv_File.DataSource = ds;
            lv_File.DataBind();

        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            update_send2Reviewers();
            string status = "";
            if (Session["RIR_STATUS"] != null)
            {
                status = Session["RIR_STATUS"].ToString();
            }
            Response.Redirect("rir.aspx?s=" + status);
            //Response.Redirect("rir.aspx");
        }
        protected void update_send2Reviewers()
        {
            string _level = drwLevel.SelectedValue;
            string code1 = drwCode1.SelectedValue;
            string code2 = drwCode2.SelectedValue;
            string code3 = drwCode3.SelectedValue;
            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_SEND2REVIEWERS ";

                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inLevel = _cmd.Parameters.Add("PI_LEVEL", OracleDbType.Varchar2);
                    inLevel.Direction = ParameterDirection.Input;
                    inLevel.Value = _level;

                    OracleParameter inCode1= _cmd.Parameters.Add("PI_CODE1_ID", OracleDbType.Int32);
                    inCode1.Direction = ParameterDirection.Input;
                    inCode1.Value = int.Parse(code1);

                    OracleParameter inCode2 = _cmd.Parameters.Add("PI_CODE2_ID", OracleDbType.Int32);
                    inCode2.Direction = ParameterDirection.Input;
                    inCode2.Value = int.Parse(code2);


                    OracleParameter inCode3 = _cmd.Parameters.Add("PI_CODE3_ID", OracleDbType.Int32);
                    inCode3.Direction = ParameterDirection.Input;
                    inCode3.Value = int.Parse(code3);


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("update_send2Reviewers", ex);

            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            string errMsg = "";
            if(_RIR_Status=="A")
            {
                // this is for RIR coordinator for final aproval
                int no_owners = 1;
                int is_emailed = 0;
                checkActionAndEmail(out no_owners, out is_emailed);
                if(no_owners>0)
                {
                    errMsg += "There are recommended actions that do not have owner or Due Date set yet. Please fix using the link in the Action Grid. <br />";
                }
                if(is_emailed==0)
                {
                    errMsg += "The Recipients List is required for RIR to be approved. Please add the Recipients List.";
                }
                if(string.IsNullOrEmpty(errMsg))
                {
                 
                    // do approval process now, inluding send emails to DIst Recipients
                    update_Approver_Status(1);
                    string status = "";
                    if (Session["RIR_STATUS"] != null)
                    {
                        status = Session["RIR_STATUS"].ToString();
                    }
                    Response.Redirect("rir.aspx?s=" + status);
                } else
                {
                    lblWarning.Text = errMsg;
                    lblWarning.Visible = true;
                }
               
            } else
            {
                // this is for two reviewers
                update_Reviewer_Status(1);
                Response.Redirect("report_view.aspx?iid=" + _Issue_IID);
            }
          

        }

        private void checkActionAndEmail(out int no_owners, out int is_emailed)
        {
            no_owners = 1;
            is_emailed = 0;
            string _sql = @"select   ( select count(*) from siims_action where issue_id=:IID and is_active='Y' and IS_IMMEDIATE='N' 
                                and (owner_sid is null or due_date is null) ) as no_owners
                              , (select count(*) from siims_rir_view v join siims_rir_report re on re.issue_id=v.issue_id and re.is_dist_saved='Y' where re.issue_id=:IID and v.is_active='Y' ) as is_emailed
                                            from dual";

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
                while (reader.Read())
                {
                    no_owners = reader.GetInt32(0);
                    is_emailed = reader.GetInt32(1);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkActionAndEmail", ex);
            }

        }


        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (_RIR_Status == "A")
            {
                // this is for RIR coordinator for final aproval
                update_Approver_Status(0);
            }
            else
            {  
                // this is for two reviewers
                update_Reviewer_Status(0);
            }
            string status = "";
            if (Session["RIR_STATUS"] != null)
            {
                status = Session["RIR_STATUS"].ToString();
            }
            Response.Redirect("rir.aspx?s=" + status);
        }

        private void update_Reviewer_Status(int is_approved)
        {
            string _comment = txtImage1.Text.Trim();

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_UPDATE_REVIEWER";
                

                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inComment = _cmd.Parameters.Add("PI_COMMENT", OracleDbType.Varchar2);
                    inComment.Direction = ParameterDirection.Input;
                    inComment.Value = _comment;

                    string is_acceped = "N";
                    if(is_approved == 1) is_acceped = "Y";
                    OracleParameter inIsApproved = _cmd.Parameters.Add("PI_IS_APPROVED", OracleDbType.Varchar2);
                    inIsApproved.Direction = ParameterDirection.Input;
                    inIsApproved.Value = is_acceped;


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("update_Reviewer_Status", ex);

            }
        }

        private void update_Approver_Status(int is_approved)
        {
            string _comment = txtImage1.Text.Trim();

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_UPDATE_APPROVER";


                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inComment = _cmd.Parameters.Add("PI_COMMENT", OracleDbType.Varchar2);
                    inComment.Direction = ParameterDirection.Input;
                    inComment.Value = _comment;

                    string is_acceped = "N";
                    if (is_approved == 1) is_acceped = "Y";
                    OracleParameter inIsApproved = _cmd.Parameters.Add("PI_IS_APPROVED", OracleDbType.Varchar2);
                    inIsApproved.Direction = ParameterDirection.Input;
                    inIsApproved.Value = is_acceped;

                    int _cy = DateTime.Now.Year;
                    int _cq = (DateTime.Now.Month - 1) / 3 + 1;

                    OracleParameter inSFY = _cmd.Parameters.Add("PI_SOURCE_CY", OracleDbType.Int32);
                    inSFY.Direction = ParameterDirection.Input;
                    inSFY.Value = _cy;

                    OracleParameter inSQtr = _cmd.Parameters.Add("PI_SOURCE_CQ", OracleDbType.Int32);
                    inSQtr.Direction = ParameterDirection.Input;
                    inSQtr.Value = _cq;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("update_Approver_Status", ex);

            }
        }

        public string FormatActionEdit(string action_id, string title)
        {
            string tmp = "";
            if(_RIR_Status=="A" && is_Owner)
            {
                tmp = "<a href='action1.aspx?type=2&from=v&aid=" + action_id + "'>" + title + "</a>";
            } else
            {
                tmp = title;
            }

            return tmp;
        }

        protected void sendEmail2Dist()
        {
            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_SEND2DISTLIST ";

                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("sendEmail2Dist", ex);

            }
        }
    }
}