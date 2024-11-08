using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_deptAction_export : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_deptAction_export");
        string _connStr;
        string _sid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            if (Request.QueryString["sid"] != null)
            {
                _sid = Request.QueryString["sid"].ToString();
            }
            else
            {
                Response.Redirect("permission.aspx");
            }


            gridBinding(_sid);
        }

        private void gridBinding(string _sid)
        {
            string _sql = @" select 'A' || act.action_id as action_id, act.title as Action_Title, act.description as Action_Description, sup.name as Manager,to_char( act.due_date,'MM/DD/YYYY') as due_date, sta.status, 
                           to_char( act.created_on,'MM/DD/YYYY') as Date_Created,to_char( act.last_on,'MM/DD/YYYY') as Date_LastModified,  
                           issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description,org.name as org ,  ip.name as Issue_Owner, issue.SIG_LEVEL,s.title as source_title, cat.acode as Action_Category,sub.ACODE as Action_Subcategory 
                               from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path 
                            FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP' and level > 1  
                             START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn 
                             join persons.person sup on sup.key=mn.mgr_SID 
                             join siims_action act on act.owner_sid=mn.key and act.IS_ACTIVE='Y' and act.status_id<> 20 
                             join VW_SIIMS_ISSUE_VIEW issue on issue.issue_id=act.issue_id 
                             join siims_status sta on act.status_id=sta.status_id
                               join siims_org org on issue.org_id=org.org_id 
                              left join siims_source s on issue.issue_id=s.issue_id  
                               left join persons.person ip on ip.key=issue.owner_sid 
                             left join siims_atrending_code cat on cat.ACODE_ID=act.cat_acode_id and cat.IS_CATEGORY='Y' 
                             left join siims_atrending_code sub on sub.ACODE_ID=act.sub_acode_id and sub.IS_CATEGORY='N' 
                             order by act.action_id desc";


            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                cmd.Parameters.Add(":SID", OracleDbType.Varchar2).Value = _sid;

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");

                string excelFileName;
                excelFileName = "action_report_mgr_" + _sid + "_" + daystring + ".xls";



                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                gvw_dept_actions.DataSource = _reader;
                gvw_dept_actions.DataBind();

                ExportToExcel(excelFileName, gvw_dept_actions);

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_dept_actions = null;
                gvw_dept_actions.Dispose();

            }
        }

        private void ExportToExcel(string fileName, GridView dg)
        {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            dg.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        // To surpress the error, you should have this method added.
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
    }
}