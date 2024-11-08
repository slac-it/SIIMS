using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_levelIssue_Listing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string stype = Request["level"].ToString();
            lnkDownload.NavigateUrl = "rep_levelIssue_export.aspx?level=" + stype;
        }
    }
}