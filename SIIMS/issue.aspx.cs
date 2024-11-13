using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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

namespace SIIMS
{
    public partial class issue : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID=-1;
        protected static readonly ILog Log = LogManager.GetLogger("issue");

        protected void Page_Load(object sender, EventArgs e)
        {
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

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (_loginSID == 0)
            {
                Response.Redirect("permission.aspx", true);
            }


            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
                // only the original creator can edit draft
                if (!checkAuthorized(_Issue_IID))
                {
                    Response.Redirect("permission.aspx");
                }
            }

            if (!Page.IsPostBack && _Issue_IID>0)
            {
                btnDelete.Visible = true;
                bindControls();
                BindFileGrid();
            }
            else
            {
                btnDelete.Visible = false;
            }
          

            if (Page.IsPostBack)
            {
                string ctrlName = Page.Request.Params.Get("__EVENTTARGET");
                Control ctrl = null;
                if (!String.IsNullOrEmpty(ctrlName))
                {
                    ctrl = Page.FindControl(ctrlName);
                }

                if (ctrl != null && ctrl.ID == "drwSType")
                {
                    setSourceSelection((DropDownList) ctrl);
                   
                }

              
                  
            }
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


        private bool checkAuthorized(int issue_id)
        {
            bool isAuthorized = false;
            string _sql = "select created_by from siims_issue where created_by=:loginSID and issue_id=:IID ";

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
                if (reader.HasRows)
                {
                    isAuthorized = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkAuthorized", ex);
            }

            return isAuthorized;
        }

        private void bindControls()
        {
            string _sql = "   select issue.TITLE,issue.DESCRIPTION,issue.ORG_ID,issue.OWNER_SID,issue.SIG_LEVEL,issue.STYPE_ID, s.TITLE as STitle, s.FY, s.QUARTER,  p.name as owner_name " +
                "from siims_issue issue left join siims_source s on issue.issue_id = s.issue_id left join persons.person p on p.key=owner_sid  where issue.issue_id=:IID ";
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
                    txtTitle.Text = reader.GetString(0);
                    txtDesc.Text =reader.IsDBNull(1)? "": reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        drwOrg.SelectedValue = reader.GetInt32(2).ToString();
                    }
                    
                    string ownerSID=reader.IsDBNull(3)? "" : reader.GetInt32(3).ToString();
                    txtOwner.Text = ownerSID;

                    if (!reader.IsDBNull(4))
                    {
                        drwLevel.SelectedValue = reader.GetString(4);
                    }

                    string _sType = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    if (_sType.Length > 0)
                    {

                        drwSType.SelectedValue = reader.GetString(5);
                   
                         PanelSourceTitle.Visible = true;
                         txtSourceTitle.Text = reader.GetString(6);
                         HiddenField_FY.Value = reader.GetString(7);
                         HiddenField_Qtr.Value = reader.GetString(8);
                         //txtSourceFY.Text = reader.GetString(7);
                         //txtSourceQtr.Text = reader.GetString(8);
                     }
                     else
                     {
                         PanelSourceTitle.Visible = false;
                     }

