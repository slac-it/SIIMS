using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Oracle.ManagedDataAccess.Client;

namespace SIIMS
{
    public partial class rep_orgAction_export : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_orgAction_export");
        string _connStr;
        string org_id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            if (Request.QueryString["org_id"] != null)
            {
                org_id = Request.QueryString["org_id"].ToString();
            }
            else
            {
                Response.Redirect("permission.aspx");
            }


            gridBinding(org_id);
        }

        private void gridBinding(string org_id)
        {
            string _sql = @" select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner,to_char( action.due_date,'MM/DD/YYYY') as due_date, sta.status, 
                       to_char( action.created_on,'MM/DD/YYYY') as Date_Created,to_char( action.last_on,'MM/DD/YYYY') as Date_LastModified,  
                       issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description,org.name as org ,  ip.name as Issue_Owner, issue.SIG_LEVEL,s.title as source_title, cat.acode as Action_Category,sub.ACODE as Action_Subcategory 
                       FROM siims_action action JOIN  VW_SIIMS_ISSUE_VIEW issue on issue.issue_id=action.issue_id and issue.org_id=:ORGID
                         join siims_org org on issue.org_id=org.org_id 
                       left join siims_status sta on action.status_id=sta.status_id left join persons.person p on p.key=action.owner_sid  
                       left join persons.person ip on ip.key=issue.owner_sid 
                       left join siims_source s on issue.issue_id=s.issue_id  left join siims_atrending_code cat on cat.ACODE_ID=action.cat_acode_id and cat.IS_CATEGORY='Y' 
                       left join siims_atrending_code sub on sub.ACODE_ID=action.sub_acode_id and sub.IS_CATEGORY='N' WHERE action.is_active='Y'
                        and action.status_id <> 20 ";


            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                cmd.Parameters.Add(":ORGID", OracleDbType.Varchar2).Value = org_id;

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");

                string excelFileName;
                excelFileName = "action_report_org_" + org_id + "_" + daystring + ".xls";



                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                gvw_org_action.DataSource = _reader;
                gvw_org_action.DataBind();

                ExportToExcel(excelFileName, gvw_org_action);

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_org_action = null;
                gvw_org_action.Dispose();

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