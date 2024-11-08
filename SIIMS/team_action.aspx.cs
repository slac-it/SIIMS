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

namespace SIIMS
{
    public partial class team_action : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("team_action");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;


            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("~/permission.aspx", true);
            }

            _loginSID = int.Parse(_logSID);
        }

        #region GridView1 Event Handlers
        //This event occurs for each row
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            string strSort = string.Empty;

            // Make sure we aren't in header/footer rows
            if (row.DataItem == null)
            {
                return;
            }

        }

        //This event occurs for any operation on the row of the grid
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        #endregion
    }
}