                     if (string.IsNullOrEmpty(ownerSID))
                     {
                         PnlPopup.Visible = true;
                     }
                     else
                     {
                         PnlPopup.Visible = false;
                         txtOwner_Name.Visible = true;
                         btnChangeOwner.Visible = true;
                         txtOwner.Text = "";
                         txtOwner_Name.Text = reader.GetString(9);
                         txtOwner_SID.Value = ownerSID;
                     }

                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }
        }

        protected void PreventErrorOnbindingDropDown(object sender, EventArgs e)
        {
            DropDownList theDropDownList = (DropDownList)sender;
            theDropDownList.DataBinding -= new EventHandler(PreventErrorOnbindingDropDown);

            string current_value = theDropDownList.SelectedValue;

            try
            {
                theDropDownList.DataBind();
                theDropDownList.SelectedValue = current_value;
            }
            catch (ArgumentOutOfRangeException)
            {
                theDropDownList.SelectedValue = "-1";
            }
        }

        private void setSourceSelection(DropDownList drwControl)
        {
            string sourceID = drwControl.SelectedValue;
            if (sourceID == "A")
            {
                PanelAssessment.Visible = true;
                PanelIncident.Visible = false;
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelSourceTitle.Visible = false;
                btnSave.Visible = false;
                btnSubmit.Visible = false;
            }

            if (sourceID == "I")
            {
                PanelIncident.Visible = true;
                PanelAssessment.Visible = false;
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelSourceTitle.Visible = false;
                btnSave.Visible = false;
                btnSubmit.Visible = false;
            }
            if (sourceID == "O")
            {
                PanelAssessment.Visible = false;
                PanelSourceTitle.Visible = false;
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = true;
                PanelIncident.Visible = false;
                btnSave.Visible = false;
                btnSubmit.Visible = false;
                //bindOtherHistory();
               
            }
            if (sourceID == "U")
            {
                PanelAssessment.Visible = false;
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelIncident.Visible = false;
                PanelSourceTitle.Visible = true;
                txtSourceTitle.Text = "Unknown";
                string fy="-1";
                string qtr="-1";
                fy = DateTime.Now.Year.ToString();
                if (DateTime.Now.Month == 10 || DateTime.Now.Month == 11 || DateTime.Now.Month == 12)
                    qtr = "1";
                if (DateTime.Now.Month == 1 || DateTime.Now.Month == 2 || DateTime.Now.Month == 3)
                    qtr = "2";
                if (DateTime.Now.Month == 4 || DateTime.Now.Month == 5 || DateTime.Now.Month == 6)
                    qtr = "3";
                if (DateTime.Now.Month == 7 || DateTime.Now.Month == 8 || DateTime.Now.Month == 9)
                    qtr = "4";

                HiddenField_FY.Value = fy;
                HiddenField_Qtr.Value = qtr;
                btnSave.Visible = true;
                btnSubmit.Visible = true;

            }

            if (sourceID == "-1")
            {
                PanelAssessment.Visible = false;
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelIncident.Visible = false;
                PanelSourceTitle.Visible = false;
            }
        }

        private void bindOtherHistory()
        {
            string _sql = " select min(s.source_id) as source_id, s.title  from siims_issue iss join siims_source s on iss.issue_id=s.issue_id where iss.stype_id='O' group by s.title order by title";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
               
                OracleDataReader _reader = cmd.ExecuteReader();
               
                if (_reader.HasRows)
                {
                    drw_OtherSelection.DataSource = _reader;
                    drw_OtherSelection.DataBind();
                    drw_OtherSelection.Items.Insert(0,new ListItem("-- Select Title from History --", "-1", true));

                  
                    PanelOtherSelection.Visible = true;
                }
                else
                {
                    PanelOtherSelection.Visible = false;
                }



                _reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindOtherHistory", ex);
            }
        }

        private Control GetControlThatCausedPostBack(Page page)
        {
            //initialize a control and set it to null
            Control ctrl = null;

            //get the event target name and find the control
            string ctrlName = page.Request.Params.Get("__EVENTTARGET");
            if (!String.IsNullOrEmpty(ctrlName))
                ctrl = page.FindControl(ctrlName);

            //return the control to the calling method
            return ctrl;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_Issue_IID > 0)
            {
                updateIssue(_Issue_IID,'Y');
            }
            else
            {
                save2DB("Y");
            }
           
                
           Response.Redirect("default.aspx");
     
        }

        private void updateIssue(int _Issue_IID, char is_Draft)
        {
            string _title = txtTitle.Text.Trim();
            string _description = txtDesc.Text.Trim();
            string _org_ID = drwOrg.SelectedValue;
            string _level = drwLevel.SelectedValue;
            string _SType_id = drwSType.SelectedValue;
            string _owner_sid = txtOwner_SID.Value;
            string _sourceTitle = txtSourceTitle.Text.Trim();
            //string _sourceFY = txtSourceFY.Text;
            //string _sourceQtr = txtSourceQtr.Text;
            string _sourceFY = HiddenField_FY.Value;
            string _sourceQtr = HiddenField_Qtr.Value;

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ISSUE_PKG.PROC_ISSUE_DRAFT_UPDATE";
                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

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

                    OracleParameter inOrgID = _cmd.Parameters.Add("PI_ORG_ID", OracleDbType.Int32);
                    inOrgID.Direction = ParameterDirection.Input;
                    if (_org_ID == "-1")
                    {
                        inOrgID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inOrgID.Value = int.Parse(_org_ID);
                    }

                    OracleParameter inLevel = _cmd.Parameters.Add("PI_LEVEL", OracleDbType.Varchar2);
                    inLevel.Direction = ParameterDirection.Input;
                    if (_level == "-1")
                    {
                        inLevel.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inLevel.Value = _level;
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

                    OracleParameter inSType = _cmd.Parameters.Add("PI_STYPE_ID", OracleDbType.Varchar2);
                    inSType.Direction = ParameterDirection.Input;
                    if (_SType_id == "-1")
                    {
                        inSType.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inSType.Value = _SType_id;
                    }

                    OracleParameter inSTitle = _cmd.Parameters.Add("PI_SOURCE_TITLE", OracleDbType.Varchar2);
                    inSTitle.Direction = ParameterDirection.Input;
                    inSTitle.Value = _sourceTitle;

                    OracleParameter inSFY = _cmd.Parameters.Add("PI_SOURCE_FY", OracleDbType.Varchar2);
                    inSFY.Direction = ParameterDirection.Input;
                    inSFY.Value = _sourceFY;

                    OracleParameter inSQtr = _cmd.Parameters.Add("PI_SOURCE_QUARTER", OracleDbType.Varchar2);
                    inSQtr.Direction = ParameterDirection.Input;
                    inSQtr.Value = _sourceQtr;

                  

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateIssue", ex);

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (_Issue_IID > 0)
            {
                updateIssue(_Issue_IID, 'N');
            }
            else
            {
                save2DB("N");
            }


            Response.Redirect("default.aspx");
           
          
        }     

       

        private void save2DB(string is_Draft)
        {
            string _title = txtTitle.Text.Trim();
            string _description= txtDesc.Text.Trim();
            string _org_ID = drwOrg.SelectedValue;
            string _level = drwLevel.SelectedValue;
            string _SType_id = drwSType.SelectedValue;
            string _owner_sid = txtOwner_SID.Value;
            string _sourceTitle = txtSourceTitle.Text.Trim();
            //string _sourceFY = txtSourceFY.Text;
            //string _sourceQtr = txtSourceQtr.Text;
            string _sourceFY = HiddenField_FY.Value;
            string _sourceQtr = HiddenField_Qtr.Value;
            string _link_ID = HiddenField_ALINK.Value;
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
                    _cmd.CommandText = "SIIMS_ISSUE_PKG.PROC_ISSUE_CREATION";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;
                     OracleParameter inIsDraft = _cmd.Parameters.Add("PI_IS_DRAFT", OracleDbType.Varchar2);
                    inIsDraft.Direction = ParameterDirection.Input;
                    inIsDraft.Value = is_Draft;

                     OracleParameter inTitle = _cmd.Parameters.Add("PI_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    //inTitle.Value = Server.HtmlEncode(_title);
                    inTitle.Value =_title;
                      OracleParameter inDesc = _cmd.Parameters.Add("PI_DESC", OracleDbType.Varchar2);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _description;

                    OracleParameter inOrgID = _cmd.Parameters.Add("PI_ORG_ID", OracleDbType.Int32);
                    inOrgID.Direction = ParameterDirection.Input;
                     if (_org_ID == "-1")
                    {
                         inOrgID.Value  = System.DBNull.Value;
                    }
                    else
                    {
                        inOrgID.Value  = int.Parse(_org_ID);
                    }

                     OracleParameter inLevel= _cmd.Parameters.Add("PI_LEVEL", OracleDbType.Varchar2);
                    inLevel.Direction = ParameterDirection.Input;
                     if (_level == "-1")
                    {
                         inLevel.Value  = System.DBNull.Value;
                    }
                    else
                    {
                        inLevel.Value  =_level;
                    }
                  
                    OracleParameter inOwnerSID = _cmd.Parameters.Add("PI_OWNER_SID", OracleDbType.Int32);
                    inOwnerSID.Direction = ParameterDirection.Input;
                     if (_owner_sid == "-1" )
                    {
                         inOwnerSID.Value  = System.DBNull.Value;
                    }
                    else
                    {
                        inOwnerSID.Value  = int.Parse(_owner_sid);
                    }

                      OracleParameter inSType= _cmd.Parameters.Add("PI_STYPE_ID", OracleDbType.Varchar2);
                    inSType.Direction = ParameterDirection.Input;
                     if (_SType_id == "-1")
                    {
                         inSType.Value  = System.DBNull.Value;
                    }
                    else
                    {
                        inSType.Value  =_SType_id;
                    }
 
                      OracleParameter inSTitle = _cmd.Parameters.Add("PI_SOURCE_TITLE", OracleDbType.Varchar2);
                    inSTitle.Direction = ParameterDirection.Input;
                    inSTitle.Value = _sourceTitle;

                    OracleParameter inSFY = _cmd.Parameters.Add("PI_SOURCE_FY", OracleDbType.Varchar2);
                    inSFY.Direction = ParameterDirection.Input;
                    inSFY.Value = _sourceFY;

                    OracleParameter inSQtr = _cmd.Parameters.Add("PI_SOURCE_QUARTER", OracleDbType.Varchar2);
                    inSQtr.Direction = ParameterDirection.Input;
                    inSQtr.Value = _sourceQtr;

                    OracleParameter inLinkID = _cmd.Parameters.Add("PI_SOURCE_LINKID", OracleDbType.Varchar2);
                    inLinkID.Direction = ParameterDirection.Input;
                    inLinkID.Value = _link_ID;

                    OracleParameter inSessionID = _cmd.Parameters.Add("PI_ATT_SESSIONID", OracleDbType.Varchar2);
                    inSessionID.Direction = ParameterDirection.Input;
                    inSessionID.Value = session_ID;


                    OracleParameter outIssue_ID = _cmd.Parameters.Add("PO_ISSUE_ID", OracleDbType.Int32);
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
                        NewItem.Text = _name + " - " + _slac_id + " - " + _dept;
                        //NewItem.Text = _name ;
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
            btnChangeOwner.Visible = false;
            //txtOwner_SID.Value = "";
            //txtOwner_Name.Text = "";
        }

        protected void btnOtherSelection_Click(object sender, EventArgs e)
        {
            PanelOtherInput.Visible = false;
            PanelOtherSelection.Visible = false;
            PanelIncident.Visible = false;
            PanelAssessment.Visible = false;
            PanelSourceType.Visible = false;
            PanelSourceTitle.Visible = true;

            btnSubmit.Visible = true;
            btnSave.Visible = true;

            string other_ID = drw_OtherSelection.SelectedValue;
            if (other_ID != "-1")
            {
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelIncident.Visible = false;
                PanelSourceTitle.Visible = true;

                // TODO: read incident table and populate


                string _sql = "select title,fy,quarter from siims_source where source_id= :sourceID ";

                try
                {
                    OracleConnection con = new OracleConnection();
                    con.ConnectionString = _connStr;
                    con.Open();

                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = _sql;
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":sourceID", OracleDbType.Varchar2).Value = other_ID;

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtSourceTitle.Text = reader.GetString(0);
                        HiddenField_FY.Value = reader.GetString(1);
                        HiddenField_Qtr.Value = reader.GetString(2);

                    }

                    reader.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    Log.Error("btnIncidentSelection_Click", ex);
                }
            }


        }

        protected void btnOtherInput_Click(object sender, EventArgs e)
        {
            PanelOtherInput.Visible = false;
            PanelOtherSelection.Visible = false;
            PanelIncident.Visible = false;
            PanelAssessment.Visible = false;
            PanelSourceType.Visible = false;
            PanelSourceTitle.Visible = true;

            txtSourceTitle.Text = txtSourceOtherTitle.Text;
            HiddenField_FY.Value = drw_OtherFY.SelectedValue;
            HiddenField_Qtr.Value = drw_OtherQtr.SelectedValue;
            btnSubmit.Visible = true;
            btnSave.Visible = true;


        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string _sql = "Update SIIMS_ISSUE set IS_ACTIVE='N', LAST_ON=sysdate, LAST_BY=:LoginSID where issue_id=:IID";
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
                    _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                  
                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("btnDelete_Click", ex);

            }
            Response.Redirect("default.aspx");
        }

        protected void btnTitleChange_Click(object sender, EventArgs e)
        {
            PanelOtherInput.Visible = false;
            PanelOtherSelection.Visible = false;
            PanelIncident.Visible = false;
            PanelAssessment.Visible = false;
            PanelSourceType.Visible = true;
            PanelSourceTitle.Visible = false;
            drwSType.SelectedValue = "-1";

        }

        protected  string ToFinancialYear()
        {
            DateTime currentDate = DateTime.Now;
            return (currentDate.Month >= 10 ? currentDate.Year + 1 : currentDate.Year).ToString();
        }


        protected void btnCreateOther_Click(object sender, EventArgs e)
        {
            PanelOtherInput.Visible = true;
            PanelOtherSelection.Visible = false;
            PanelIncident.Visible = false;
            PanelAssessment.Visible = false;
            PanelSourceType.Visible = false;
            PanelSourceTitle.Visible = false;

            DateTime currentDate = DateTime.Now;

            int currFY= (currentDate.Month >= 10 ? currentDate.Year + 1 : currentDate.Year) ;
            int preFY = currFY - 1;

            drw_OtherFY.Items.Add(preFY.ToString());
            drw_OtherFY.Items.Add(currFY.ToString());

        }

       

        protected void btnIncidentSelection_Click(object sender, EventArgs e)
        {

            btnSubmit.Visible = true;
            btnSave.Visible = true;

            string inc_ID = drwIncidentTitle.SelectedValue;
            if (inc_ID != "-1")
            {
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelIncident.Visible = false;
                PanelAssessment.Visible = false;
                PanelSourceType.Visible = false;
                PanelSourceTitle.Visible = true;

                // TODO: read incident table and populate


                string _sql = "select title,fy,quarter from siims_incident where inc_id= :incID ";

                try
                {
                    OracleConnection con = new OracleConnection();
                    con.ConnectionString = _connStr;
                    con.Open();

                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = _sql;
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":incID", OracleDbType.Varchar2).Value = inc_ID;

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtSourceTitle.Text = reader.GetString(0);
                        //txtSourceFY.Text = reader.GetString(1);
                        //txtSourceQtr.Text = reader.GetString(2);
                        HiddenField_FY.Value = reader.GetString(1);
                        HiddenField_Qtr.Value = reader.GetString(2);

                    }

                    reader.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    Log.Error("btnIncidentSelection_Click", ex);
                }

            }
        }


        protected void btnAssessmentSelection_Click(object sender, EventArgs e)
        {

            btnSubmit.Visible = true;
            btnSave.Visible = true;

            string  assessment_ID = drwAssessmentTitle.SelectedValue;
            if (assessment_ID != "-1")
            {
                PanelOtherInput.Visible = false;
                PanelOtherSelection.Visible = false;
                PanelIncident.Visible = false;
                PanelAssessment.Visible = false;
                PanelSourceType.Visible = false;
                PanelSourceTitle.Visible = true;

                // TODO: read incident table and populate


                string _sql = @"SELECT TITLE,  REPLACE(FY_DUE, 'FY', '20') as FY, QTR_DUE FROM IAS_ASSESSMENT WHERE IS_ACTIVE='Y' and ASSESSMENT_ID=:Ass_ID 
                              UNION SELECT N.TITLE, REPLACE(N.FY_DUE, 'FY', '20') as FY, N.QTR_DUE FROM IASR_ASSESSMENT N
                                WHERE N.IS_ACTIVE = 'Y' and N.ASSESSMENT_ID = :ASS_ID";

                try
                {
                    OracleConnection con = new OracleConnection();
                    con.ConnectionString = _connStr;
                    con.Open();

                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = _sql;
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":Ass_ID", OracleDbType.Varchar2).Value = assessment_ID;

                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtSourceTitle.Text = reader.GetString(0);
                        HiddenField_FY.Value = reader.GetString(1);
                        HiddenField_Qtr.Value = reader.GetInt32(2).ToString();
                        //txtSourceFY.Text = reader.GetString(1);
                        //txtSourceQtr.Text = reader.GetInt32(2).ToString();
                       
                    }

                    HiddenField_ALINK.Value = assessment_ID;
                    reader.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    Log.Error("btnAssessmentSelection_Click", ex);
                }

            }
        }

     

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ImageButton lnkDelete = (ImageButton)e.Item.FindControl("ImgBtnDelete");
                if (lnkDelete != null)
                {

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
                string _sql = "UPDATE SIIMS_ISSUE_ATT SET IS_ACTIVE='N', LAST_ON=SysDate, LAST_BY=:loginSID WHERE isatt_id=:ISATT_ID";

                try
                {
                    OracleCommand _cmd = new OracleCommand(_sql, _conn);
                    _cmd.BindByName = true;
                    _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                    _cmd.Parameters.Add(":ISATT_ID", OracleDbType.Int32).Value = _attachmentId;
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
            bool isError = objFile.downLoadAttachment(_attachmentId, 1);
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
                            _cmd.CommandText = " Insert Into SIIMS_ISSUE_ATT ( ISSUE_ID, FILE_NAME,FILE_SIZE ,CONTENT_TYPE,FILE_DATA,CREATED_BY,TEMP_ID)  Values(:ISSID,:FILE_NAME,:FILE_SIZE,:CONTENT_TYPE,:FILE_DATA,:loginSID,:TempID)";
                            _cmd.BindByName = true;
                            if (_Issue_IID == -1)
                            {
                                _cmd.Parameters.Add(":ISSID", OracleDbType.Int32).Value = System.DBNull.Value;
                            }
                            else
                            {
                                _cmd.Parameters.Add(":ISSID", OracleDbType.Int32).Value = _Issue_IID;
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

            if (_Issue_IID > 0)
            {

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
                        sqlCmd.CommandText = "select ISATT_ID,FILE_NAME, CREATED_ON from SIIMS_ISSUE_ATT where temp_id=:TempID and IS_ACTIVE='Y' ";
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