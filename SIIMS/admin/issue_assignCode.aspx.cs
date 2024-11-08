using log4net;
using Oracle.ManagedDataAccess.Client;
using SIIMS.App_Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class issue_assignCode : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = 0;
        protected static readonly ILog Log = LogManager.GetLogger("issue_assignCode");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            {
                Response.Redirect("../permission.aspx");
            }

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            if (Request["iid"] != null)
            {
                _Issue_IID = int.Parse(Request["iid"].ToString());
            }
            else
            {
                Response.Redirect("issue_code.aspx", true);
            }



            if (!Page.IsPostBack && _Issue_IID > 0)
            {
                bindControls();
                BindFileGrid();
                bindTrendingCode();
            }

        }

        private void bindTrendingCode()
        {
            string _sql = " select ICODE_ID, IS_WORKTYPE from siims_issue_code where issue_id=:IID and IS_ACTIVE='Y' ";
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                int iCount = 0;
                while (reader.Read())
                {
                    int icode_id = reader.GetInt32(0);
                    string is_worktype = reader.GetString(1);
                    if (is_worktype == "Y")
                    {
                        drw_WorkType.SelectedValue = icode_id.ToString();
                    }
                    else
                    {
                        iCount++;
                        if (iCount==1) drw_Functional1.SelectedValue = icode_id.ToString();
                        if (iCount == 2) drw_Functional2.SelectedValue = icode_id.ToString();
                        if (iCount == 3) drw_Functional3.SelectedValue = icode_id.ToString();
                    }


                }
              
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindTrendingCode", ex);
            }
        }

        private void bindControls()
        {
            string _sql = "   select TITLE,DESCRIPTION,ORG_ID,OWNER_SID,SIG_LEVEL,STYPE_ID,  STitle, FY, QUARTER, owner_name " +
            " , status_id, status, sub_status, sub_status_id, TYPE , NAME " +
            " from VW_SIIMS_ISSUE_VIEW where issue_id=:IID ";
            try
            {
                lblIssueID.Text = _Issue_IID.ToString();
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTitle.Text = reader.GetString(0);
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    lblDesc.Text = desc.Replace("\r", "<br />");
                    //lblDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        //lblOrg.Text = reader.GetInt32(2).ToString();
                        lblOrg.Text = reader.GetString(15);
                    }


                    string status = "";
                    if (!reader.IsDBNull(11))
                    {
                        status = reader.GetString(11);

                        if (!reader.IsDBNull(13) && status == "Open")
                        {
                            status += ": " + reader.GetString(12);
                        }
                        lblStatus.Text = status;
                    }



                    if (!reader.IsDBNull(4))
                    {
                        lblLevel.Text = reader.GetString(4);
                    }

                    string _sType = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    if (_sType.Length > 0)
                    {
                        lblSType.Text = reader.GetString(14);

                        lblSourceTitle.Text = reader.GetString(6);
                        lblSourceFY.Text = reader.GetString(7);
                        lblSourceQtr.Text = reader.GetString(8);
                    }


                    lblOwner.Text = reader.GetString(9);



                }
                else
                {
                    Response.Redirect("permission.aspx");
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }
        }

        private void BindFileGrid()
        {
            // TBD: 

            DataSet ds = new DataSet();

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.BindByName = true;
                    sqlCmd.CommandText = "select ISATT_ID,FILE_NAME, CREATED_ON from SIIMS_ISSUE_ATT where issue_id=:IID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_IID;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }


            lv_Files.DataSource = ds;
            lv_Files.DataBind();

        }

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {

        }

        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "download")
            {
                FileData(_attachmentId);
            }

        }

        private void FileData(int _attachmentId)
        {

            FileUtil objFile = new FileUtil();
            bool isError = objFile.downLoadAttachment(_attachmentId, 1);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request["from"] != null && Request["from"].ToString() == "e")
            {
                Response.Redirect("issue_editcode.aspx");
            }
            else
            {
                Response.Redirect("issue_code.aspx");
            }
          
        }

        public string FORMATACTIONStatus(string status, string due_date)
        {
            string msg = "";
            if (status == "Open")
            {
                msg = due_date;
            }
            else
            {
                msg = status;
            }

            return msg;
        }


        protected void GVActions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //GridView gvTemp = (GridView)sender;

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    string actionId = (gvTemp).DataKeys[e.Row.RowIndex].Value.ToString();

            //    Label lblStatusID = e.Row.FindControl("lblStatusID") as Label;

            //}

            //Check if this is our Blank Row being databound, if so make the row invisible
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (((DataRowView)e.Row.DataItem)["Action_ID"].ToString() == String.Empty) e.Row.Visible = false;
            //}
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // check the functional areas are difference if set 
            // write data to the DB
            // 
            bool checkError = false;

            string farea1 = drw_Functional1.SelectedValue;
            string farea2 = drw_Functional2.SelectedValue;
            string farea3 = drw_Functional3.SelectedValue;

            if (farea1 != "0" && farea2 != "0")
            {
                if (farea1 == farea2)
                {
                    checkError = true;
                }
            }

            if (farea1 != "0" && farea3 != "0")
            {
                if (farea1 == farea3)
                {
                    checkError = true;
                }
            }

            if (farea2 != "0" && farea3 != "0")
            {
                if (farea3 == farea2)
                {
                    checkError = true;
                }
            }

            if (checkError)
            {
                lblError.Visible = true;
                return;
            }

            string workTypeID = drw_WorkType.SelectedValue;
            saveIssueCode2DB(workTypeID,farea1,farea2,farea3);
           // the algorithm is: delete all for the issue_id and insert if new code id is not zero
            // will use a packaged procedure to do this

            // go back to the list
            if (Request["from"] != null && Request["from"].ToString() == "e")
            {
                Response.Redirect("issue_editcode.aspx");
            }
            else
            {
                Response.Redirect("issue_code.aspx");
            }
        }

        private void saveIssueCode2DB(string workType_ID, string function1_id, string function2_id, string function3_id)
        {

            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ISSUE_CODE_PKG.PROC_ISSUE_CODE_INSERT";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Issue_IID;

                    OracleParameter inWorkTypeID = _cmd.Parameters.Add("PI_WORKTYPE_ID", OracleDbType.Int32);
                    inWorkTypeID.Direction = ParameterDirection.Input;
                    inWorkTypeID.Value = int.Parse(workType_ID);

                    OracleParameter inFuntion1 = _cmd.Parameters.Add("PI_FUNCTION1_ID", OracleDbType.Int32);
                    inFuntion1.Direction = ParameterDirection.Input;
                    inFuntion1.Value = int.Parse(function1_id);

                    OracleParameter inFuntion2 = _cmd.Parameters.Add("PI_FUNCTION2_ID", OracleDbType.Int32);
                    inFuntion2.Direction = ParameterDirection.Input;
                    inFuntion2.Value = int.Parse(function2_id);

                    OracleParameter inFuntion3 = _cmd.Parameters.Add("PI_FUNCTION3_ID", OracleDbType.Int32);
                    inFuntion3.Direction = ParameterDirection.Input;
                    inFuntion3.Value = int.Parse(function3_id);



                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("saveIssueCode2DB", ex);

            }
        }
    }
}