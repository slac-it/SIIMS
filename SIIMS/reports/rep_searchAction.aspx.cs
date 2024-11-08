using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using log4net;

namespace SIIMS.reports
{
    public partial class rep_searchAction : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_deptIssue_export");
        string _connStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
        }

        protected void LnkExcel_Click(object sender, EventArgs e)
        {
            BindGridExcel(hdnkeyword.Value);
        }

        protected void BtnGo_Click(object sender, EventArgs e)
        {
            hdnkeyword.Value = TxtKeyword.Text.Trim();
        }


        private void BindGridExcel(string keyword)
        {
            GridView gvw_action = new GridView();

            string _sql = @" select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner,to_char( action.due_date,'MM/DD/YYYY') as due_date, sta.status,
                    to_char( action.created_on,'MM/DD/YYYY') as Date_Created,to_char( action.last_on,'MM/DD/YYYY') as Date_LastModified,  
                  issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description, org.name as org , 
                  ip.name as Issue_Owner,issue.SIG_LEVEL, s.title as source_title, cat.acode as Action_Category, sub.acode as Action_Subcategory 
                    FROM siims_action action JOIN  siims_issue issue on issue.issue_id=action.issue_id  join siims_org org on issue.org_id=org.org_id 
                    join siims_status sta on action.status_id=sta.status_id left join persons.person p on p.key=action.owner_sid   left join persons.person ip on ip.key=issue.owner_sid 
                      join siims_source s on issue.issue_id=s.issue_id left join siims_atrending_code cat on cat.ACODE_ID=action.cat_acode_id 
                     LEFT join siims_atrending_code sub on sub.ACODE_ID=action.sub_acode_id  WHERE action.is_active='Y' and action.status_id <> 20 
                     ";


            if (!string.IsNullOrEmpty(keyword))
            {
                _sql += " and (lower(action.title) like lower(:keyword) or lower(action.description) like lower(:keyword) ) ";
            }

            _sql += " order by 1 ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                if (!string.IsNullOrEmpty(keyword))
                {
                    cmd.Parameters.Add(":keyword", OracleDbType.Varchar2).Value = "%" + keyword + "%";
                }

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");
                string excelFileName = "action_report" + "_" + daystring + ".xls";

                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                gvw_action.DataSource = _reader;
                gvw_action.DataBind();

                ExportToExcel(excelFileName, gvw_action);

                

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_action = null;
                gvw_action.Dispose();

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