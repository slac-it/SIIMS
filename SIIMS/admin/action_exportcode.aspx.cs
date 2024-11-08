using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class action_exportcode : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("issue_exportcode");
        string _connStr;
        bool isFullReport=false;

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            // allow everyone to download, see JIRA ticket DEV-8973
            //if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            //{
            //    Response.Redirect("../permission.aspx");
            //}

             if (Request.QueryString["isFull"] != null) 
            {
                string isfull = Request.QueryString["isFull"].ToString();
                if (isfull == "1")
                {
                    isFullReport = true;
                }
            }

            gridBinding();
        }

        private void gridBinding()
        {
            string _sql = @" select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner
                             , to_char( action.due_date, 'MM/DD/YYYY') as due_date, sta.status, 
                             to_char(action.created_on, 'MM/DD/YYYY') as Date_Created,to_char(action.last_on, 'MM/DD/YYYY') as Date_LastModified,  
                             issue.issue_id, issue.title as Issue_Title,  NVL(issue.description,' ') as Issue_Description,org.name as org ,  ip.name as Issue_Owner, issue.SIG_LEVEL,s.title as source_title
                             , cat.acode as Action_Category,sub.ACODE as Action_Subcategory
                             FROM siims_action action left JOIN VW_SIIMS_ISSUE_VIEW issue on issue.issue_id = action.issue_id left join siims_org org on issue.org_id = org.org_id
                            left join siims_status sta on action.status_id = sta.status_id left join persons.person p on p.key = action.owner_sid
                             left join persons.person ip on ip.key = issue.owner_sid
                             left join siims_source s on issue.issue_id = s.issue_id  left join siims_atrending_code cat on cat.ACODE_ID = action.cat_acode_id and cat.IS_CATEGORY = 'Y'
                             left join siims_atrending_code sub on sub.ACODE_ID = action.sub_acode_id and sub.IS_CATEGORY = 'N'
                             WHERE action.is_active = 'Y' ";
            if (isFullReport) {
                _sql += " and (action.status_id in (21,22,23) or (action.status_id=20 and (issue.is_rir='N' or issue.is_RIR is null)))   ";
            } else {
                _sql += " and action.CAT_ACODE_ID is not null and  action.SUB_ACODE_ID is not null  ";
            }
            _sql += " order by 1 ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");
                string excelFileName;
                if (isFullReport)
                {
                    excelFileName = "action" + "_full_" + daystring + ".xls";
                }
                else
                {
                    excelFileName = "action" + "_code_" + daystring + ".xls";
                }
               

                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                GridView dg = new GridView();
                dg.DataSource = _reader;
                dg.DataBind();

         
                ExportToExcel2( excelFileName,dg);

                cmd.Dispose();
                dg = null;
                dg.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
              

            }
        }

        private void ExportToExcel2(string fileName, GridView dg)
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

        private void ExportToExcel(string fileName,DataGrid dg)
        {
            Response.Clear();
            Response.Buffer = true;
            //Response.ContentType = "application/vnd.ms-excel";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Charset = "";


            this.EnableViewState = false;
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.Html32TextWriter hw = new Html32TextWriter(tw);


            dg.RenderControl(hw);


            StringBuilder sbResponseString = new StringBuilder();
            sbResponseString.Append(tw + "</body></html>");
            Response.AddHeader("Content-Length", sbResponseString.Length.ToString());
            Response.Write(sbResponseString.ToString());
            Response.End();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        // To surpress the error, you should have this method added.
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }
    }
}