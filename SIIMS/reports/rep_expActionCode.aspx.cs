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

namespace SIIMS
{
    public partial class rep_expActionCode : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_expActionCode");
        string _connStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string iCode_ID = "";

            if (!String.IsNullOrEmpty(Request.QueryString["icodeid"]))
            {
                iCode_ID = Request.QueryString["icodeid"].ToString();

            }
           

            gridBinding(iCode_ID);
        }

        private void gridBinding(string iCode_ID)
        {
            string filter = Session["FILTER1"].ToString();

            string _sql = " select 'A' || action.action_id as action_id, action.title as Action_Title, action.description as Action_Description, p.name as action_owner,to_char( action.due_date,'MM/DD/YYYY') as due_date, sta.status, " +
                        " to_char( action.created_on,'MM/DD/YYYY') as Date_Created,to_char( action.last_on,'MM/DD/YYYY') as Date_LastModified,  " +
                       " issue.issue_id, issue.title as Issue_Title, issue.description as Issue_Description, org.name as org , ip.name as Issue_Owner,issue.SIG_LEVEL, s.title as source_title, cat.acode as Action_Category, sub.acode as Action_Subcategory " +
                       " FROM siims_action action JOIN  siims_issue issue on issue.issue_id=action.issue_id join siims_org org on issue.org_id=org.org_id " +
                       " join siims_status sta on action.status_id=sta.status_id left join persons.person p on p.key=action.owner_sid  " +
                       " left join persons.person ip on ip.key=issue.owner_sid " +
                       " join siims_source s on issue.issue_id=s.issue_id  and " + filter +
                       " join siims_atrending_code cat on cat.ACODE_ID=action.cat_acode_id and cat.IS_CATEGORY='Y' " +
                       " join siims_atrending_code sub on sub.ACODE_ID=action.sub_acode_id and sub.IS_CATEGORY='N' WHERE action.is_active='Y' and action.CAT_ACODE_ID is not null " +
                       " and  action.SUB_ACODE_ID is not null ";
            
            
            if(!string.IsNullOrEmpty(iCode_ID)) {
                _sql += " and action.issue_id in (select issue_id from siims_issue_code where icode_id=:ICODE_ID ) "; 
            }

             _sql +=    " order by 1 ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                if (!string.IsNullOrEmpty(iCode_ID))
                {
                    cmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = int.Parse(iCode_ID);
                }

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");
                string excelFileName = "action_trending_report" + "_" + daystring + ".xls";

                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                gvw_action_codes.DataSource = _reader;
                gvw_action_codes.DataBind();

                ExportToExcel(excelFileName, gvw_action_codes);

                //Response.Clear();
                //Response.Buffer = true;
                ////Response.ContentType = "application/vnd.ms-excel";
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;filename=" + excelFileName);
                //Response.Charset = "";
                //this.EnableViewState = false;
                //System.IO.StringWriter tw = new System.IO.StringWriter();
                //System.Web.UI.Html32TextWriter hw = new Html32TextWriter(tw);


                //gvw_action_codes.RenderControl(hw);

                //StringBuilder sbResponseString = new StringBuilder();
                //sbResponseString.Append(tw + "</body></html>");
                //Response.AddHeader("Content-Length", sbResponseString.Length.ToString());
                //Response.Write(sbResponseString.ToString());
                ////Response.End();
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_action_codes = null;
                gvw_action_codes.Dispose();

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