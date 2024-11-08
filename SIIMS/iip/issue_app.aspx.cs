using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using SIIMS.App_Code;

namespace SIIMS.iip
{
    public partial class issue_app : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        int _Issue_IID = 0;

        protected static readonly ILog Log = LogManager.GetLogger("issue_app");

        protected void Page_Load(object sender, EventArgs e)
        {
                      
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


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
                Response.Redirect("~/default.aspx", true);
            }

            Helpers _helper = new Helpers();
            if (!_helper.IsLabManager(_loginSID))
            {
                // only IIP or Deputy director can do issue approval
                Response.Redirect("../issue_view.aspx?iid=" + _Issue_IID, true);
            }

            if (!Page.IsPostBack && _Issue_IID > 0)
            {
                bindControls();
            }

        }

        private void bindControls()
        {
            int sub_status_id=0;

            string _sql = " select issue.TITLE,issue.DESCRIPTION,org.name,issue.OWNER_SID, p.name as owner_name , issue.SIG_LEVEL,issue.STYPE_ID, stype.type, s.TITLE as STitle, s.FY, s.QUARTER , change.NEW_LEVEL, issue.sub_status_id " +
                " from siims_issue issue join siims_org org on issue.org_id=org.org_id join siims_source_type stype on stype.STYPE_ID=issue.stype_id left join siims_source s on issue.issue_id = s.issue_id " +
                "  left join persons.person p on p.key=owner_sid   left join siims_issue_change change on change.issue_id=issue.issue_id and change.is_active='Y' where issue.issue_id=:IID ";
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
                    lblDesc.Text = reader.GetString(1);
                    lblOrg.Text = reader.GetString(2);
                    lblOwner.Text = reader.GetString(4);
                    lblLevel.Text = reader.GetString(5);

                    if (!reader.IsDBNull(11))
                    {
                        lblNewLevel.Text = reader.GetString(11);
                    }
                    sub_status_id = reader.GetInt32(12);

                    string _sType = reader.GetString(6);
                    lblSType.Text = reader.GetString(7);
                    //if (_sType != null)
                    //{
                    //    PanelSourceTitle.Visible = true;
                    //    txtSourceTitle.Text = reader.GetString(8);
                    //    txtSourceFY.Text = reader.GetString(9);
                    //    txtSourceQtr.Text = reader.GetString(10);
                    //}
                    //else
                    //{
                    //    PanelSourceTitle.Visible = false;
                    //}


                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindControls", ex);
            }

          
            if (sub_status_id == 110)
            {
                PanelNewLevel.Visible = true;
            }

            if (sub_status_id == 111)
            {
                PanelDeletion.Visible = true;
            }
       
        }

     

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx");
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            updateDB("Y");
            Response.Redirect("../default.aspx");
            
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            updateDB("N");
            Response.Redirect("../default.aspx");
        }

        private void updateDB(string p_isApproved)
        {
            string _note = txtNote.Text.Trim();

          
          
            try
            {
                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText = "SIIMS_ISSUE_PKG.PROC_ISSUE_APPROVE";

                    OracleParameter inIssueID = _cmd.Parameters.Add("PI_ISSUE_ID", OracleDbType.Int32);
                    inIssueID.Direction = ParameterDirection.Input;
                    inIssueID.Value = _Issue_IID;

                    OracleParameter inISApproved = _cmd.Parameters.Add("PI_IS_APPROVED", OracleDbType.Varchar2);
                    inISApproved.Direction = ParameterDirection.Input;
                    inISApproved.Value = p_isApproved;

                    OracleParameter inNote = _cmd.Parameters.Add("PI_NOTE", OracleDbType.Varchar2);
                    inNote.Direction = ParameterDirection.Input;
                    inNote.Value = _note;

                    OracleParameter inLogin = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogin.Direction = ParameterDirection.Input;
                    inLogin.Value = _loginSID;

                    _cmd.ExecuteNonQuery();

                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("updateDB", ex);

            }
        }
    }
}