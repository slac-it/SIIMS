using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class rir : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        bool is_Owner = false;
        protected static readonly ILog Log = LogManager.GetLogger("RIR");
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

            string status = "";

            if (Request["s"] != null)
            {
                status = Request["s"].ToString();
            } 
            if(!string.IsNullOrEmpty(status)) { 
                PanelList.Visible = true;

                if(status=="E")
                {
                    lit_Title.Text = "Submitted to RIR Coordinator";
                } else if (status == "D")
                {
                    lit_Title.Text = "RIR Draft";
                }
                else if (status == "R")
                {
                    lit_Title.Text = "Submitted for Review (RPFO, DREP)";
                }
                else if (status == "A")
                {
                    lit_Title.Text = "Submitted for Approval to RIR Coordinator";
                }
                else if (status == "C")
                {
                    lit_Title.Text = "RIR Completed";
                }
            }

            Session["RIR_STATUS"] = status;
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

        //    Session["IS_OWNER"] = "1";
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

        private int checkUserType(int issue_id, out int is_done, out int is_dist, out int is_initiator)
        {
            is_initiator = 0;
            is_done = 0;
            is_dist = 0;
            int is_reviewer = 0;
            string _sql = @" select (select count(*) from SIIMS_RIR_REVIEWER where reviewer_sid=:loginSID and IS_OWNER='N' and IS_ACTIVE='Y') as reviewer,
                            ( select count(*) from 
                            siims_rir_report_approve where issue_id=:IID and is_active='Y'  and reviewer_sid=:loginSID and is_respond='Y') as done
                           , (select count(*) from siims_rir_view v join siims_rir_report re on re.issue_id=v.issue_id and re.is_dist_saved='Y' where re.issue_id=:IID and v.is_active='Y' ) as is_emailed,
                            (select count(*) as type1 from siims_issue where created_by=:loginSID and issue_id=:IID) as initiator 
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
                    is_done = reader.GetInt32(1);
                    is_dist = reader.GetInt32(2);
                    is_initiator = reader.GetInt32(3);
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

        public string FormatAction(string _iid, string _status, string date_closed)
        {
            int is_done = 0;
            int is_dist = 0;
            int is_initiator = 0;
            int is_reviewer = checkUserType(int.Parse(_iid), out is_done, out is_dist, out  is_initiator);
            string tmp = "N/A";
            if(_status=="E")
            {
       
                 if(is_Owner)
                {
                    tmp = " <a href = 'create.aspx?from=d&iid=" + _iid + "'> Edit </a>   <br />   <a href = 'report_view.aspx?from=d&iid=" + _iid + "' > Request for Review </a>";
                } else if (is_initiator == 1 || is_reviewer == 1)
                {
                    tmp = " <a href = 'create.aspx?from=d&iid=" + _iid + "'> Edit </a> ";
                } 
            }

            if (_status == "R")
            {

                if (is_reviewer == 1 && is_done==0)
                {
                    tmp = "  <a href = 'report_view.aspx?from=d&iid=" + _iid + "' > Approve Report</a>";
                }
    
            }

            if (_status == "A" && is_Owner)
            {

                if(is_dist==0)
                {
                    tmp = "  <a href = 'report_dist.aspx?from=d&iid=" + _iid + "' > Create Notification email Recipients List</a> <br /> <a href = 'report_view.aspx?from=d&iid=" + _iid + "' > Approve Report</a>";
                } else
                {
                    tmp = "  <a href = 'report_view.aspx?from=d&iid=" + _iid + "' > Approve Report</a>";
                }
            }

            if (_status == "D" && (is_initiator == 1 || is_Owner))
            {
                if(is_Owner)
                {
                    tmp = " <a href = 'create.aspx?from=d&iid=" + _iid + "'> Edit </a> ";
                }
                if (is_initiator == 1)
                {
                    tmp = " <a href = 'create.aspx?from=d&iid=" + _iid + "'> Complete Report Draft  </a> ";
                }

            }
            if (_status == "C" )
            {
                tmp = date_closed;
            }

            return tmp;        
        }

        public string FormatNumber(string no_items)
        {
            int is_done = 0;
            int is_dist = 0;
            int is_initiator = 0;
            int _userType = checkUserType(-1, out is_done, out is_dist, out is_initiator);

            if(is_initiator == 1 || is_Owner)
            {
                return no_items;
            } else
            {
                return "0";
            }


        }

        protected void gvw1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            bool isChangeHeader = false;
            if (Request["s"] != null && Request["s"].ToString()=="C")
            {
                isChangeHeader = true;
            }
            if (e.Row.RowType == DataControlRowType.Header & isChangeHeader)
            {
                e.Row.Cells[6].Text = " Issued Date";
            }
        }


    }
}