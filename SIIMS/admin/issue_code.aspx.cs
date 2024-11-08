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
    public partial class issue_code : System.Web.UI.Page
    {
        protected static readonly ILog Log = LogManager.GetLogger("issue_code");

        string _connStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            if (Session["ISADMIN"] == null || Session["ISADMIN"].ToString() != "1")
            {
                Response.Redirect("../permission.aspx");
            }
        }

      

        protected void gvw1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvw1.PageIndex = e.NewPageIndex;
            gvw1.DataBind();

            //bindGrid(); 
            //SubmitAppraisalGrid.PageIndex = e.NewPageIndex;
            //SubmitAppraisalGrid.DataBind();
        }
     
    }
}