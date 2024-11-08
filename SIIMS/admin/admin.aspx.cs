using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.admin
{
    public partial class admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string adminList = System.Web.Configuration.WebConfigurationManager.AppSettings["AdminList"];


            if (Session["LoginSID"]==null)
            {
                Response.Redirect("../default.aspx", true);
            }

            int _userSID = int.Parse(Session["LoginSID"].ToString());

            if (adminList.Contains(_userSID.ToString()))
            {
                Session["ISADMIN"] = "1";
            }
            else
            {
                Session["ISADMIN"] = "0";
                Response.Redirect("../default.aspx", true);
            }
           
        }
    }
}