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
    public partial class rep_expIssueCode : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_expIssueCode");
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

            string _sql = " select issue.issue_id, issue.title, issue.description, org.name as org,p.name as owner, issue.SIG_LEVEL, sta.status, to_char( iss.created_on,'MM/DD/YYYY') as Date_Created,to_char( iss.last_on,'MM/DD/YYYY') as Date_LastModified " +
                " , src.type as source_type,  s.title as source_title, s.FY as Source_FY, s.QUARTER as Source_Quarter, wtype.worktype as work_type " +
                " ,cat1.category as Functional_Area_1 ,cat1.sub as Sub_Functional_Area_1,cat2.category as Functional_Area_2,cat2.sub as Sub_Functional_Area_2 " +
                "  ,cat3.category as Functional_Area_3,cat3.sub as Sub_Functional_Area_3  " +
                " from siims_issue iss  JOIN  VW_SIIMS_ISSUE_VIEW issue on issue.issue_id=iss.issue_id join siims_org org on issue.org_id=org.org_id  " +
                " join siims_status sta on issue.status_id=sta.status_id  " +
                "  left join persons.person p on p.key=issue.owner_sid  " +
                " left join siims_source_type src on src.stype_id=issue.stype_id " +
                "  join siims_source s on issue.issue_id=s.issue_id  and " + filter +
                " join (select code.issue_id, tr.WORKTYPE from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID and code.IS_WORKTYPE='Y' " +
                " where code.IS_ACTIVE='Y') wtype on wtype.issue_id=issue.issue_id " +
                " join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID " +
                " and code.IS_WORKTYPE='N' and code.order_id=1 where code.IS_ACTIVE='Y') cat1 on cat1.issue_id=issue.issue_id  " +
                " left join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID " +
                " and code.IS_WORKTYPE='N' and code.order_id=2 where code.IS_ACTIVE='Y') cat2 on cat2.issue_id=issue.issue_id " +
                " left join (select code.issue_id, tr.category,tr.sub from siims_issue_code code join SIIMS_ITRENDING_CODE tr on tr.ICODE_ID=code.ICODE_ID " +
                " and code.IS_WORKTYPE='N' and code.order_id=3 where code.IS_ACTIVE='Y') cat3 on cat3.issue_id=issue.issue_id " +
                " WHERE iss.is_active='Y' and exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='Y') " +
                " AND exists (select * from siims_issue_code where issue_id=issue.issue_id and is_worktype='N')  ";

            if(!string.IsNullOrEmpty(iCode_ID)) {
                _sql += "   and issue.issue_id in (select issue_id from siims_issue_code where icode_id=:ICODE_ID ) "; 
            }

             _sql +=    " order by 1 ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName=true;

                if(!string.IsNullOrEmpty(iCode_ID)) {
                    cmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = int.Parse(iCode_ID);
                }
          

                string daystring = DateTime.Now.ToString("MM-dd-yyyy");
                string excelFileName = "issue_trending_report" + "_" + daystring + ".xls";

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
                ////Response.End();
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
                //gvw_issue_codes.Dispose();
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