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
    public partial class rir_group : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        private String strCurrentOrgID = "";
        protected static readonly ILog Log = LogManager.GetLogger("rir_group");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (Session["ISRIRADMIN"] == null || Session["ISRIRADMIN"].ToString() != "Y")
            {
                Response.Redirect("../default.aspx", true);
            }

            if (!Page.IsPostBack)
            {
                populateGroup();
            }
        }

        private void populateGroup()
        {
            DataSet ds = new DataSet();

            string _sql = "";

            _sql = @" select dept.DEPT_ID, org.org_id, dept.dept, org.name as org from SIIMS_DEPT dept
                         join SIIMS_ORG org on dept.org_id = org.org_id
                         where dept.is_active = 'Y' order by org.name, dept";

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                using (OracleCommand sqlCmd = new OracleCommand())
                {
                    sqlCmd.BindByName = true;
                    sqlCmd.Connection = _conn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = _sql;
                    using (OracleDataAdapter sqlAdp = new OracleDataAdapter(sqlCmd))
                    {
                        sqlAdp.Fill(ds);
                    }
                }
            }

            lv_group.DataSource = ds;
            lv_group.DataBind();


        }

        protected void NewGroup(Object sender, EventArgs e)
        {

            lv_group.EditIndex = -1;
            lv_group.InsertItemPosition = InsertItemPosition.FirstItem;
            ((LinkButton)sender).Visible = false;
            populateGroup();
        }


        protected void CommandList(Object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "SAVE")
            {
                SaveList(e.Item);
            }

        }

        protected void SaveList(ListViewItem e)
        {

            // Get the new values
            DropDownList orgList = (DropDownList)e.FindControl("drwOrg");


            TextBox txtDesc = (TextBox)e.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "Insert Into SIIMS_DEPT (ORG_ID, DEPT, LAST_BY, LAST_ON, IS_ACTIVE) Values(:ORG_ID, :code,:SID,sysdate,'Y')";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":ORG_ID", OracleDbType.Int32).Value = int.Parse(orgList.SelectedValue);
                _cmd1.Parameters.Add(":code", OracleDbType.Varchar2).Value = txtDesc.Text.Trim();
                _cmd1.Parameters.Add(":SID", OracleDbType.Int32).Value = _loginSID;
                _cmd1.ExecuteNonQuery();
                _cmd1.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error("SaveList", ex);
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            // Hide the text boxes
            CancelInsertMode();

            // bind the listview
            populateGroup();

        }

       

        protected void InsertList(Object sender, ListViewInsertEventArgs e)
        {
            lv_group.InsertItemPosition = InsertItemPosition.None;
            //  lv_Accomplishments.InsertItemPosition = InsertItemPosition.FirstItem;

        }

        protected void CancelInsertMode()
        {

            lv_group.InsertItemPosition = InsertItemPosition.None;
            populateGroup();
            lnkNew.Visible = true;
        }

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                LinkButton lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this group?');");

                }
                // set focus for the editing row
                TextBox txtDescEdit = e.Item.FindControl("txtDesc") as TextBox;
                if (txtDescEdit != null)
                    txtDescEdit.Focus();

            }


            // Get a handle to the ddl_status DropDownList control
            DropDownList ddlOrg = (DropDownList)e.Item.FindControl("drwOrg");
            // Make sure we have the handle !
            if (ddlOrg != null)
            {
                ddlOrg.SelectedIndex = ddlOrg.Items.IndexOf(ddlOrg.Items.FindByValue(strCurrentOrgID));
            }

            if (lv_group.InsertItemPosition != InsertItemPosition.None)
            {
                TextBox txtDesc = (TextBox)lv_group.InsertItem.FindControl("txtDesc");
                txtDesc.Focus();
            }

        }

        // Invoked when the Edit Link is clicked
        protected void EditList(Object sender, ListViewEditEventArgs e)
        {
            // Set the Listview to Editmode
            lv_group.EditIndex = e.NewEditIndex;

            Label lblTemp = (Label)lv_group.Items[e.NewEditIndex].FindControl("lblOrgID");
            strCurrentOrgID = lblTemp.Text;

            // Bind the view 
            populateGroup();
        }

        // Invoked when the Delete Link is clicked
        protected void DeleteList(Object sender, ListViewDeleteEventArgs e)
        {

            //// Get the current item being deleted
            //ListViewItem myItem = lv_condition.Items[e.ItemIndex];

            //// get the author id which is being deleted
            //Label lblNoteID = (Label)myItem.FindControl("lblConditionID");

            string smt_id = lv_group.DataKeys[e.ItemIndex].Value.ToString();
            //Log.Debug("Using DataKey I got: " + smt_id + "   Using lable is: " + lblNoteID.Text);

            DeleteCondition(smt_id);
            populateGroup();
        }

        protected void CancelList(Object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingEdit)
            {
                lv_group.EditIndex = -1;
                populateGroup();
            }
            else if (e.CancelMode == ListViewCancelMode.CancelingInsert)
            {
                CancelInsertMode();
            }
        }

        private void DeleteCondition(string noteID)
        {
            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                string _sql = "UPDATE SIIMS_DEPT SET IS_ACTIVE='N', LAST_ON=SysDate, LAST_BY=:SID WHERE DEPT_ID=:Note_ID";

                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.BindByName = true;
                _cmd.Parameters.Add(":SID", OracleDbType.Varchar2).Value = _loginSID;
                _cmd.Parameters.Add(":Note_ID", OracleDbType.Varchar2).Value = noteID;

                try
                {
                    _conn.Open();
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.Error("DeleteCondition", ex);
                }

                finally
                {
                    _cmd.Dispose();
                    _conn.Close();

                }


            }

        }

        protected void UpdateList(Object sender, ListViewUpdateEventArgs e)
        {
            // Get the current item being edited
            ListViewItem myItem = lv_group.Items[lv_group.EditIndex];

            // get the author id which is being edited
            //Label lblNoteID = (Label)myItem.FindControl("lblConditionID");

            string smt_id = lv_group.DataKeys[e.ItemIndex].Value.ToString();
            //Log.Debug("Using DataKey in UpdateList I got: " + smt_id + "   Using lable is: " + lblNoteID.Text);

            // Get the new values
            DropDownList orgList = (DropDownList)myItem.FindControl("drwOrg");

            TextBox txtDesc = (TextBox)myItem.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "UPDATE  SIIMS_DEPT SET ORG_ID=:Org_ID, DEPT=:CODE,  LAST_BY=:SID, LAST_ON=sysdate WHERE DEPT_ID=:GID";
                _cmd1.CommandType = CommandType.Text;
                _cmd1.Parameters.Add(":Org_ID", OracleDbType.Int32).Value = int.Parse(orgList.SelectedValue);
                _cmd1.Parameters.Add(":CODE", OracleDbType.Varchar2).Value = txtDesc.Text.Trim();
                _cmd1.Parameters.Add(":GID", OracleDbType.Int32).Value = smt_id;
                _cmd1.Parameters.Add(":SID", OracleDbType.Int32).Value = _loginSID;

                _cmd1.ExecuteNonQuery();
                _cmd1.Dispose();
                _conn.Close();

            }
            catch (Exception ex)
            {
                Log.Error("UpdateList", ex);
            }
            finally
            {
                if (_conn != null) { _conn.Close(); }
            }

            // get out of the edit mode
            lv_group.EditIndex = -1;

            // bind the listview
            populateGroup();
        }
    }
}