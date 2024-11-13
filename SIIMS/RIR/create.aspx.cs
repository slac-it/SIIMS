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
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class RIR_Create : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        bool is_Owner = false;
        protected static readonly ILog Log = LogManager.GetLogger("RIR_Create");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
          
            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            if (Session["IS_OWNER"] != null)
            {
                if (Session["IS_OWNER"].ToString() == "1")
                {
                    is_Owner = true;
                }
                else 
                {
                    is_Owner = false;
                }
            }
            else
            {
                is_Owner = false;
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (_loginSID == 0)
            {
                Response.Redirect("../permission.aspx", true);
            }



            int is_Reviewer=0;
            int isInitiator = 0;
            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
                is_Reviewer = checkUserType(_Issue_IID, out isInitiator);
             
            }

            if (_Issue_IID > 0)
            {
                LinkAction1.PostBackUrl = "action1.aspx?type=1&from=e&iid=" + _Issue_IID;
                LinkAction2.PostBackUrl = "action1.aspx?type=2&from=e&iid=" + _Issue_IID;
                if (Request["from"] != null && Request["from"].ToString() == "0")
                {

                    btnSave.Visible = true;
                } else
                {
                    btnSave.Visible = false;
                }
                

            } else
            {
                btnDelete.Visible = false;
                PanelTxtStatement.Visible = true;
                PanelLblStatement.Visible = false;

            }
            txtDateDisc.Attributes.Add("readonly", "readonly");

            if (!Page.IsPostBack && _Issue_IID > 0)
            {
                if (Request["from"] != null && Request["from"].ToString() != "0")
                {
      
                    PanelActions.Visible = true;
                }

              


                string rir_status=bindControls();

                // only initiator can work on Draft report
                if(rir_status=="D" && is_Owner==false && isInitiator != 1) 
                {
                    Response.Redirect("../permission.aspx", true);
                }

                if (rir_status == "E" )
                {
                    btnDelete.Visible = false;
                    btnSubmit.Text = "Save";
                    if(is_Owner) btnDelete.Visible = true;
                 
                } 
              
                 

                // Must be initator. or appprover to edit the reporter.
                if (rir_status == "E" && (is_Reviewer == 0 && isInitiator == 0 && !is_Owner))
                {
                    Response.Redirect("report_view.aspx?iid=" + _Issue_IID);
                }

                if(rir_status != "D" && rir_status != "E")
                {
                    Response.Redirect("report_view.aspx?iid=" + _Issue_IID);
                }

                //Log.Debug("REport Status: " + rir_status);
                BindFileGrid();
                BindImAction();
                BindReAction();
             
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
            string _sql = @"select  ROW_NUMBER() OVER(ORDER BY ac.created_on) AS  Imme_No  ,action_id, ac.DESCRIPTION, to_char(ac.due_date,'MM/DD/YYYY') as due_date, 
                                      p.name  as owner
                                       from siims_action  ac  left join persons.person p on p.key=ac.owner_sid where ac.issue_id = :IID and ac.is_active='Y' and ac.IS_IMMEDIATE='N' ";

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

        private int checkUserType(int issue_id, out int isInitiator)
        {
            isInitiator = 0;
            int is_reviewer = 0;
            string _sql = @"select (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y') as reviewer
                             ,(select count(*) as type3 from siims_issue where created_by=:loginSID and issue_id=:IID) as initiator
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
                    is_reviewer = reader.GetInt32(0);
                    isInitiator = reader.GetInt32(1);
                    //Log.Debug("UserType in DB:" + userType);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkUserType", ex);
            }

            return is_reviewer;
        }

        private string bindControls()
        {
            string rir_status = "D";
            string _sql = @"   select report.rir_status, issue.TITLE,report.statement,issue.ORG_ID,issue.dept_id,to_char(report.date_discovered, 'MM/DD/YYYY') as date_disc,
                                            report.LOCATION,report.CONDITION_ID,  p.name as poc_name , report.POC_SID
                                            from siims_issue issue 
                                            join siims_rir_report report on issue.issue_id=report.issue_id
                                            left join persons.person p on p.key= report.POC_SID
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
                  

                    txtTitle.Text = reader.GetString(1);
                    if (rir_status == "E")
                    {
                        btnSave.Visible = false;
                        lnkStatement.PostBackUrl = "report_statement.aspx?iid=" + _Issue_IID;
                        PanelLblStatement.Visible = true;
                        PanelTxtStatement.Visible = false;
                        lblStatement.Text = reader.IsDBNull(2) ? "" : reader.GetString(2).Replace("\r", "<br />");
                    } else
                    {
                        PanelLblStatement.Visible = false;
                        PanelTxtStatement.Visible = true;
                        txtDesc.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    }
                

                    int _org_ID = -1;
                    if (!reader.IsDBNull(3))
                    {
                        _org_ID = reader.GetInt32(3);
                        drwOrg.SelectedValue = _org_ID.ToString();
                    }

                    int _dept_ID = -1;
                    if (!reader.IsDBNull(4))
                    {
                        _dept_ID= reader.GetInt32(4);
                        //drwGroup.SelectedValue = _dept_ID.ToString();
                    }
                    if(_org_ID!=-1)
                    {
                        bind_drwDept(_org_ID);
                        drwGroup.SelectedValue = _dept_ID.ToString();
                    } 

                
                    txtDateDisc.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    txtLocation.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);

                    if (!reader.IsDBNull(7))
                    {
                        drwCondition.SelectedValue = reader.GetInt32(7).ToString();
                    }

                  
                    string poc_name= reader.IsDBNull(8) ? "" : reader.GetString(8);
                    txtPOCName.Text = poc_name;
                    int poc_sid = reader.IsDBNull(9) ? -1 : reader.GetInt32(9);
                    //Log.Debug("POC Name:" + poc_name + "   SID:" + poc_sid);
                    txtPOC_SID.Value = poc_sid.ToString();

                    if (poc_sid !=-1)
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
                Log.Error("bindControls", ex);
            }

            return rir_status;
        }

        protected void bind_drwDept(int org_ID)
        {
            string _sql = @"select '-1' as dept_id, '-- Please Select --' as display_name, 'AAA' as dept_name  from dual union select dept_id,display_name, dept_name  from vw_siims_rir_dept
                    where  org_id=:Org_ID ";
            if(_Issue_IID > 0)
            {
                _sql += @" union select to_char(dept.dept_id), to_char(dept.dept_id) || ' - ' || dept.dept as display_name, dept.dept as dept_name from SIIMS_DEPT dept
                        join siims_issue iss on iss.dept_id=dept.dept_id and iss.issue_id=:IID  where  dept.org_id=:Org_ID and length(iss.dept_id)<6
                        union select org.org_code as dept_id, org.org_code || ' - ' || org.description as display_name,  org.description as dept_name
                        from  SID.Organizations org  join siims_issue iss on org.org_code=to_char(iss.dept_id) and iss.issue_id=:IID  and iss.org_id=:Org_ID and length(iss.dept_id)>6 ";
            }

            _sql += "   order by 3";


            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":Org_ID", OracleDbType.Varchar2).Value = org_ID;
                if (_Issue_IID > 0)
                {
                    cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;
                }

                OracleDataReader reader = cmd.ExecuteReader();

                drwGroup.Items.Clear();
                drwGroup.DataSource = reader;
                drwGroup.DataTextField = "display_name";
                drwGroup.DataValueField = "dept_id";

                drwGroup.DataBind();

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bind_drwDept", ex);
            }
        }

        protected void drwOrg_Changed(object sender, EventArgs e)
        {
            //string current_value = drwGroup.SelectedValue;
            string org_ID = drwOrg.SelectedValue;
            bind_drwDept(int.Parse(org_ID));
            
            //drwGroup.SelectedValue = current_value;
        }
        //protected void PreventErrorOnbindingDropDown(object sender, EventArgs e)
        //{
        //    DropDownList theDropDownList = (DropDownList)sender;
        //    theDropDownList.DataBinding -= new EventHandler(PreventErrorOnbindingDropDown);

        //    string current_value = theDropDownList.SelectedValue;

        //    try
        //    {
        //        theDropDownList.DataBind();
        //        theDropDownList.SelectedValue = current_value;
        //    }
        //    catch (ArgumentOutOfRangeException)
        //    {
        //        theDropDownList.SelectedValue = "-1";
        //    }
        //}


        //private Control GetControlThatCausedPostBack(Page page)
        //{
        //    //initialize a control and set it to null
        //    Control ctrl = null;

        //    //get the event target name and find the control
        //    string ctrlName = page.Request.Params.Get("__EVENTTARGET");
        //    if (!String.IsNullOrEmpty(ctrlName))
        //        ctrl = page.FindControl(ctrlName);

        //    //return the control to the calling method
        //    return ctrl;
        //}


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
                                NewItem.Text = _name + " - " + _slac_id  + " - " + _dept;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_Issue_IID > 0)
            {
                updateReport(_Issue_IID, "D");
                Response.Redirect("create.aspx?from=n&iid=" + _Issue_IID);
                //goBackURL();
            }
            else
            {
                string issue_id = saveRIR2DB("D");
                Response.Redirect("create.aspx?from=n&iid=" + issue_id);

            }
          
        }

        protected void btnKeep_Click(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(2000);
            Log.Debug("Issue ID:" + _Issue_IID);
            string status = checkStatus(_Issue_IID);
            Log.Debug("Status:" + status);
            if (_Issue_IID > 0)
            {
                updateReport(_Issue_IID, checkStatus(_Issue_IID));
                //Response.Redirect("create.aspx?from=n&iid=" + _Issue_IID);
                //goBackURL();
            }
            else
            {
                string issue_id = saveRIR2DB("D");
                Response.Redirect("create.aspx?from=0&iid=" + issue_id);

            }
           
        }


        private string checkStatus(int _iid)
        {
            string rir_status = "D";
            string _sql = @"   select rir_status from siims_rir_report  where issue_id=:IID  ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _iid;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rir_status = reader.GetString(0);


                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("checkStatus", ex);
            }

            return rir_status;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            if (_Issue_IID > 0)
            {
                updateReport(_Issue_IID, "E");
             
            }
            else
            {
                string issue_id = saveRIR2DB("E");
              
            }
            string status = "";
            if (Session["RIR_STATUS"] != null)
            {
                status = Session["RIR_STATUS"].ToString();
            }
            //System.Threading.Thread.Sleep(2000);
            Response.Redirect("rir.aspx?s=" + status);
        }

        private void goBackURL()
        {
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("../todolist.aspx");
            }
            else
            {
                string status = "";
                if (Session["RIR_STATUS"] != null)
                {
                    status = Session["RIR_STATUS"].ToString();
                }
                Response.Redirect("rir.aspx?s=" + status);
            }
        }

        private string  saveRIR2DB(string rir_Status)
        {
            string _newID = "-1";
            string _title = txtTitle.Text.Trim();
            string _statement = txtDesc.Text.Trim();
            string _org_ID = drwOrg.SelectedValue;
            string _dept_ID = drwGroup.SelectedValue;
            string _poc_SID = txtPOC_SID.Value;
            string _date_Discover = Request.Form[txtDateDisc.UniqueID];
            string _location = txtLocation.Text;
            string _condition_id = drwCondition.SelectedValue;
            string session_ID = HiddenField_ATTSESSIONID.Value;

            //Log.Debug("Title:" + _title);
            //Log.Debug("_statement:" + _statement);
            //Log.Debug("_org_ID:" + _org_ID);
            //Log.Debug("_dept_ID:" + _dept_ID);
            //Log.Debug("_poc_SID:" + _poc_SID);
            //Log.Debug("_date_Discover:" + _date_Discover);
            //Log.Debug("_location:" + _location);
            //Log.Debug("_condition_id:" + _condition_id);
            //Log.Debug("session_ID:" + session_ID);

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_RIR_CREATION";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;
                    OracleParameter inIsDraft = _cmd.Parameters.Add("PI_RIR_STATUS", OracleDbType.Varchar2);
                    inIsDraft.Direction = ParameterDirection.Input;
                    inIsDraft.Value = rir_Status;

                    OracleParameter inTitle = _cmd.Parameters.Add("PI_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    //inTitle.Value = Server.HtmlEncode(_title);
                    inTitle.Value = _title;

                    OracleParameter inLocation = _cmd.Parameters.Add("PI_LOCATION", OracleDbType.Varchar2);
                    inLocation.Direction = ParameterDirection.Input;
                    inLocation.Value = _location;

                    OracleParameter inDesc = _cmd.Parameters.Add("PI_STATEMENT", OracleDbType.Clob);
                    inDesc.Direction = ParameterDirection.Input;
                    inDesc.Value = _statement;


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

                    OracleParameter inDeptID = _cmd.Parameters.Add("PI_DEPT_ID", OracleDbType.Int32);
                    inDeptID.Direction = ParameterDirection.Input;
                    if (_dept_ID == "-1" ||string.IsNullOrEmpty(_dept_ID))
                    {
                        inDeptID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDeptID.Value = int.Parse(_dept_ID);
                    }

                    OracleParameter inPOC_SID = _cmd.Parameters.Add("PI_POC_SID", OracleDbType.Int32);
                    inPOC_SID.Direction = ParameterDirection.Input;
                    if (_poc_SID == "-1")
                    {
                        inPOC_SID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inPOC_SID.Value = int.Parse(_poc_SID);
                    }

                    OracleParameter inDate_Disc = _cmd.Parameters.Add("PI_DATE_DISC", OracleDbType.Date);
                    inPOC_SID.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_date_Discover))
                    {
                        inDate_Disc.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDate_Disc.Value = Convert.ToDateTime(_date_Discover);
                    }


                    OracleParameter inCondition_ID = _cmd.Parameters.Add("PI_CONDITION_ID", OracleDbType.Int32);
                    inCondition_ID.Direction = ParameterDirection.Input;
                    if (_condition_id == "-1")
                    {
                        inCondition_ID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inCondition_ID.Value = int.Parse(_condition_id);
                    }



                    OracleParameter inSessionID = _cmd.Parameters.Add("PI_ATT_SESSIONID", OracleDbType.Varchar2);
                    inSessionID.Direction = ParameterDirection.Input;
                    inSessionID.Value = session_ID;

                    OracleParameter outIssue_ID = _cmd.Parameters.Add("PO_ISSUE_ID", OracleDbType.Int32);
                    outIssue_ID.Direction = ParameterDirection.Output;

                 

                    _cmd.ExecuteNonQuery();

                    _newID = outIssue_ID.Value.ToString();
                    Log.Debug("Issue ID: " + _newID);

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("saveRIR2DB", ex);

            }

            return _newID;
        }


        private void updateReport(int _Issue_IID, string report_Status)
        {
            string _title = txtTitle.Text.Trim();
            string _statement = txtDesc.Text.Trim();
            string _org_ID = drwOrg.SelectedValue;
            string _dept_ID = drwGroup.SelectedValue;
            string _poc_SID = txtPOC_SID.Value;
            string _date_Discover = Request.Form[txtDateDisc.UniqueID];
            string _location = txtLocation.Text;
            string _condition_id = drwCondition.SelectedValue;
            Log.Debug("Date Discovered:" + _date_Discover);

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    if (report_Status == "E" && string.IsNullOrEmpty(_statement)) 
                    {
                        // this is the update without changing statement 
                        _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_RIR_UPDATE2";
                    } else
                    {
                        _cmd.CommandText = "APPS_ADMIN.SIIMS_RIR_PKG.PROC_RIR_UPDATE";
                        OracleParameter inDesc = _cmd.Parameters.Add("PI_STATEMENT", OracleDbType.Varchar2);
                        inDesc.Direction = ParameterDirection.Input;
                        inDesc.Value = _statement;
                    }

                    OracleParameter inIssue_ID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssue_ID.Direction = ParameterDirection.Input;
                    inIssue_ID.Value = _Issue_IID;

                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;
                    OracleParameter inIsDraft = _cmd.Parameters.Add("PI_RIR_STATUS", OracleDbType.Varchar2);
                    inIsDraft.Direction = ParameterDirection.Input;
                    inIsDraft.Value = report_Status;

                    OracleParameter inTitle = _cmd.Parameters.Add("PI_TITLE", OracleDbType.Varchar2);
                    inTitle.Direction = ParameterDirection.Input;
                    inTitle.Value = _title;


                    OracleParameter inLocation = _cmd.Parameters.Add("PI_LOCATION", OracleDbType.Varchar2);
                    inLocation.Direction = ParameterDirection.Input;
                    inLocation.Value = _location;

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

                    OracleParameter inDeptID = _cmd.Parameters.Add("PI_DEPT_ID", OracleDbType.Int32);
                    inDeptID.Direction = ParameterDirection.Input;
                    if (_dept_ID == "-1")
                    {
                        inDeptID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDeptID.Value = int.Parse(_dept_ID);
                    }

                    OracleParameter inPOC_SID = _cmd.Parameters.Add("PI_POC_SID", OracleDbType.Int32);
                    inPOC_SID.Direction = ParameterDirection.Input;
                    if (_poc_SID == "-1")
                    {
                        inPOC_SID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inPOC_SID.Value = int.Parse(_poc_SID);
                    }

                    OracleParameter inDate_Disc = _cmd.Parameters.Add("PI_DATE_DISC", OracleDbType.Date);
                    inPOC_SID.Direction = ParameterDirection.Input;
                    if (string.IsNullOrEmpty(_date_Discover))
                    {
                        inDate_Disc.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inDate_Disc.Value = Convert.ToDateTime(_date_Discover);
                    }


                    OracleParameter inCondition_ID = _cmd.Parameters.Add("PI_CONDITION_ID", OracleDbType.Int32);
                    inCondition_ID.Direction = ParameterDirection.Input;
                    if (_condition_id == "-1")
                    {
                        inCondition_ID.Value = System.DBNull.Value;
                    }
                    else
                    {
                        inCondition_ID.Value = int.Parse(_condition_id);
                    }

                  


                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateReport", ex);

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            goBackURL();

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
            goBackURL();
        }

    

        protected string ToFinancialYear()
        {
            DateTime currentDate = DateTime.Now;
            return (currentDate.Month >= 10 ? currentDate.Year + 1 : currentDate.Year).ToString();
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

        protected void action1_DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                string aid= lv_Actions1.DataKeys[e.Item.DisplayIndex].Value.ToString();
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

        protected void action1_CommandList(Object sender, RepeaterCommandEventArgs  e)
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