﻿using System;
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
    public partial class rep_searchIssue : System.Web.UI.Page
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

        protected void BindGridExcel(string keyword)
        {
            GridView gvw_issue = new GridView();
            string _sql = @" select issue.issue_id, issue.title, issue.description, org.name as org, p.name as owner, issue.SIG_LEVEL, sta.status,
                            to_char( iss.created_on,'MM/DD/YYYY') as Date_Created,to_char( iss.last_on,'MM/DD/YYYY') as Date_LastModified 
                              , src.type as source_type,  s.title as source_title, s.FY as Source_FY, s.QUARTER as Source_Quarter
                             ,wtype.worktype as work_type 
                             ,cat1.category as Functional_Area_1 ,cat1.sub as Sub_Functional_Area_1,cat2.category as Functional_Area_2,cat2.sub as Sub_Functional_Area_2 
                             ,cat3.category as Functional_Area_3,cat3.sub as Sub_Functional_Area_3  
                            from siims_issue iss  JOIN  VW_SIIMS_ISSUE_VIEW issue on issue.issue_id=iss.issue_id
                                left join siims_org org on issue.org_id=org.org_id
                                join siims_status sta on issue.status_id=sta.status_id
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
                                and code.IS_WORKTYPE='N' and code.order_id=3 where code.IS_ACTIVE='Y') cat3 on cat3.issue_id=issue.issue_id 
                            where  iss.is_active='Y' and issue.status_id in (11,12)  ";

            if (keyword != "")
            {
                _sql += " and (lower(issue.title) like lower(:keyword) OR LOWER(ISSUE.DESCRIPTION) LIKE lower(:keyword) )";
            }
            _sql += " order by 1";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                if (keyword != "")
                {
                    cmd.Parameters.Add(":keyword", OracleDbType.Varchar2).Value = "%" + keyword + "%";
                }
                string daystring = DateTime.Now.ToString("MM-dd-yyyy");

                string excelFileName;
                excelFileName = "issue_report"  + "_" + daystring + ".xls";



                OracleDataReader _reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

              

                gvw_issue.DataSource = _reader;
                gvw_issue.DataBind();

                ExportToExcel(excelFileName, gvw_issue);

                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("gridBinding", ex);
            }
            finally
            {
                gvw_issue = null;
                gvw_issue.Dispose();

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

        protected void BtnGo_Click(object sender, EventArgs e)
        {
            hdnkeyword.Value = TxtKeyword.Text.Trim();
        }
    }
}