using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using log4net;
using System.Configuration;
using SIIMS.App_Code;

namespace SIIMS
{
    public partial class rep_deptAction : System.Web.UI.Page
    {
        string _connStr;
        protected static readonly ILog Log = LogManager.GetLogger("rep_deptAction");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            txtOwner.Focus();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            //txtSLACID.Text = "";
            txtOwner.Text = "";
            ddlEmplist.Items.Clear();
            lblMsg.Visible = false;
            lblMsg.Text = "";
            btnView.Enabled = true;
            btnClear.Visible = false;
        }

        protected void cmdFind_Click(object sender, EventArgs e)
        {
            string errorMsg = "Please enter the first few characters to start your search";

            string lastName = txtOwner.Text.Trim();

            if (string.IsNullOrEmpty(lastName))
            {
                lblError.Text = errorMsg;
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;
            ddlEmplist.Visible = true;

            string _sql = "select distinct p.key, p.name, dept.description as dept_name from persons.person p join SID.organizations dept on p.dept_id=dept.org_id " +
                   " join sid.person sp on p.key=sp.person_id join but b on b.but_sid=p.key and b.But_ldt='win' " +
                    " where p.gonet='ACTIVE' and p.status in ('CONTRACTOR','EMP','CONSULTANT','SU') and LOWER(p.name) LIKE LOWER(:PName) || '%'  ORDER BY p.name";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":PName", OracleDbType.Varchar2).Value = lastName;

                OracleDataReader _reader = cmd.ExecuteReader();

                ddlEmplist.Items.Clear();
                if (_reader.HasRows)
                {
                    ddlEmplist.Visible = true;
                    btnView.Visible = true;
                    btnClear.Visible = true;
                    lblError.Visible = false;
                    while (_reader.Read())
                    {
                        ListItem NewItem = new ListItem();
                        string _name = Convert.ToString(_reader["name"]);
                        string _slac_id = Convert.ToString(_reader["key"]);
                        string _dept = Convert.ToString(_reader["dept_name"]);
                        //NewItem.Text = _name + " : " + _slac_id + " @ " + _dept;
                        NewItem.Text = _name;
                        NewItem.Value = _slac_id;
                        ddlEmplist.Items.Add(NewItem);

                    }

                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Name is not in directory.";
                    ddlEmplist.Visible = false;
                    btnView.Visible = false;
                }

                _reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("cmdFind_Click", ex);
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            btnView.Enabled = false;
            //btnBack.Visible = true;
            string _emp_name = ddlEmplist.SelectedItem.Text;
            string _emp_sid = ddlEmplist.SelectedValue;

            if (string.IsNullOrEmpty(_emp_sid))
            {
                lblError.Visible = true;
                lblError.Text = "You have to select one person from the dropdown list.";
                return;
            }

            Response.Redirect("rep_deptAction_listing.aspx?sid=" + _emp_sid);
        }
    }
}