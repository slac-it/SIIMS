using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class migration_download : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int FID = 0;
        protected static readonly ILog Log = LogManager.GetLogger("Migration_download");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();

            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            if (Request["iid"] != null)
            {
                FID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("default.aspx", true);
            }

            // check the PDF is ready
            outputPdf();

        }

      
        private void outputPdf()
        {
            int pi_sid = 0;
            byte[] byteArray = null;
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                string _sql = _sql = " select  pdf_data from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION  WHERE issue_id=:IID ";

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;
                _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = FID;

                try
                {
                    _conn.Open();
                    using (OracleDataReader dataReader = _cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                byteArray = (Byte[])(dataReader.GetOracleBlob(0)).Value;
                            }
                        }
                        else
                        {
                            Response.Redirect("../permission.aspx");
                        }
                    }

                }
                catch (Exception ex)
                {
                    byteArray = null;
                    Log.Error("outputPdf()", ex);

                }
                finally
                {
                    _cmd.Dispose();
                    _conn.Close();

                }
            }


            if (byteArray != null)
            {
                string fileName = "Report_" + FID.ToString() + ".pdf";
             

                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.AddHeader("Content-Type", "application/pdf");
                response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}; size={1}", fileName, byteArray.Length.ToString()));
                response.BinaryWrite(byteArray);
                // Note: it is important to end the response, otherwise the ASP.NET
                // web page will render its content to PDF document stream
                response.End();
            }
            else
            {
                Response.Redirect("migration_view.aspx?iid=" + FID);
            }

        }
    }
}