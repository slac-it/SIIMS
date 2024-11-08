using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_srcIssue_Listing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string stitle = Session["STitle"].ToString();
            string stype = Request["stype"].ToString();
            lnkDownload.NavigateUrl = "rep_srcIssue_export.aspx?stype=" + stype;
        }
    }
}