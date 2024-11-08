using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_orgIssue_Listing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string _orgID = Request["org_id"].ToString();

            lnkDownload.NavigateUrl = "rep_orgIssue_export.aspx?org_id=" + _orgID;
        }
    }
}