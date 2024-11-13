using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using SIIMS.App_Code;
using System.IO;
using log4net;

namespace SIIMS
{
    public partial class issue_view : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = 0;
        protected static readonly ILog Log = LogManager.GetLogger("issue_view");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                //getUserSID();
                //getSSOSID();
                // login SSO info
                string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];
                getVar.AttSid = Request.ServerVariables["SSO_SID"];
                getVar.AttLoginName = Request.ServerVariables["SSO_FIRSTNAME"];
                getVar.AttEmail = Request.ServerVariables["SSO_EMAIL"];
                getVar.Visible = (_isUAT == "1" ? true : false);

                getVar.getSSOSID1();

                // end login SSO info

            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (_loginSID == 0)
            {
                Response.Redirect("../permission.aspx", true);
            }


            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("~/default.aspx", true);
            }

          

            if (!Page.IsPostBack && _Issue_IID > 0)
            {
                bindControls();
                BindFileGrid();
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

        private void bindControls()
        {
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
                    if (!reader.IsDBNull(11)) {
                         status = reader.GetString(11);

                         if (!reader.IsDBNull(13) && status=="Open")
                         {
                             status += ": " + reader.GetString(12) ;
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
            bool isError = objFile.downLoadAttachment(_attachmentId, 1);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request["from"] != null && Request["from"].ToString() == "todo")
            {
                Response.Redirect("todolist.aspx");
            }
            else if (Request["from"] != null && Request["from"].ToString() == "team")
            {
                Response.Redirect("team_issue.aspx");
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
            //GridView gvTemp = (GridView)sender;

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    string actionId = (gvTemp).DataKeys[e.Row.RowIndex].Value.ToString();

            //    Label lblStatusID = e.Row.FindControl("lblStatusID") as Label;

            //}

            //Check if this is our Blank Row being databound, if so make the row invisible
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (((DataRowView)e.Row.DataItem)["Action_ID"].ToString() == String.Empty) e.Row.Visible = false;
            //}
        }
    }

}