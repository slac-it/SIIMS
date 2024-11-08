using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class migration_view : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = -1;
        protected static readonly ILog Log = LogManager.GetLogger("migration_view");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Log.Error("Error: _logSID is empty!");
                Response.Redirect("../permission.aspx", true);
            }
            _loginSID = int.Parse(Session["LoginSID"].ToString());
           
            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("../permission.aspx", true);
            }

            if (!Page.IsPostBack)
            {
                bindControls();
                //BindFileGrid();

            }


       }

        private void bindControls()
        {
            string _sql = @"select  TITLE,statement,initiator_name_generated, to_char(initiated_on_generated, 'MM/DD/YYYY') as date_init
                        ,smt_org_orig as Org,GROUP_GENERATED as DEPT_NAME
                        ,poc_name_generated ,to_char(DATE_DISCOVERED_GENERATED, 'MM/DD/YYYY') as date_disc
                        ,LOCATION,CONDITION_GENERATED
                        ,sig_level, rir_code_generated,to_char(issued_date_generated, 'MM/DD/YYYY') as date_issued
                        ,source_title,CY,CQ,SIIMS_STATUS_ORIG
                        from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION 
                         where issue_id=:IID ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    lblTitle.Text = reader.GetString(0);
                    lblStatement.Text = reader.IsDBNull(1) ? "&nbsp;" : reader.GetString(1).Replace("\n", "<br />");
                    lblInitiator.Text = reader.GetString(2);
                    lblDateCreated.Text = reader.GetString(3);
                    lblOrg.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    lblDept.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    lblPOC.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    lblDateDisc.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);
                    lblLocation.Text = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    lblCondition.Text = reader.IsDBNull(9) ? "" : reader.GetString(9);

                    
                   
                    lblLevel.Text = reader.IsDBNull(10) ? "" : reader.GetString(10);
                    lblCode.Text = reader.IsDBNull(11) ? "" : reader.GetString(11);

                     lblIssueDate.Text = reader.IsDBNull(12) ? "" : reader.GetString(12);

                    lblSource.Text = reader.IsDBNull(13) ? "" : reader.GetString(13);
                    lblCY.Text = reader.IsDBNull(14) ? "" : reader.GetString(14);
                    lblCQ.Text = reader.IsDBNull(15) ? "" : reader.GetString(15);
                    lblIssue_Status.Text= reader.IsDBNull(16) ? "" : reader.GetString(16);
                }

                reader.Close();
                con.Close();

            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }
        }

     

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                outputPdf(_attachmentId);
            }
            //if (e.CommandName.ToLower() == "delete")
            //{
            //    DeleteAttachment(_attachmentId);

            //}

        }

        private void outputPdf(int _iid)
        {
            int pi_sid = 0;
            byte[] byteArray = null;
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                string _sql = _sql = " select issue_id, pdf_data from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION  WHERE issue_id=:IID  ";

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;
                _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _iid;

                try
                {
                    _conn.Open();
                    using (OracleDataReader dataReader = _cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                pi_sid = dataReader.GetInt32(0);
                                byteArray = (Byte[])(dataReader.GetOracleBlob(1)).Value;
                            }
                        }
                        else
                        {
                            Response.Redirect("permission.aspx");
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
                string fileName = "";
                fileName = "Attachment_for_" + _iid.ToString() + ".pdf";
              

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
                Response.Redirect("migration_view.aspx?iid=" + _iid);
            }

        }
    }
}