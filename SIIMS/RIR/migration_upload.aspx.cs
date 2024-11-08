using log4net;
using Oracle.ManagedDataAccess.Client;
using SIIMS.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class migration_upload : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int FID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("migration_upload");
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
                bindStaticControls(FID);
            }
            else
            {
                Response.Redirect("migration_list.aspx", true);
            }
        }

        private void bindStaticControls(int _iid)
        {
            string _sql = @"select issue_id,title
                            from APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION 
                            where issue_id=:IID";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Varchar2).Value = _iid;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lblIssueID.Text = reader.GetInt32(0).ToString();
                    lblTitle.Text = reader.GetString(1);

                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindStaticControls", ex);
            }

        }

        protected void btnUploadPDF_Click(object sender, EventArgs e)
        {

            if (oFilePDF.HasFile)
            {
                string filename = Path.GetFileName(oFilePDF.PostedFile.FileName);
                int file_size = oFilePDF.PostedFile.ContentLength;
                string content_type = oFilePDF.PostedFile.ContentType;

                FileUtil objFile = new FileUtil();
                string errorMsg = objFile.checkFilePDF(filename, file_size);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    lblUploadPDF.Visible = true;
                    lblUploadPDF.Text = errorMsg;
                    return;
                }
                Byte[] _filedata = objFile.GetByteArrayFromFileField(oFilePDF.PostedFile);

                OracleConnection _conn = null;
                try
                {
                    _conn = new OracleConnection(_connStr);
                    _conn.Open();
                    OracleCommand _cmd = new OracleCommand("", _conn);
                    _cmd.CommandText = " UPDATE APPS_ADMIN.SIIMS_RIR_DATA_MIGRATION  SET PDF_DATA=:FILE_DATA,PDF_FILENAME=:fielName, PDF_ON=sysdate,PDF_BY=:loginSID where issue_id=:IID";
                    _cmd.BindByName = true;
                    _cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = FID;
                    _cmd.Parameters.Add(":fielName", OracleDbType.Varchar2).Value = filename;
                    _cmd.Parameters.Add(":FILE_DATA", OracleDbType.Blob).Value = _filedata;
                    _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                    _cmd.ExecuteNonQuery();

                    lblUploadPDF.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.Error("btnUploadPDF_Click: DB Insertion", ex);
                }

                Response.Redirect("migration_list.aspx");

            }
            else
            {
                lblUploadPDF.Text = "Click 'Choose File' and browse to select the file to upload.";
                lblUploadPDF.Visible = true;
            }


        }
    }
}