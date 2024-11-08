using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class test_sso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string headers = String.Empty;
            foreach (var key in Request.Headers.AllKeys)
                headers += key + "=" + Request.Headers[key] + Environment.NewLine;

            //Response.Write("HTTP Header: <br />");
            //Response.Write(headers);

            Response.Write("<br /><br />HTTP Server Variables: <br />");
            foreach (string var in Request.ServerVariables)
            {
                Response.Write("Header is:" + var + "<br>");
                Response.Write("Value: " + Request[var] + "<br>");
            }
            Response.Write("<br /><br /> Page content:");

            lblSSO.Text = "NO";
            if(Request["REMOTE_USER"] != null)
            {
                lblSSO.Text = "User:" + Request["REMOTE_USER"].ToString();
            }
        }
    }
}