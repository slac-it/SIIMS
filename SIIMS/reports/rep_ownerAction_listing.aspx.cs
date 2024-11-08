using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_ownerAction_listing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string _sid = Request["sid"].ToString();
            lnkDownload.NavigateUrl = "rep_ownerAction_export.aspx?sid=" + _sid;
        }
    }
}