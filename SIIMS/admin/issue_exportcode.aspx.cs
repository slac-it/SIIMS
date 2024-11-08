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
    public partial class issue_exportcode : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("issue_exportcode");
        string _connStr;
        bool isFullReport = false;
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
            string _sql = @" select issue.issue_id, issue.title, issue.description, org.name as org, p.name as owner, issue.SIG_LEVEL, sta.status,
                        to_char( iss.created_on,'MM/DD/YYYY') as Date_Created,to_char( iss.last_on,'MM/DD/YYYY') as Date_LastModified 
                         , src.type as source_type,  s.title as source_title, s.FY as Source_FY, s.QUARTER as Source_Quarter,wtype.worktype as work_type
                         ,cat1.category as Functional_Area_1 ,cat1.sub as Sub_Functional_Area_1,cat2.category as Functional_Area_2,cat2.sub as Sub_Functional_Area_2 
                         ,cat3.category as Functional_Area_3,cat3.sub as Sub_Functional_Area_3  
                        FROM VW_SIIMS_ISSUE_VIEW issue join siims_issue iss on iss.issue_id=issue.issue_id and iss.is_active='Y' 
                        left join siims_org org on issue.org_id=org.org_id  
                         left join siims_status sta on issue.status_id=sta.status_id  
                         left join persons.person p on p.key=issue.owner_sid  
                         left join siims_source_type src on src.stype_id=issue.stype_id 
                         left join siims_source s on issue.issue_id=s.issue_id  
                        left join (select code.issue_id, tr.WORKTYPE from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID and code.IS_WORKTYPE='Y' 
                         where code.IS_ACTIVE='Y') wtype on wtype.issue_id=issue.issue_id 
                         left join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID 
                         and code.IS_WORKTYPE='N' and code.order_id=1 where code.IS_ACTIVE='Y') cat1 on cat1.issue_id=issue.issue_id  
                         left join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID 
                         and code.IS_WORKTYPE='N' and code.order_id=2 where code.IS_ACTIVE='Y') cat2 on cat2.issue_id=issue.issue_id 
                         left join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID 
                        and code.IS_WORKTYPE='N' and code.order_id=3 where code.IS_ACTIVE='Y') cat3 on cat3.issue_id=issue.issue_id  ";
            if(isFullReport)  {
                _sql += " where (issue.status_id in (11,12) or (issue.status_id=10 and (issue.is_rir='N' or issue.is_RIR is null)))  order by 1 ";
            } else {
                _sql += " where exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='Y') " +
               " AND exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='N')  " +
               " order by 1 ";
            }
            
           
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
                    excelFileName = "issue_full_report" + "_" + daystring + ".xls";
                }
                else
                {
                    excelFileName = "issue_trending_code" + "_" + daystring + ".xls";
                }
               

                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                gvw_issue_codes.DataSource = _reader;
                gvw_issue_codes.DataBind();

                ExportToExcel(excelFileName, gvw_issue_codes);

                //Response.Clear();
                //Response.Buffer = true;
                ////Response.ContentType = "application/vnd.ms-excel";
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;filename=" + excelFileName);
                //Response.Charset = "";
                //this.EnableViewState = false;
                //System.IO.StringWriter tw = new System.IO.StringWriter();
                //System.Web.UI.Html32TextWriter hw = new Html32TextWriter(tw);


                //gvw_issue_codes.RenderControl(hw);

                //StringBuilder sbResponseString = new StringBuilder();
                //sbResponseString.Append(tw + "</body></html>");
                //Response.AddHeader("Content-Length", sbResponseString.Length.ToString());
                //Response.Write(sbResponseString.ToString());
                //Response.End();
                //System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_issue_codes = null;
                gvw_issue_codes.Dispose();

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