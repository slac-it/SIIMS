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
    public partial class action_ChOwners : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("action_ChOwners");
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
            reset();

        }

        private void reset()
        {
            txtNewOwner.Text = "";
            lblTitle.Text = "";

            Panel2.Visible = false;
            Panel1.Visible = false;

            lblOwner.Text = "";
            btnRequest.Visible = true;
            txtOwner.ReadOnly = false;
            txtNewOwner.ReadOnly = false;
            cmdFind.Visible = true;
            cmdFind.Enabled = true;
        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
            string _actionID = txtOwner.Text;
            int actionID = int.Parse(GetNumbers(_actionID));

            string _sql = "";

            OracleConnection _conn = new OracleConnection(_connStr);

            try
            {

                //int c_SID;

                _sql = @"select p.name, p.key, ac.title, st.status
                            from siims_action ac  join persons.person p on p.key=ac.owner_sid 
                             join siims_status st on st.status_id=ac.status_id
                            where ac.action_id = :aID";
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
                    Panel1.Visible = true;
                    cmdFind.Enabled = false;
                    cmdFind.Visible = false;

                    lblOwner.Text = _reader.GetString(0) + " (" + _reader.GetInt32(1) + ")";
                    //c_SID = _reader.GetInt32(1);
                    lblTitle.Text= _reader.GetString(2);
                    lblStatus.Text = _reader.GetString(3);
                    _conn.Close();
                    HiddenAID.Value = actionID.ToString();
                    txtOwner.ReadOnly = true;
                }
                else
                {
                    lblError.Text = "Error: this action either does not exist or the action is still in draft and has no owner yet!";
                    lblError.Visible = true;
                }




            }
            catch (Exception ex)
            {
                Log.Error("cmdFind_Click", ex);
            }

        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {

            string _AID = HiddenAID.Value;
            try
            {
                int owner_SID = int.Parse(txtNewOwner.Text);

                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandText = " select name from persons.person where key=:SID and gonet='ACTIVE' ";
                    _cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = owner_SID;
                    OracleDataReader _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    if (_reader.HasRows)
                    {
                        _reader.Read();
                        lblError.Visible = false;
                        lblError2.Visible = false;
                        txtNewOwner.ReadOnly = true;
                        lblNewOwner.Text = _reader.GetString(0);
                        Panel2.Visible = true;
                        btnRequest.Visible = false;
                    }
                    else
                    {
                        lblError2.Text = "Error: we can not find a person with this SLAC ID!";
                        lblError2.Visible = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("btnRequest_Click", ex);
                lblError2.Text = "Error: " + ex.Message;
                lblError2.Visible = true;

            }


        }

        protected void btnChange_Click(object sender, EventArgs e)
        {

            string _AID = HiddenAID.Value;
            try
            {
                int owner_SID = int.Parse(txtNewOwner.Text);

                using (OracleConnection _connection = new OracleConnection())
                {
                    _connection.ConnectionString = _connStr;
                    _connection.Open();
                    OracleCommand _cmd = _connection.CreateCommand();
                    _cmd.BindByName = true;
                    _cmd.CommandText = " UPDATE siims_action SET owner_SID=:ownerSID, LAST_ON=sysdate, LAST_BY=:loginSID where action_id=:AID";
                    _cmd.Parameters.Add(":ownerSID", OracleDbType.Int32).Value = owner_SID;
                    _cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = _loginSID;
                    _cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = int.Parse(_AID);

                    _cmd.ExecuteNonQuery();

                    _connection.Close();

                    reset();
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