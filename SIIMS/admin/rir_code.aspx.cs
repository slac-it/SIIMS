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
    public partial class rir_code : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;
        protected static readonly ILog Log = LogManager.GetLogger("rir_code");
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

            if (!Page.IsPostBack)
            {
                populateCode();
            }
        }



        private void populateCode()
        {
            DataSet ds = new DataSet();

            string _sql = "";

            _sql = " select RIRCODE_ID, category,code from SIIMS_RIR_CODE where is_active = 'Y' order by category ";

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
            populateCode();
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
            TextBox txtCat = (TextBox)e.FindControl("txtCat");

            if(CheckCategory(txtCat.Text.Trim(), 0))
            {
                return;
            }
            TextBox txtDesc = (TextBox)e.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "Insert Into SIIMS_RIR_CODE (Category, Code, LAST_BY, LAST_ON, IS_ACTIVE) Values(:Cat, :code,:SID,sysdate,'Y')";
                _cmd1.CommandType = CommandType.Text;

                _cmd1.Parameters.Add(":Cat", OracleDbType.Varchar2).Value = txtCat.Text.Trim();
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
            populateCode();

        }

        private bool CheckCategory(string categ, int code_id)
        {
            bool isError = false;

            string _sql;

            if (code_id == 0)
            {
                _sql = @"select count(*) from SIIMS_RIR_CODE where is_active='Y' and Category=:Cat ";
            } else
            {
                _sql = @"select count(*) from SIIMS_RIR_CODE where is_active='Y' and Category=:Cat and RIRCODE_ID  <> :CID ";
            }

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":Cat", OracleDbType.Varchar2).Value = categ;
                if(code_id > 0)
                {
                    cmd.Parameters.Add(":CID", OracleDbType.Int32).Value = code_id;
                }
               

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int _isAdmin = reader.GetInt32(0);
                    if (_isAdmin > 0) isError = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("CheckCategory", ex);
            }
            if(isError)
            {
                CustomValidator val = new CustomValidator();
                val.ErrorMessage = "Category:" + categ + " existed already. It must be unique.";
                val.IsValid = false;
                this.Page.Validators.Add(val);
            }

            return isError;
        }
     

        protected void InsertList(Object sender, ListViewInsertEventArgs e)
        {
            lv_condition.InsertItemPosition = InsertItemPosition.None;
            //  lv_Accomplishments.InsertItemPosition = InsertItemPosition.FirstItem;

        }

        protected void CancelInsertMode()
        {

            lv_condition.InsertItemPosition = InsertItemPosition.None;
            populateCode();
            lnkNew.Visible = true;
        }

        protected void DataBoundList(Object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                LinkButton lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this Tracking/Trending Code?');");

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
            populateCode();
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
            populateCode();
        }

        protected void CancelList(Object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingEdit)
            {
                lv_condition.EditIndex = -1;
                populateCode();
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
                string _sql = "UPDATE SIIMS_RIR_CODE SET IS_ACTIVE='N', LAST_ON=SysDate, LAST_BY=:SID WHERE RIRCODE_ID=:Note_ID";

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
            //Log.Debug("Using DataKey in UpdateList I got: " + smt_id);
            // Get the new values
            Literal txtCat = (Literal)myItem.FindControl("ltCat");

            //Log.Debug("Got  Category: " + txtCat.Text);


            if (CheckCategory(txtCat.Text.Trim(), int.Parse(smt_id)))
            {
                return;
            }

            TextBox txtDesc = (TextBox)myItem.FindControl("txtDesc");

            OracleConnection _conn = null;
            try
            {
                _conn = new OracleConnection(_connStr);
                _conn.Open();
                OracleCommand _cmd1 = new OracleCommand("", _conn);
                _cmd1.BindByName = true;
                _cmd1.CommandText = "UPDATE  SIIMS_RIR_CODE SET CATEGORY=:CAT, CODE=:CODE,  LAST_BY=:SID, LAST_ON=sysdate WHERE RIRCODE_ID=:GID";
                _cmd1.CommandType = CommandType.Text;
                _cmd1.Parameters.Add(":CAT", OracleDbType.Varchar2).Value = txtCat.Text.Trim();
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
            lv_condition.EditIndex = -1;

            // bind the listview
            populateCode();
        }
    }
}