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
    public partial class action_assignCode : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Action_AID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("action_view");
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
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);


            if (Request["aid"] != null)
            {
                _Action_AID = int.Parse(Request["aid"].ToString());
            }
            else
            {
                Response.Redirect("action_code.aspx", true);
            }

            if (!Page.IsPostBack && _Action_AID > 0)
            {
                int issue_ID = bindControls();
                bindIssueControls(issue_ID);
                BindFileGrid();
            }

        }


        private void bindIssueControls(int _Issue_IID)
        {
            lblIssueID.Text = _Issue_IID.ToString();
            string _sql = "   select TITLE,DESCRIPTION,ORG_ID,OWNER_SID,SIG_LEVEL,STYPE_ID,  STitle, FY, QUARTER, owner_name " +
             " , status_id, status, sub_status, sub_status_id, TYPE , NAME " +
             " from VW_SIIMS_ISSUE_VIEW where issue_id=:IID ";
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
                if (reader.Read())
                {
                    lblITitle.Text = "<a href='../issue_view.aspx?iid=" + _Issue_IID + "'>" + reader.GetString(0) + "</a>";
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    desc = desc.Replace("\r", "<br />");
                    lblIDesc.Text = desc;
                    if (desc.Length > 100)
                    {
                        string desc1 = desc.Substring(0, Math.Min(desc.Length, 100));
                        desc1 = desc1 + " ... &nbsp; &nbsp; <a href='javascript:showFull();'>Read More</a>";

                        lblIDescShort.Text = desc1;
                        lblIDescShort.Visible = true;
                        lblIDesc.CssClass = "invisibeText";
                    }
                    else
                    {
                        lblIDescShort.Visible = false;

                    }

                    //lblDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        //lblOrg.Text = reader.GetInt32(2).ToString();
                        lblOrg.Text = reader.GetString(15);
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


                    lblIOwner.Text = reader.GetString(9);



                }
                else
                {
                    Response.Redirect("action_code.aspx");
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindIssueControls", ex);
            }
        }

        private int bindControls()
        {
          
            int issue_id = 0;
            string _sql = "  select action.title,action.description, to_char(action.due_date,'MM/DD/YYYY') as duedate,p.name,sta.status, action.issue_id " +
                 "  ,NVL(action.cat_acode_id,0) cat_id, NVL(action.sub_acode_id,0) sub_id " +
                   " from siims_action action left join siims_status sta on action.status_id=sta.STATUS_ID left join persons.person p on p.key=action.owner_sid " +
                " where action.action_id=:AID ";
            try
            {
                lblAID2.Text = 'A' + _Action_AID.ToString();
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblATitle.Text = reader.GetString(0);
                    string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);

                    lblADesc.Text = desc.Replace("\r", "<br />");

                    lblDue.Text = reader.IsDBNull(2) ? "" : FormatDueDate(reader.GetString(2));
                    lblAOwner.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    lblStatus.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    issue_id = reader.GetInt32(5);
                    int cat_id = reader.GetInt32(6);
                    int sub_id = reader.GetInt32(7);
                    drw_cat.SelectedValue = cat_id.ToString();
                    drw_sub.SelectedValue = sub_id.ToString();
                }
                else
                {
                    Response.Redirect("action_code.aspx");
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

            return issue_id;
        }

        public string FormatDueDate(string _dueDate)
        {
            DateTime dueDate = DateTime.Parse(_dueDate);
            TimeSpan ts = DateTime.Now.Date - dueDate;
            // Difference in days.
            int differenceInDays = ts.Days;
            if (differenceInDays > 0)
            {
                return " <span style='color:red; font-weight:bold;'>" + _dueDate + " </span>";
            }
            else
            {
                return _dueDate;
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
                    sqlCmd.CommandText = "select ACATT_ID,FILE_NAME, CREATED_ON from SIIMS_ACTION_ATT where action_id=:AID and IS_ACTIVE='Y' ";
                    sqlCmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_AID;
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
            bool isError = objFile.downLoadAttachment(_attachmentId, 2);
            if (isError)
            {
                lblMsg.Text = "Error: empty data!";
                lblMsg.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            if (Request["from"] != null && Request["from"].ToString() == "a")
            {
                Response.Redirect("action_code.aspx");
            }
         
            else
            {
                Response.Redirect("action_editcode.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            string catID = drw_cat.SelectedValue;
            string subID = drw_sub.SelectedValue;
            saveActionType2DB(catID, subID);
           // the algorithm is: delete all for the issue_id and insert if new code id is not zero
            // will use a packaged procedure to do this

            // go back to the list
            if (Request["from"] != null && Request["from"].ToString() == "a")
            {
                Response.Redirect("action_code.aspx");
            }

            else
            {
                Response.Redirect("action_editcode.aspx");
            }
        }

        private void saveActionType2DB(string cat_ID, string sub_id)
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
                    _cmd.CommandText = "SIIMS_ISSUE_CODE_PKG.PROC_ACTION_TYPE_INSERT";
                    OracleParameter inLoginSid = _cmd.Parameters.Add("PI_CREATED_BY", OracleDbType.Int32);
                    inLoginSid.Direction = ParameterDirection.Input;
                    inLoginSid.Value = _loginSID;

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Action_AID;

                    OracleParameter inWorkTypeID = _cmd.Parameters.Add("PI_CAT_ID", OracleDbType.Int32);
                    inWorkTypeID.Direction = ParameterDirection.Input;
                    inWorkTypeID.Value = int.Parse(cat_ID);

                    OracleParameter inFuntion1 = _cmd.Parameters.Add("PI_SUB_ID", OracleDbType.Int32);
                    inFuntion1.Direction = ParameterDirection.Input;
                    inFuntion1.Value = int.Parse(sub_id);

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("saveActionType2DB", ex);

            }
        }

       
    }
}