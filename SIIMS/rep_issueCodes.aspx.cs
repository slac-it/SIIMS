using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_issueCodes : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("rep_issueCodes");
        string _connStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string filter_query = "";
            string data_range="";

            string start_year = drwFY_start.SelectedValue;
            string end_year = drwFY_end.SelectedValue;
            string start_qtr = drwQtr_start.SelectedValue;
            string end_qtr = drwQtr_end.SelectedValue;

            int int_sFY=int.Parse(start_year);
            int int_eFY=int.Parse(end_year);
            int int_sQtr=int.Parse(start_qtr);
            int int_eQtr=int.Parse(end_qtr);
            if (int_sFY > int_eFY)
            {
                lblErrMsg.Text = "Error: End Date must be later than Start Date.";
                lblErrMsg.Visible = true;
                return;
            }

            if (int_sFY == int_eFY)
            {
                if (int_sQtr > int_eQtr)
                {
                    lblErrMsg.Text = "Error: End Date must be later than Start Date.";
                    lblErrMsg.Visible = true;
                    return;
                }

                filter_query = " (s.FY=" + start_year + " and s.QUARTER between " + int_sQtr + " and " + int_eQtr + ")";
            }
            else
            {
                filter_query = "( (s.FY >" + start_year + " and s.FY < " + int_eFY + ")" +
                                " or (s.FY=" + start_year + " and s.QUARTER >= " + start_qtr + ") " +
                                " or (s.FY=" + end_year + " and s.QUARTER <= " + end_qtr + ") )";

            }

            bindWorkType(filter_query);
            bindFunctional(filter_query);

            Session["FILTER1"] = filter_query;

            PanelSelection.Visible = false;
            PanelReport.Visible = true;
            PanelCharting.Visible = false;

            data_range="Data Range: FY"+start_year+"-Q"+start_qtr+" to FY" + end_year + "-Q" +end_qtr;
            lblRange.Text = data_range;
            lblDataRange2.Text = data_range;

        }

      

        private void bindFunctional(string filter_query)
        {
            string _sql = " select icode.CATEGORY || ' - ' || icode.SUB as functional, icode.icode_id, count(issue_id) as NCount from siims_issue_code fcode " +
                        " join SIIMS_ITRENDING_CODE icode on icode.ICODE_ID=fcode.ICODE_ID and icode.IS_WORKTYPE='N' where fcode.IS_WORKTYPE='N' and fcode.ISSUE_ID in ( " +
                        " select s.issue_id from siims_source s where exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='Y') " +
                        " AND exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='N') AND " + filter_query + ")"  +
                        "  group by icode.CATEGORY || ' - ' || icode.SUB, icode.icode_id order by 3 desc, 1";

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = _sql;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                string maxCount = ds.Tables[0].Rows[0][2].ToString();
                if (maxCount != "0")
                {
                    MAXFUNCTIONAL.Value = maxCount;
                }
            }

           

            grw_functional.DataSource = ds;
            grw_functional.DataBind();

        }

        private void bindWorkType(string filter_query)
        {
            string _sql = " select icode.WORKTYPE, icode.icode_id, count(s.issue_id) as ncount from siims_source s join siims_ISSUE_CODE wcode on wcode.issue_id = s.issue_id and wcode.IS_WORKTYPE='Y' " +
                        " join SIIMS_ITRENDING_CODE icode on icode.ICODE_ID=wcode.ICODE_ID and icode.IS_WORKTYPE='Y' " +
                         " join siims_ISSUE_CODE fcode on fcode.issue_id = s.issue_id and fcode.IS_WORKTYPE='N' and fcode.order_id=1 " +
                        " where " + filter_query + " group by icode.WORKTYPE, icode.icode_id order by 3 desc, 1 ";

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = _sql;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                string maxCount = ds.Tables[0].Rows[0][2].ToString();
                if (maxCount != "0")
                {
                    MAXWORKTYPE.Value = maxCount;
                }

                //btnExpCode.Visible = true;
                lnkexpCode.Visible = true;
            }
            else
            {
                //btnExpCode.Visible = false;
                lnkexpCode.Visible = false;
            }
           

            grw_workType.DataSource = ds;
            grw_workType.DataBind();

    
        }

        protected void wt_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow gvRow = grw_workType.Rows[index];
            int iCode_ID=Convert.ToInt32(grw_workType.DataKeys[gvRow.RowIndex].Value);

            PanelCharting.Visible = true;
            PanelReport.Visible = false;
            PanelSelection.Visible = false;

            string iCode=bindIssueGraphTitle(iCode_ID,"Y");
            //lblIssueTitle.Text = "Issues by Functional Area for Work Type: " + iCode;
            lblIssueTitle.Text = "Issues by Functional Area";

            // bind functional areas 
            bindIssueCode(iCode_ID);
            bindActionType(iCode_ID);
            bindChartData(iCode_ID,"Y");
            

        }

        private void bindChartData(int iCode_ID, string is_WT)
        {
            Series series = Chart1.Series["Series1"];

            string title = bindIssueGraphTitle(iCode_ID, is_WT);
            Chart1.Titles[0].Text = title + " Issues";

            Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            string filter_query = Session["FILTER1"].ToString();
            string filter_query2 = filter_query.Replace("s.", "ss.");
            string _sql = "Select distinct ss.FY ,SS.Quarter,  'FY'|| ss.FY || '-Q'|| SS.Quarter as FYQ, (select count(*) from siims_issue_code fcode  " +
                       " join SIIMS_ITRENDING_CODE icode on icode.ICODE_ID=fcode.ICODE_ID and icode.ICODE_ID=fcode.ICODE_ID " +
                       " join siims_source s on fcode.ISSUE_ID=s.ISSUE_ID and " + filter_query +
                       " join siims_issue_code wcode on wcode.issue_id=fcode.issue_id and ";
            if (is_WT =="Y") {
                 _sql += " wcode.IS_WORKTYPE='N' and wcode.ORDER_ID=1 ";
            } else {
                 _sql += " wcode.IS_WORKTYPE='Y'  ";
            }
             _sql +=   " where fcode.ICODE_ID=:ICODE_ID and s.FY=ss.FY and s.Quarter=ss.Quarter) NCOUNT " +
                       " from siims_source ss where " + filter_query2 +
                       " order by ss.FY ,SS.Quarter ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = iCode_ID;

                OracleDataReader reader = cmd.ExecuteReader();
                int maxY = 0;
                int NCount = 0;
                while (reader.Read())
                {
                    string Xname = Convert.ToString(reader["FYQ"]);
                    int numIssue = Convert.ToInt32(reader["NCOUNT"]);
                    if (numIssue > maxY) maxY = numIssue;
                    series.Points.AddXY(Xname, numIssue);
                    NCount++;
                }

                int multipleOf5=(int) Math.Ceiling(1.0* maxY / 5.0);
                maxY = multipleOf5 * 5;

              

                series.BorderWidth = 2;
                //series.Font.
                Chart1.ChartAreas[0].AxisY.Maximum = maxY;
                Chart1.ChartAreas[0].AxisY.Minimum = 0;
                Chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                Chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                //series.IsValueShownAsLabel = true;
           
                if (NCount <= 8)
                {
                    Chart1.ChartAreas[0].AxisX.Interval = 1;
                }
                else if (NCount <= 16)
                {
                    Chart1.ChartAreas[0].AxisX.Interval = 2;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                }
                else if (NCount <= 32)
                {
                    Chart1.ChartAreas[0].AxisX.Interval = 4;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                }
                else
                {
                    Chart1.ChartAreas[0].AxisX.Interval = 8;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
                    Chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                }

                series.MarkerColor = Color.Red;
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 6;
              

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindChartData", ex);
            }

        }

        private void bindChartTitle(int iCode_ID)
        {
            throw new NotImplementedException();
        }


        private string bindIssueGraphTitle(int iCode_ID, string is_worktype)
        {
            string _sql = "";
            string icode="";
            if (is_worktype == "Y")
            {
                _sql = " select WORKTYPE from SIIMS_ITRENDING_CODE where ICODE_ID=:ICODE_ID and IS_WORKTYPE='Y' ";
            }
            else
            {
                _sql = " select CATEGORY || ' - ' || SUB as functional from SIIMS_ITRENDING_CODE where ICODE_ID=:ICODE_ID and IS_WORKTYPE='N' ";
            }
            
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = iCode_ID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    icode = reader.GetString(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindIssueGraphTitle", ex);
            }

            return icode;
        }

        private void bindActionType(int iCode_ID)
        {
            string filter_query = Session["FILTER1"].ToString();


            string _sql = " select acode.acode, count(ac.action_id) as NCOUNT from siims_action ac join siims_atrending_code acode on acode.acode_id=ac.CAT_ACODE_ID " +
                        " where ac.CAT_ACODE_ID is not null and ac.SUB_ACODE_ID is not null and ac.IS_ACTIVE='Y' and ac.ISSUE_ID in ( " +
                        " select s.issue_id from siims_source s where exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='Y') " +
                        " AND exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='N') AND " + filter_query + ")" +
                        "  and ac.issue_id in (select issue_id from siims_issue_code where icode_id=:ICODE_ID) " +
                        "  group by acode order by 2 desc, 1";

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = _sql;
                    sqlCmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = iCode_ID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                string maxCount = ds.Tables[0].Rows[0][1].ToString();
                if (maxCount != "0")
                {
                    MAXACTIONTYPE.Value = maxCount;
                }
                lnkExpActions.Visible = true;
                lnkExpActions.NavigateUrl = "rep_expActionCode.aspx?icodeid=" + iCode_ID;
            }
            else
            {
                lnkExpActions.Visible = false;
            }
           

            grw_ActionCodes.DataSource = ds;
            grw_ActionCodes.DataBind();
        }

        public string FormatActionType(int max_Col, string str_NCount)
        {
            int max_Count = int.Parse(MAXACTIONTYPE.Value);
            int _NCount = int.Parse(str_NCount);
            double fwidth = _NCount * max_Col * 1.0 / max_Count;
            int box_width = (int)Math.Floor(fwidth);

            string tmp = "<div style='width:" + box_width + "px; background-color:#5D7B9D; height:15px; float:left;'></div>";

            return tmp;

        }

        private void bindIssueCode(int iCode_ID)
        {
            string filter_query = Session["FILTER1"].ToString();


            string _sql = " select icode.CATEGORY || ' - ' || icode.SUB as icode,  count(issue_id) as NCount from siims_issue_code fcode " +
                        " join SIIMS_ITRENDING_CODE icode on icode.ICODE_ID=fcode.ICODE_ID and icode.IS_WORKTYPE='N' where fcode.IS_WORKTYPE='N' and fcode.ISSUE_ID in ( " +
                        " select s.issue_id from siims_source s where exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='Y') " +
                        " AND exists (select * from siims_issue_code where issue_id=s.issue_id and is_worktype='N') AND " + filter_query + ")" +
                        " and fcode.issue_id in (select issue_id from siims_issue_code where icode_id=:ICODE_ID) "  +
                        "  group by icode.CATEGORY || ' - ' || icode.SUB order by 2 desc, 1";

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = _sql;
                    sqlCmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = iCode_ID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                string maxCount = ds.Tables[0].Rows[0][1].ToString();
                if (maxCount != "0")
                {
                    MAXISSUECODE.Value = maxCount;
                }
                lnkExpIssues.Visible = true;
                lnkExpIssues.NavigateUrl= "rep_expIssueCode.aspx?icodeid=" + iCode_ID;
            }
            else
            {
                lnkExpIssues.Visible = false;
            }


            grw_IssueCodes.DataSource = ds;
            grw_IssueCodes.DataBind();
        }

        public string FormatIssueCode(int max_Col, string str_NCount)
        {
            int max_Count = int.Parse(MAXISSUECODE.Value);
            int _NCount = int.Parse(str_NCount);
            double fwidth = _NCount * max_Col * 1.0 / max_Count;
            int box_width = (int)Math.Floor(fwidth);

            string tmp = "<div style='width:" + box_width + "px; background-color:#5D7B9D; height:15px; float:left;'></div>";

            return tmp;

        }

        protected void fn_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow gvRow = grw_functional.Rows[index];
            int iCode_ID = Convert.ToInt32(grw_functional.DataKeys[gvRow.RowIndex].Value);

            PanelCharting.Visible = true;
            PanelReport.Visible = false;
            PanelSelection.Visible = false;

            string iCode = bindIssueGraphTitle(iCode_ID, "N");
            //lblIssueTitle.Text = "Issues by Work Type for Funtional Area: " + iCode;
            lblIssueTitle.Text = "Issues by Work Type";

            bindIssueCode_WT(iCode_ID);
            bindActionType(iCode_ID);
            bindChartData(iCode_ID, "N");
        }

        private void bindIssueCode_WT(int iCode_ID)
        {
            string filter_query = Session["FILTER1"].ToString();

            string _sql = " select icode.WORKTYPE as icode,  count(s.issue_id) as ncount from siims_source s join siims_ISSUE_CODE wcode on wcode.issue_id = s.issue_id and wcode.IS_WORKTYPE='Y' " +
                        " join SIIMS_ITRENDING_CODE icode on icode.ICODE_ID=wcode.ICODE_ID and icode.IS_WORKTYPE='Y' " +
                         " join siims_ISSUE_CODE fcode on fcode.issue_id = s.issue_id and fcode.IS_WORKTYPE='N' and fcode.order_id=1 " +
                         " join siims_ISSUE_CODE fcode2 on fcode2.issue_id = s.issue_id and fcode2.IS_WORKTYPE='N' and fcode2.icode_id=:ICODE_ID "  +
                        " where " + filter_query + " group by icode.WORKTYPE order by 2 desc, 1 ";

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = _sql;
                    sqlCmd.Parameters.Add(":ICODE_ID", OracleDbType.Int32).Value = iCode_ID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                string maxCount = ds.Tables[0].Rows[0][1].ToString();
                if (maxCount != "0")
                {
                    MAXISSUECODE.Value = maxCount;
                }

                lnkExpIssues.Visible = true;
                lnkExpIssues.NavigateUrl = "rep_expIssueCode.aspx?icodeid=" + iCode_ID;
            }
            else
            {
                lnkExpIssues.Visible = false;
            }

          

            grw_IssueCodes.DataSource = ds;
            grw_IssueCodes.DataBind();


        }

     

        public string FormatWorkType(int max_Col, string str_NCount)
        {
            int max_Count = int.Parse(MAXWORKTYPE.Value);
            int _NCount = int.Parse(str_NCount);
            double fwidth = _NCount * max_Col * 1.0 / max_Count;
            int box_width =(int) Math.Floor(fwidth);

            string tmp = "<div style='width:" + box_width + "px; background-color:#5D7B9D; height:15px; float:left;'></div>";

            return tmp;

        }

        public string FormatFunctional(int max_Col, string str_NCount)
        {
            int max_Count = int.Parse(MAXFUNCTIONAL.Value);
            int _NCount = int.Parse(str_NCount);
            double fwidth = _NCount * max_Col * 1.0 / max_Count;
            int box_width = (int)Math.Floor(fwidth);

            string tmp = "<div style='width:" + box_width + "px; background-color:#5D7B9D; height:15px; float:left;'></div>";

            return tmp;

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            drwFY_start.SelectedValue = "-1";
            drwFY_end.SelectedValue = "-1";
            drwQtr_start.SelectedValue = "-1";
            drwQtr_end.SelectedValue = "-1";
        }

        protected void btnBack2Graph_Click(object sender, EventArgs e)
        {
            PanelCharting.Visible = false;
            PanelReport.Visible = true;
            PanelSelection.Visible = false;
        }

       

        protected void btnBack2Selection_Click(object sender, EventArgs e)
        {
            PanelCharting.Visible = false;
            PanelReport.Visible = false;
            PanelSelection.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("reports.aspx");
        }
    }
}