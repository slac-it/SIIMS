using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS.RIR
{
    public partial class migration_list : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string FormatAction(string _iid, string pdf_fileName)
        {
            if (string.IsNullOrEmpty(pdf_fileName))
            {
                return "<a href='migration_upload.aspx?iid=" + _iid + "'>Upload PDF</a>";

            }
            else
            {
                string msg = "<a href='migration_download.aspx?iid=" + _iid + "'>" + pdf_fileName + "</a> <br />";
                msg += "<a href='migration_delete.aspx?iid=" + _iid + "'>Delete</a> <br />";
                return msg;
            }
        }
    }
}