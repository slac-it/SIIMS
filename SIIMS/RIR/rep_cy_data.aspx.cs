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

namespace SIIMS.RIR
{
    public partial class rep_cy_data : System.Web.UI.Page
    {
        string _connStr;
        string _sqlStatement;
        string _sqlToExcel = "";
        string selectedOrgID = "-1";
        string selectedCY = "-1";
        string selectedCQ = "-1";
        string keyword = "";
        protected static readonly ILog Log = LogManager.GetLogger("rep_cy_data");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            if(!Page.IsPostBack)
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

                getSQLStatemewnt(pageContent);
                //lblSQL.Text = _sqlToExcel;
            }  else
            {
                _sqlStatement = ViewState["RIR_SQL"].ToString();
                selectedOrgID=ViewState["RIR_SQL_ORGID"].ToString();
                selectedCY =ViewState["RIR_SQL_CY"].ToString();
                selectedCQ =ViewState["RIR_SQL_CQ"].ToString();
                keyword =ViewState["RIR_SQL_KW"].ToString();
            }
           

          
         
            bindGrid(); 
        }

        private void getSQLStatemewnt(ContentPlaceHolder cPH)
        {
            string _sql = @"select issue.issue_id, ss.title as RIR_ID,issue.title, p.name as Initiator, to_char(rep.DATE_DISCOVERED, 'MM/DD/YYYY') as date_discovered, 
                              org.NAME as Org_Name,issue.DEPT_NAME, rep.rir_status
                            , (select listagg(code.CODE, '; ') WITHIN GROUP(ORDER BY NULL)
                            from SIIMS_RIR_REPORTCODE rcode join SIIMS_RIR_CODE code on code.RIRCODE_ID = rcode.RIRCODE_ID where rcode.is_active = 'Y'
                            and rcode.issue_id = issue.issue_id) as Tracking_Code, app.date_closed as issued_date
                            from  siims_issue issue
                            join SIIMS_RIR_REPORT rep on issue.issue_id = rep.ISSUE_ID
                            join persons.person p on p.key = issue.CREATED_BY
                            join(select issue_id, to_char(max(date_respond),'MM/DD/YYYY') as date_closed from siims_rir_report_approve  where is_active = 'Y' and IS_ACCEPTED = 'Y'
                            group by issue_id) app on app.issue_id = issue.issue_id
                            join siims_org org on org.ORG_ID = issue.ORG_ID
                            join siims_source ss on ss.issue_id=issue.issue_id
                            where issue.is_rir = 'Y' and issue.is_active = 'Y' and rep.rir_status = 'C' 
                            and (:OrgID=-1 or issue.org_id=:OrgID) and (:CY=-1 or ss.CY=:CY)  and (:CQ=-1 or ss.CQ=:CQ) ";

            string _sqlExcel = @"select ss.title as RIR_ID, issue.title,rep.statement ,act.title as Action_Title, act.description as Action_Description, 
                                p.name as Initiator, to_char(issue.created_on, 'MM/DD/YYYY') as date_initiated
                             ,org.NAME as Org_Name, issue.DEPT_NAME,poc.name as POC, to_char(rep.DATE_DISCOVERED, 'MM/DD/YYYY') as date_discovered, 
                             rep.location,cond.CONDITION,issue.sig_level
                            , (select listagg(code.CODE, '; ') WITHIN GROUP(ORDER BY NULL)
                            from SIIMS_RIR_REPORTCODE rcode join SIIMS_RIR_CODE code on code.RIRCODE_ID = rcode.RIRCODE_ID where rcode.is_active = 'Y'
                            and rcode.issue_id = issue.issue_id) as Tracking_Code, app.date_closed as issued_date, to_char(act.due_date, 'MM/DD/YYYY') as Action_Due_Date,aowner.name as action_owner, issue.issue_id
                            from  siims_issue issue
                            join SIIMS_RIR_REPORT rep on issue.issue_id = rep.ISSUE_ID
                            left join siims_action act on act.issue_id=issue.issue_id and act.is_active='Y'
                            left join persons.person aowner on act.owner_sid=aowner.key
                            join persons.person poc on poc.key= rep.POC_SID
                            join persons.person p on p.key = issue.CREATED_BY
                            join SIIMS_RIR_CONDITION cond on cond.CONDITION_ID=rep.CONDITION_ID
                            join(select issue_id, to_char(max(date_respond),'MM/DD/YYYY') as date_closed from siims_rir_report_approve  where is_active = 'Y' and IS_ACCEPTED = 'Y'
                            group by issue_id) app on app.issue_id = issue.issue_id
                            join siims_org org on org.ORG_ID = issue.ORG_ID
                            join siims_source ss on ss.issue_id=issue.issue_id
                            where issue.is_rir = 'Y' and issue.is_active = 'Y' and rep.rir_status = 'C' 
                            and (:OrgID=-1 or issue.org_id=:OrgID) and (:CY=-1 or ss.CY=:CY)  and (:CQ=-1 or ss.CQ=:CQ) ";

            // get the first name data from the first page and set the label
            // on this page

            DropDownList drwOrg = (DropDownList)(cPH.FindControl("drwOrg"));
            if (drwOrg != null) selectedOrgID = drwOrg.SelectedValue;
           

            DropDownList drwCY = (DropDownList)(cPH.FindControl("drwCY"));
            if (drwCY != null) selectedCY = drwCY.SelectedValue;
           
            DropDownList drwCQ = (DropDownList)(cPH.FindControl("drwCQ"));
            if (drwCQ != null) selectedCQ = drwCQ.SelectedValue;
           

            ListBox lboxDept = (ListBox)(cPH.FindControl("drwDept"));
            bool foundAll = false;
            string sql2 = "";
            int deptNo = lboxDept.GetSelectedIndices().Count();
            foreach (int i in lboxDept.GetSelectedIndices())
            {
                string itemValue = lboxDept.Items[i].Value;
                if (itemValue == "-1")
                {
                    foundAll = true;
                }
            }
            if (!foundAll && deptNo > 0)
            {
                sql2 += " and (";
                foreach (int i in lboxDept.GetSelectedIndices())
                {
                    string itemValue = lboxDept.Items[i].Value;
                    sql2 += " issue.dept_id=" + int.Parse(itemValue) + " or";
                }
                _sql += ReplaceLastOccurrence(sql2, "or", ")");
                _sqlExcel += ReplaceLastOccurrence(sql2, "or", ")");
            }

            ListBox lboxCode = (ListBox)(cPH.FindControl("drwCode"));
            bool foundAllCode = false;
            string sql3 = "";
            int codeNo = lboxCode.GetSelectedIndices().Count();
            foreach (int i in lboxCode.GetSelectedIndices())
            {
                string itemValue = lboxCode.Items[i].Value;
                if (itemValue == "-1")
                {
                    foundAllCode = true;
                }
            }
            if (!foundAllCode && codeNo > 0)
            {
                sql3 += " and (";
                foreach (int i in lboxCode.GetSelectedIndices())
                {
                    string itemValue = lboxCode.Items[i].Value;
                    sql3 += "  exists (select issue_id from siims_rir_reportcode where rircode_id=" + int.Parse(itemValue) + "  and issue_id=issue.issue_id) or";
                }
                _sql += ReplaceLastOccurrence(sql3, "or", ")");
                _sqlExcel += ReplaceLastOccurrence(sql3, "or", ")");
            }

            TextBox txtKeyword = (TextBox)(cPH.FindControl("txtKeyword"));
            if (txtKeyword != null) keyword = txtKeyword.Text.ToLower();

            //lblOrg.Text = selectedOrgID;
            //lblYear.Text = selectedCY;
            //lblKeyword.Text = keyword;
            //lblQuarter.Text = selectedCQ;

            if (!string.IsNullOrEmpty(keyword))
            {
                _sql += " and (lower(issue.title) like '%' || :keyword || '%' or lower(rep.statement) like '%' || :keyword || '%' ) ";
                _sqlExcel += " and (lower(issue.title) like '%' || :keyword || '%' or lower(rep.statement) like '%' || :keyword || '%' ) ";
            }


            _sql += " order by issue.CREATED_ON desc";
            _sqlExcel += " order by RIR_ID ";

            _sqlStatement = _sql;
            _sqlToExcel = _sqlExcel;
            ViewState["RIR_SQL"] = _sqlExcel;
            ViewState["RIR_SQL_ORGID"] = selectedOrgID;
            ViewState["RIR_SQL_CY"] = selectedCY;
            ViewState["RIR_SQL_CQ"] = selectedCQ;
            ViewState["RIR_SQL_KW"] = keyword;
        }

        private void bindGrid()
        {
            try
            {
                OracleConnection con = new OracleConnection();
                DataTable dt = new DataTable();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sqlStatement;
                cmd.BindByName = true;
                cmd.Parameters.Add(":OrgID", OracleDbType.Int32).Value = int.Parse(selectedOrgID);
                cmd.Parameters.Add(":CY", OracleDbType.Int32).Value = int.Parse(selectedCY);
                cmd.Parameters.Add(":CQ", OracleDbType.Int32).Value = int.Parse(selectedCQ);
                if (!string.IsNullOrEmpty(keyword))
                {
                    cmd.Parameters.Add(":keyword", OracleDbType.Varchar2).Value = keyword;
                }
                using (OracleDataAdapter sqlAdp = new OracleDataAdapter(cmd))
                {
                    sqlAdp.Fill(dt);
                }
                //OracleDataReader reader = cmd.ExecuteReader();
                gvw1.DataSource = dt;
                gvw1.DataBind();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";
                //reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindGrid", ex);
            }
        }

     

        protected void gvw1_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvw1.DataSource = dtrslt;
                gvw1.DataBind();


            }

        }

        public string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        protected void ImgBtnExport_Click(object sender, ImageClickEventArgs e)
        {
            gvw1.Columns.Clear();
            gvw1.AutoGenerateColumns = true;
            //gvw1.BorderStyle = BorderStyle.Solid;
            gvw1.GridLines = GridLines.Both;
            ExportToExcel("rir_closedreports.xls");

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

            gvw1.AllowPaging = false;
            gvw1.AllowSorting = false;
           

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sqlStatement;
                cmd.BindByName = true;
                cmd.Parameters.Add(":OrgID", OracleDbType.Int32).Value = int.Parse(selectedOrgID);
                cmd.Parameters.Add(":CY", OracleDbType.Int32).Value = int.Parse(selectedCY);
                cmd.Parameters.Add(":CQ", OracleDbType.Int32).Value = int.Parse(selectedCQ);
                if (!string.IsNullOrEmpty(keyword))
                {
                    cmd.Parameters.Add(":keyword", OracleDbType.Varchar2).Value = keyword;
                }

                OracleDataReader reader = cmd.ExecuteReader();
                gvw1.DataSource = reader;
                gvw1.DataBind();
                gvw1.RenderControl(m_HtmlWriter);

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