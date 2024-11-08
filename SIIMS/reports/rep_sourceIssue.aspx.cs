using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class rep_sourceIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string title = drwTitle.SelectedValue;
            string stype = drwSType.SelectedValue;
            Session["STitle"] = title;
            Response.Redirect("rep_srcIssue_listing.aspx?stype=" + stype);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            drwSType.ClearSelection();
            drwTitle.ClearSelection();
            drwSourceFY.ClearSelection();
            drwSourceQtr.ClearSelection();
        }

        protected void drwSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drwSType.SelectedValue != "-1")
            {
                PanelTitle.Visible = true;
                btnView.Visible = true;
            }
        }

    }
}