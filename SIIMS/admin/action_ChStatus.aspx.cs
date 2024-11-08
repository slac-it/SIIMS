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

namespace SIIMS.admin
{
    public partial class action_ChStatus : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("action_ChStatus");
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
        }



        protected void btnClear_Click(object sender, EventArgs e)
        {
            //txtSLACID.Text = "";
            txtOwner.Enabled = true;
            txtOwner.Text = "";
            ddlStatuslist.Items.Clear();
            Panel1.Visible = false;
            PanelOK.Visible = false;

            lblAStatus.Text = "";
            lblATitle.Text = "";
            lblIStatus.Text = "";
            lblITitle.Text = "";
            lblError.Visible = false;
            cmdFind.Enabled = true;
            cmdFind.Visible = true;

        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
            PanelWarning.Visible = false;
            lblError.Visible = false;
            string _actionID = txtOwner.Text;
            int actionID = int.Parse(GetNumbers(_actionID));
            txtOwner.Enabled = false;

            string _sql = "";

            OracleConnection _conn = new OracleConnection(_connStr);

            try
            {

                int c_astatusID;
                int c_istatusID;

                _sql = @"select st.status as astatus,sa.title as atitle, si.title as ititle, ist.status as istatus,si.status_id as istatus_id, sa.status_id as astatus_id
                            from siims_action sa 
                            join siims_issue si on si.issue_id=sa.issue_id
                            join siims_status ist  on ist.status_id = si.status_id 
                            join siims_status st  on st.status_id = sa.status_id 
                            where sa.action_id = :aID";
                _conn.Open();

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;

                _cmd.Parameters.Add(":aID", OracleDbType.Int32).Value = actionID;
                //   _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _reviewID;

                OracleDataReader _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (_reader.HasRows)
                {
                    _reader.Read();
                    lblError.Visible = false;
                    PanelOK.Visible = true;
                    Panel1.Visible = true;
                    cmdFind.Enabled = false;
                    cmdFind.Visible = false;

                    lblAStatus.Text = _reader.GetString(0);
                    lblATitle.Text = _reader.GetString(1);
                    lblITitle.Text = _reader.GetString(2);
                    lblIStatus.Text = _reader.GetString(3);
                   
                    c_istatusID = _reader.GetInt32(4);
                    c_astatusID = _reader.GetInt32(5);

                    _conn.Close();
                    HiddenAID.Value = actionID.ToString();
                    if (c_istatusID == 13)
                    {
                        lblError.Text = "Note: the associated issue is deleted. You have to change the issue status first.";
                        lblError.Visible = true;
                        txtOwner.Enabled = true;
                        btnChange.Visible = false;
                        return;
                    }
                    bindStatus(c_astatusID,c_istatusID);
                    bool isFound=bindPendingRequest(actionID);
                    if(isFound)
                    {
                        PanelWarning.Visible = true;
                        btnChange.OnClientClick += "javascript:return confirm('There are pending requests for this action. Are you sure you want to clear the requests and change the action status?');";
                    }
                 
                } else
                {
                    lblError.Text = "Error: this action does not exist.";
                    lblError.Visible = true;
                    txtOwner.Enabled = true;
                }




            } catch (Exception ex)
            {
                Log.Error("cmdFind_Click", ex);
            }

        }


        private bool bindPendingRequest(int aid)
        {
            bool isFound = false;
            try
            {
                OracleConnection _conn = new OracleConnection(_connStr);
                string _sql1 = @" select ch.action_id, ch.sub_status_id, s.status, ch.created_by, to_char(ch.created_on,'MM/DD/YYYY') as request_on ,p.name
                                    from siims_action_change ch
                                    join siims_status s on s.status_id = ch.sub_status_id and s.is_active = 'Y'
                                     join persons.person p on p.key = ch.created_by
                                    where ch.action_id =:AID and ch.is_active = 'Y' ";

                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand(_sql1, _conn);
                _cmd1.BindByName = true;

                _cmd1.Parameters.Add(":AID", OracleDbType.Int32).Value = aid;
                //   _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _reviewID;

                OracleDataReader _reader1 = _cmd1.ExecuteReader(CommandBehavior.CloseConnection);
               
                if(_reader1.HasRows)
                {
                    isFound = true;
                }
                gvwRequests.DataSource = _reader1;
                gvwRequests.DataBind();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Log.Error("bindPendingRequest", ex);
            }

            return isFound;
        }

        private void bindStatus(int a_status_id, int i_status_id)
        {

            try
            {
                OracleConnection _conn = new OracleConnection(_connStr);
                string _sql1;
                if (i_status_id == 12) {
                    _sql1 = @" select status_id, status from siims_status where object_id = 2 and parent_id is null and is_active = 'Y' 
                            and status_id not in (20,21,22,:statusID)  ORDER BY status_id";
                } else
                {
                    _sql1 = @" select status_id, status from siims_status where object_id = 2 and parent_id is null and is_active = 'Y' 
                            and status_id not in (20,:statusID)  ORDER BY status_id";
                }
                

                _conn.Open();

                OracleCommand _cmd1 = new OracleCommand(_sql1, _conn);
                _cmd1.BindByName = true;

                _cmd1.Parameters.Add(":statusID", OracleDbType.Int32).Value = a_status_id;
                //   _cmd.Parameters.Add(":RID", OracleDbType.Varchar2).Value = _reviewID;

                OracleDataReader _reader1 = _cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                ddlStatuslist.Items.Clear();
                if (_reader1.HasRows)
                {
                    while (_reader1.Read())
                    {
                        ListItem NewItem = new ListItem();
                        string _status = Convert.ToString(_reader1["status"]);
                        string _status_id = Convert.ToString(_reader1["status_id"]);
                        NewItem.Text = _status;
                        NewItem.Value = _status_id;
                        ddlStatuslist.Items.Add(NewItem);

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("bindStatus", ex);
            }
        }

      

        protected void btnChange_Click(object sender, EventArgs e)
        {

            string _AID = HiddenAID.Value;
            try
            {
                int status_id = int.Parse(ddlStatuslist.SelectedValue);

                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;

                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandText= " APPS_ADMIN.SIIMS_ADMIN_PKG.PROC_CHANGE_ACTION_STATUS";
                    OracleParameter inLogSID = _cmd.Parameters.Add("PI_CREATED_SID", OracleDbType.Int32);
                    inLogSID.Direction = ParameterDirection.Input;
                    inLogSID.Value = _loginSID;

                    OracleParameter inStatusID = _cmd.Parameters.Add("PI_NEW_STATUS_ID", OracleDbType.Int32);
                    inStatusID.Direction = ParameterDirection.Input;
                    inStatusID.Value = status_id;

                    OracleParameter newSID = _cmd.Parameters.Add("PI_ACTION_ID", OracleDbType.Int32);
                    newSID.Direction = ParameterDirection.Input;
                    newSID.Value = int.Parse(_AID);
                    _connection.Open();
                    _cmd.ExecuteNonQuery();

                    _connection.Close();

                    lblAStatus.Text = ddlStatuslist.SelectedItem.Text;
                    bindPendingRequest(int.Parse(_AID));
                    Panel1.Visible = false;
                    cmdFind.Enabled = true;
                    cmdFind.Visible = true;
                    PanelWarning.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("btnChange_Click", ex);
                lblError.Text = "Error: " + ex.Message;
                lblError.Visible = true;

            }
          

        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}