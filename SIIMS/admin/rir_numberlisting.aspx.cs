using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class rir_numberlisting : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int intType = 0;
        protected static readonly ILog Log = LogManager.GetLogger("rir_numberlisting");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (Session["ISRIRADMIN"] == null || Session["ISRIRADMIN"].ToString() != "Y")
            {
                Response.Redirect("../rir/rir.aspx", true);
            }

           
            if (Request["type"] != null)
            {
                intType = int.Parse(Request["type"].ToString());
            }



            if (!Page.IsPostBack)
            {
                if (Page.PreviousPage == null)
                {
                    Response.Redirect("../permission.aspx");
                }
                ContentPlaceHolder pageContent = (ContentPlaceHolder)
                 (Page.PreviousPage.Form.FindControl("ContentPlaceHolder1"));

                if (pageContent == null)
                {
                    Response.Redirect("../permission.aspx");
                }
                else
                {
                    TextBox txtsDate = (TextBox)(pageContent.FindControl("txtStartDate"));
                    TextBox txteDate = (TextBox)(pageContent.FindControl("txtEndDate"));
                    string startDate = txtsDate.Text;
                    string endDate = txteDate.Text;
                    ViewState["SDATE"] = startDate;
                    ViewState["EDATE"] = endDate;
                    if (intType == 1)
                    {
                        LitTitle.Text = "Actions Opened between " + startDate + " and " + endDate;
                    } 
                    if (intType==2) { 
                            LitTitle.Text = "Actions Closed between " + startDate + " and " + endDate;
                    }
                    bindGrid(intType, startDate, endDate);
                }



            }
        }

        private void bindGrid(int intType, string sDate, string eDate)
        {
            string _sql;

            if (intType == 1)
            {
                _sql = @"select p.name as owner,act.action_id, 'A' || act.action_id  as action_aid
                            ,act.title as atitle, to_char(act.DUE_DATE,'MM/DD/YYYY') as dueDate, act.DUE_DATE, issue.issue_id,issue.title as ititle, issue.sig_level,
                            act.status_id,  decode(act.status_id,20,'Owner Rejected',sta.status) as status
                        from  (select distinct action_id from   (select jn.action_id,jn.status_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                          and (issue.is_rir='N' or issue.is_rir is null)
                        where jn.status_id in (21,22)  and jn.is_active='Y' 
                        group by jn.action_id,jn.status_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                          union
                         select jn.action_id,jn.status_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id  and issue.is_active='Y'
                          and issue.is_rir='Y' 
                        where jn.status_id in (21,22)  and jn.is_active='Y' 
                        group by jn.action_id,jn.status_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                        ) tmp1 ) tmp
                        join siims_action act on act.action_id=tmp.action_id and act.is_active='Y'
                        left join persons.person p on act.owner_sid=p.key
                         join siims_issue issue on issue.issue_id=act.issue_id 
                         join siims_status sta on act.status_id=sta.status_id
                         order by act.action_id desc";
            } else
            {
                _sql = @"select p.name as owner,act.action_id, 'A' || act.action_id  as action_aid
                            ,act.title as atitle, to_char(act.DUE_DATE,'MM/DD/YYYY') as dueDate, act.DUE_DATE, issue.issue_id,issue.title as ititle, issue.sig_level,
                            act.status_id,  decode(act.status_id,20,'Owner Rejected',sta.status) as status
                        from   (select jn.action_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                         and (issue.is_rir='N' or issue.is_rir is null)
                        where jn.status_id=23  and jn.is_active='Y' 
                        group by jn.action_id
                         having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                           union
                         select jn.action_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id  and issue.is_active='Y'
                          and issue.is_rir='Y' 
                        where jn.status_id =23  and jn.is_active='Y' 
                        group by jn.action_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                        ) tmp
                        join siims_action act on act.action_id=tmp.action_id and act.is_active='Y'
                        join persons.person p on act.owner_sid=p.key
                         join siims_issue issue on issue.issue_id=act.issue_id 
                         join siims_status sta on act.status_id=sta.status_id
                         order by act.action_id desc";
            }
            //Log.Debug(_sql);
            OracleDataReader _reader = null;
            try
            {
                using (OracleConnection _conn = new OracleConnection(_connStr))
                {
                    using (OracleCommand _cmd = new OracleCommand(_sql, _conn))
                    {
                        _cmd.BindByName = true;
                        _cmd.Parameters.Add("SID", OracleDbType.Int32).Value = _loginSID;
                        //_cmd.Parameters.Add("StatusID", OracleDbType.Int32).Value = 21+ intType;
                        _cmd.Parameters.Add(":SDate", OracleDbType.Date).Value = Convert.ToDateTime(sDate);
                        _cmd.Parameters.Add(":EDate", OracleDbType.Date).Value = Convert.ToDateTime(eDate);
                        _conn.Open();
                        _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        GV_Actions.DataSource = _reader;
                        GV_Actions.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {

                Log.Error("bindGrid", ex);
            }
            finally
            {
                if (_reader != null) { _reader.Close(); _reader.Dispose(); }
            }
        }

        protected void ImgBtnExport_Click(object sender, ImageClickEventArgs e)
        {
            GV_Actions.Columns.Clear();
            GV_Actions.AutoGenerateColumns = true;
            //gvw1.BorderStyle = BorderStyle.Solid;
            GV_Actions.GridLines = GridLines.Both;
            string daystring = DateTime.Now.ToString("MM-dd-yyyy");
            string exportFile;
            if (intType==1)
            {
                exportFile = "admin_opened_actions_reports_" + daystring + ".xls";
            } else
            {
                exportFile = "admin_closed_actions_reports_" + daystring + ".xls";
            }
           
            ExportToExcel(exportFile);

        }

        private void ExportToExcel(string fileName)
        {

            const string m_Http_Attachment = "attachment;filename=";
            const string m_Http_Content = "content-disposition";


            Response.ClearContent();

            Response.AddHeader(m_Http_Content, m_Http_Attachment + fileName);
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter m_StringWriter = new StringWriter();
            HtmlTextWriter m_HtmlWriter = new HtmlTextWriter(m_StringWriter);

            GV_Actions.AllowPaging = false;
            GV_Actions.AllowSorting = false;


            try
            {
                string _sql;
                if (intType==1)
                {
                    _sql = @"select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner,to_char( action.due_date,'MM/DD/YYYY') as due_date,
                          decode(action.status_id,20,'Owner Rejected',sta.status) as status,
                       to_char( action.created_on,'MM/DD/YYYY') as Date_Created,to_char( action.last_on,'MM/DD/YYYY') as Date_LastModified,  
                       issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description,org.name as org , 
                       ip.name as Issue_Owner, issue.SIG_LEVEL,s.title as source_title, cat.acode as Action_Category,sub.ACODE as Action_Subcategory 
                       FROM  (select distinct action_id from 
                         ( select jn.action_id,  jn.status_id,min(jn_datetime) as edate from APPS_ADMIN.siims_action_jn jn
                            join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                          and (issue.is_rir='N' or issue.is_rir is null)
                            where jn.status_id in (21,22)  and jn.is_active='Y' 
                            group by jn.action_id, jn.status_id
                            having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate) 
                    union
                         select jn.action_id, jn.status_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id  and issue.is_active='Y'
                          and issue.is_rir='Y' 
                        where jn.status_id in (21,22)  and jn.is_active='Y' 
                        group by jn.action_id, jn.status_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                            ) tmp1 ) tmp                          
                    join    siims_action action on tmp.action_id=action.action_id and action.is_active='Y'
                           join vw_siims_issue_view issue on action.issue_id=issue.issue_id
                        join siims_status sta on action.status_id=sta.status_id
                        left join persons.person p on p.key=action.owner_sid
                        join siims_org org on issue.org_id=org.org_id 
                         left join persons.person ip on ip.key=issue.owner_sid 
                        left join siims_source s on issue.issue_id=s.issue_id
                        left join siims_atrending_code cat on cat.ACODE_ID=action.cat_acode_id and cat.IS_CATEGORY='Y' 
                          left join siims_atrending_code sub on sub.ACODE_ID=action.sub_acode_id and sub.IS_CATEGORY='N' 
                        order by tmp.action_id ";
                } else
                {
                     _sql = @"select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner,to_char( action.due_date,'MM/DD/YYYY') as due_date,
                          decode(action.status_id,20,'Owner Rejected',sta.status) as status,
                       to_char( action.created_on,'MM/DD/YYYY') as Date_Created,to_char( action.last_on,'MM/DD/YYYY') as Date_LastModified,  
                       issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description,org.name as org , 
                       ip.name as Issue_Owner, issue.SIG_LEVEL,s.title as source_title, cat.acode as Action_Category,sub.ACODE as Action_Subcategory 
                       FROM 
                         ( select jn.action_id, min(jn_datetime) as edate from APPS_ADMIN.siims_action_jn jn
                            join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                           and (issue.is_rir='N' or issue.is_rir is null)
                            where jn.status_id =23  and jn.is_active='Y' 
                            group by jn.action_id
                             having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                        union
                         select jn.action_id, min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_issue issue on issue.issue_id=jn.issue_id  and issue.is_active='Y'
                          and issue.is_rir='Y' 
                        where jn.status_id =23  and jn.is_active='Y' 
                        group by jn.action_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                            ) tmp                               
                    join    siims_action action on tmp.action_id=action.action_id and action.is_active='Y'
                           join vw_siims_issue_view issue on action.issue_id=issue.issue_id
                        join siims_status sta on action.status_id=sta.status_id
                        join persons.person p on p.key=action.owner_sid
                        join siims_org org on issue.org_id=org.org_id 
                         left join persons.person ip on ip.key=issue.owner_sid 
                        left join siims_source s on issue.issue_id=s.issue_id
                        left join siims_atrending_code cat on cat.ACODE_ID=action.cat_acode_id and cat.IS_CATEGORY='Y' 
                          left join siims_atrending_code sub on sub.ACODE_ID=action.sub_acode_id and sub.IS_CATEGORY='N' 
                        order by tmp.action_id ";
                }
               

                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add("SID", OracleDbType.Int32).Value = _loginSID;
                //cmd.Parameters.Add("StatusID", OracleDbType.Int32).Value = 21 + intType;
                cmd.Parameters.Add(":SDate", OracleDbType.Date).Value = Convert.ToDateTime(ViewState["SDATE"].ToString());
                cmd.Parameters.Add(":EDate", OracleDbType.Date).Value = Convert.ToDateTime(ViewState["EDATE"].ToString());

                OracleDataReader reader = cmd.ExecuteReader();
                GV_Actions.DataSource = reader;
                GV_Actions.DataBind();
                GV_Actions.RenderControl(m_HtmlWriter);

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("ExportToExcel", ex);
            }


            string m_gridViewText = m_StringWriter.ToString();


            Response.Write(m_gridViewText);
            Response.End();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
    }
}