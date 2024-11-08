using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class rep_cy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                drwDept.SelectedValue = "-1";
                drwCode.SelectedValue = "-1";
                drwCY.SelectedValue = "-1";
                drwCQ.SelectedValue = "-1";
            }
          
        }
        //protected void drwOrg_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (drwOrg.SelectedValue != "-1")
        //    {
        //        PanelTitle.Visible = true;
        //        btnView.Visible = true;
        //    }

        //}

 
        protected void btnClear_Click(object sender, EventArgs e)
        {
            drwOrg.ClearSelection();
            drwDept.ClearSelection();
            drwCode.ClearSelection();
            drwCY.ClearSelection();
            drwCQ.ClearSelection();
            drwOrg.SelectedValue = "-1";
            drwDept.SelectedValue = "-1";
            drwCode.SelectedValue = "-1";
            drwCY.SelectedValue = "-1";
            drwCQ.SelectedValue = "-1";
            txtKeyword.Text = "";
        }
    }
}