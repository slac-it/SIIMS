using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class charting_test : System.Web.UI.Page
    {
        string _connStr;
        protected static readonly ILog Log = LogManager.GetLogger("charting_test");

        protected void Page_Load(object sender, EventArgs e)
        {
              _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
              getChartData();
        }

        private void getChartData()
        {
            Series series = Chart1.Series["Series1"];
            Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            int startFY = 2011;
            int endFY = 2018;
            int startFQ = 2;
            int endFQ = 1;



            string _sql = "select src.FY, src.Quarter, 'FY'||src.FY || '-Q'|| src.Quarter as date_range, count(src.FY) as Num_Issues  " +
                        " from  siims_source src join siims_issue iss on src.issue_id=iss.issue_id and  iss.is_active='Y' " +
                        " where  ((src.FY=:startFY and src.Quarter>= :startFQ) or (src.FY = :endFY and src.Quarter <= :endFQ) or (FY>:startFY and FY< :endFY)) " +
                        " group by src.FY,src.Quarter order by FY, Quarter ";

             try
                {
                    OracleConnection con = new OracleConnection();
                    con.ConnectionString = _connStr;
                    con.Open();

                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = _sql;
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":startFY", OracleDbType.Int32).Value = startFY;
                    cmd.Parameters.Add(":endFY", OracleDbType.Int32).Value = endFY;
                    cmd.Parameters.Add(":startFQ", OracleDbType.Int32).Value = startFQ;
                    cmd.Parameters.Add(":endFQ", OracleDbType.Int32).Value = endFQ;

                    OracleDataReader reader = cmd.ExecuteReader();
                    int NCount = 0;
                    int maxY = 0;
                    while (reader.Read())
                    {
                        string Xname =Convert.ToString( reader["date_range"]);
                        int numIssue = Convert.ToInt32(reader["Num_Issues"]);
                        series.Points.AddXY(Xname, numIssue);
                        NCount++;
                        if (numIssue > maxY) maxY = numIssue;
                    }

                    int multipleOf5 = (int)Math.Ceiling(1.0 * maxY / 5.0);
                    maxY = multipleOf5 * 5;

                    series.BorderWidth = 2;
                    //series.Font.
                    Chart1.ChartAreas[0].AxisY.Maximum = maxY;
                    Chart1.ChartAreas[0].AxisY.Minimum = 0;
                    Chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    Chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    series.IsValueShownAsLabel = true;

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
                    Log.Error("getChartData", ex);
                }

            // sql script:

         
        }

        protected void Chart1_DataBound(object sender, EventArgs e)
        {
            foreach( DataPoint p in Chart1.Series[0].Points)  {
                p.CustomProperties = "Exploded=true";
              
               
            }
   
        }
    }
}