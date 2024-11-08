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
    public partial class rir_condition : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        protected static readonly ILog Log = LogManager.GetLogger("rir_condition");
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
                Response.Redirect("../rir/rir.aspx", true);
            }

            if(!Page.IsPostBack)
            {
                populateCondition();
            }
           
        }

      

        private void populateCondition()
        {
            DataSet ds = new DataSet();

            string _sql = "";

            _sql = " select condition_id, condition from siims_rir_condition where is_active = 'Y' order by condition ";

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

            lv_condition.DataSource = ds;
            lv_condition.DataBind();


        }

        protected void NewCondition(Object sender, EventArgs e)
        {

            lv_condition.EditIndex = -1;
            lv_condition.InsertItemPosition = InsertItemPosition.FirstItem;
            ((LinkButton)sender).Visible = false;
            populateCondition();
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
            TextBox txtDesc = (TextBox)e.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "Insert Into SIIMS_RIR_CONDITION (CONDITION, LAST_BY, LAST_ON, IS_ACTIVE) Values(:Condition,:SID,sysdate,'Y')";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":Condition", OracleDbType.Varchar2).Value = txtDesc.Text.Trim();
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
            populateCondition();

        }

        protected void InsertList(Object sender, ListViewInsertEventArgs e)
        {
            lv_condition.InsertItemPosition = InsertItemPosition.None;
            //  lv_Accomplishments.InsertItemPosition = InsertItemPosition.FirstItem;

        }

        protected void CancelInsertMode()
        {

            lv_condition.InsertItemPosition = InsertItemPosition.None;
            populateCondition();
            lnkNew.Visible = true;
        }

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                LinkButton lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this Radiological Posting or Condition?');");

                }
                // set focus for the editing row
                TextBox txtDescEdit = e.Item.FindControl("txtDesc") as TextBox;
                if (txtDescEdit != null)
                    txtDescEdit.Focus();

            }

            if (lv_condition.InsertItemPosition != InsertItemPosition.None)
            {
                TextBox txtDesc = (TextBox)lv_condition.InsertItem.FindControl("txtDesc");
                txtDesc.Focus();
            }

        }

        // Invoked when the Edit Link is clicked
        protected void EditList(Object sender, ListViewEditEventArgs e)
        {
            // Set the Listview to Editmode
            lv_condition.EditIndex = e.NewEditIndex;

            // Bind the view 
            populateCondition();
        }

        // Invoked when the Delete Link is clicked
        protected void DeleteList(Object sender, ListViewDeleteEventArgs e)
        {

            //// Get the current item being deleted
            //ListViewItem myItem = lv_condition.Items[e.ItemIndex];

            //// get the author id which is being deleted
            //Label lblNoteID = (Label)myItem.FindControl("lblConditionID");

            string smt_id = lv_condition.DataKeys[e.ItemIndex].Value.ToString();
            //Log.Debug("Using DataKey I got: " + smt_id + "   Using lable is: " + lblNoteID.Text);

            DeleteCondition(smt_id);
            populateCondition();
        }

        protected void CancelList(Object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingEdit)
            {
                lv_condition.EditIndex = -1;
                populateCondition();
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
                string _sql = "UPDATE SIIMS_RIR_Condition SET IS_ACTIVE='N', LAST_ON=SysDate, LAST_BY=:SID WHERE condition_ID=:Note_ID";

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
            ListViewItem myItem = lv_condition.Items[lv_condition.EditIndex];

            // get the author id which is being edited
            //Label lblNoteID = (Label)myItem.FindControl("lblConditionID");

            string smt_id = lv_condition.DataKeys[e.ItemIndex].Value.ToString();
            //Log.Debug("Using DataKey in UpdateList I got: " + smt_id + "   Using lable is: " + lblNoteID.Text);

            // Get the new values
            TextBox txtDesc = (TextBox)myItem.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "UPDATE  SIIMS_RIR_CONDITION SET condition= :Desc2, LAST_BY=:SID, LAST_ON=sysdate WHERE Condition_ID=:GID";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":Desc2", OracleDbType.Varchar2).Value = txtDesc.Text.Trim();
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
            lv_condition.EditIndex = -1;

            // bind the listview
            populateCondition();
        }
    }

}