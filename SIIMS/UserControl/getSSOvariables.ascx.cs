using Oracle.ManagedDataAccess.Client;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.UserControl
{
    public partial class WebUserControl1 : System.Web.UI.UserControl
    {

        int _userSID;
        string _connStr;
        private string? _AttSid;
        private string? _attLoginName;
        private string? _attEmail;  
        protected static readonly ILog Log = LogManager.GetLogger("_default");


        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

        }

        public string? AttSid
        {
            get { return _AttSid; }
            set { _AttSid = value; }
        }
        public string? AttLoginName
        {
            get { return _attLoginName; }
            set { _attLoginName = value; }
        }
        public string? AttEmail
        {
            get { return _attEmail; }
            set { _attEmail = value; }
        }

        public void getSSOSID1()
        {
            string _attSid = (_AttSid != null) ? _AttSid.ToString() : string.Empty ;
            TextBox1.Text = _attSid;

            if (_attSid != null)
            {
                if (_attSid.IndexOf(";") != -1)
                {
                    _attSid = _attSid.Substring(0, _attSid.IndexOf(";"));
                    Session["LoginSID"] = _attSid;
                }
                else
                {
                    Session["LoginSID"] = _attSid;
                }
            }
            else
            {
                Session["LoginSID"] = "";
            }


            string loginName = (_attLoginName != null) ? _attLoginName.ToString() : string.Empty;
            TextBox2.Text = loginName;
            if (loginName != null)
            {
                if (loginName.IndexOf(";") != -1)
                {
                    loginName = loginName.Substring(0, loginName.IndexOf(";"));
                    Session["LoginName"] = loginName;
                }
                else
                {
                    Session["LoginName"] = loginName;

                }
            }
            else
            {
                Session["LoginName"] = "None";
            }

            string loginEmail = (_attEmail != null) ? _attEmail.ToString() : string.Empty;
            TextBox3.Text = loginEmail;
            if (loginEmail != null)
            {
                if (loginEmail.IndexOf(";") != -1)
                {
                    loginEmail = loginEmail.Substring(0, loginEmail.IndexOf(";"));
                    Session["LoginEmail"] = loginEmail;
                }
                else
                {
                    Session["LoginEmail"] = loginEmail;
                }
            }
            else
            {
                Session["LoginEmail"] = "None";
            }

            //Session["IS_OWNER"] = "1";
            getIsOwner(_attSid);
        }


        private void getIsOwner(string oSID)
        {
            //string ownerSID = Session["LoginSID"].ToString();
            string ownerSID = oSID;

            string _sql = @"select count(*) as isOwner from siims_rir_reviewer where reviewer_sid=" + ownerSID + @" and is_active='Y' and is_owner='Y'";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Session["IS_OWNER"] = reader.GetInt32(0).ToString();
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Is Owner for login=" + Session["LoginName"].ToString(), ex);
            }

        }


    }
}