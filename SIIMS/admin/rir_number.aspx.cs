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
    public partial class rir_number : System.Web.UI.Page
    {
        string _connStr;
        int _loginSID;

        protected static readonly ILog Log = LogManager.GetLogger("rir_number");
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;

            string _logSID = Session["LoginSID"] == null ? "" : Session["LoginSID"].ToString();
            if (string.IsNullOrEmpty(_logSID))
            {
                Response.Redirect("../permission.aspx", true);
            }

            _loginSID = int.Parse(Session["LoginSID"].ToString());

            if (Session["ISRIRADMIN"] == null || Session["ISRIRADMIN"].ToString() != "Y")
            {
                Response.Redirect("../rir/rir.aspx", true);
            }
            txtStartDate.Attributes.Add("readonly", "readonly");
            txtEndDate.Attributes.Add("readonly", "readonly");

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            string startDate  = Request.Form[txtStartDate.UniqueID];
            string  endDate = Request.Form[txtEndDate.UniqueID];
            PanelReport.Visible = true;
            calculateNumbers(startDate, endDate);
        }

        protected void calculateNumbers(string  startDate, string endDate)
        {

            OracleDataReader _reader = null;

            string _sql = "";

            try
            {
                // exclude people already in the .ist
                _sql = @"select  (select count(*) as open1 from (select distinct action_id from ( select jn.action_id,jn.status_id, min(jn_datetime) as edate 
                            from APPS_ADMIN.siims_action_jn jn
                               join siims_action act on jn.action_id=act.action_id and act.is_active='Y' 
                            join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                          and (issue.is_rir='N' or issue.is_rir is null)
                            where jn.status_id in (21,22)  and jn.is_active='Y' 
                            group by jn.action_id, jn.status_id
                            having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                    union
                         select jn.action_id, jn.status_id,min(jn_datetime) as event_date from APPS_ADMIN.siims_action_jn jn
                        join siims_action act on jn.action_id=act.action_id and act.is_active='Y' 
                        join siims_issue issue on issue.issue_id=jn.issue_id  and issue.is_active='Y'
                          and issue.is_rir='Y' 
                        where jn.status_id in (21,22)  and jn.is_active='Y' 
                        group by jn.action_id, jn.status_id
                        having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate) ) tmp1  ) ) as open1,
                           (select count(*) as close1 from (select jn.action_id, min(jn_datetime) as edate from APPS_ADMIN.siims_action_jn jn
                                    join siims_action act on jn.action_id=act.action_id and act.is_active='Y' 
                                    join siims_issue issue on issue.issue_id=jn.issue_id and issue.owner_sid=:SID and issue.is_active='Y'
                                    and (issue.is_rir='N' or issue.is_rir is null)
                                                                where jn.status_id=23  and jn.is_active='Y' 
                                                                group by jn.action_id
                                                                having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate)
                                     union select jn.action_id, min(jn_datetime) as edate from APPS_ADMIN.siims_action_jn jn
                                       join siims_action act on jn.action_id=act.action_id and act.is_active='Y' 
                                    join siims_issue issue on issue.issue_id=jn.issue_id and issue.is_active='Y' and issue.is_rir='Y' 
                                                                where jn.status_id=23  and jn.is_active='Y' 
                                                                group by jn.action_id
                                                                having trunc(min(jn_datetime)) between trunc(:SDate) and trunc(:EDate) 
                                                               )) as close1
                            from dual";

                using (OracleConnection _conn = new OracleConnection(_connStr) ) {
                    using (OracleCommand _cmd = new OracleCommand(_sql, _conn))
                    {
                        _cmd.BindByName = true;
                        _cmd.Parameters.Add("SID", OracleDbType.Int32).Value = _loginSID;
                        _cmd.Parameters.Add(":SDate", OracleDbType.Date).Value = Convert.ToDateTime(startDate);
                        _cmd.Parameters.Add(":EDate", OracleDbType.Date).Value = Convert.ToDateTime(endDate);
                        _conn.Open();
                        _reader = _cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        while (_reader.Read())
                        {
                            int intOpen1 = _reader.GetInt32(0);
                            lblOpenIssues.Text = intOpen1.ToString();
                            if (intOpen1 == 0)
                            {
                                btnType1.Visible = false;
                            } else
                            {
                                btnType1.Visible = true;
                            }
                               
                            int intOpen2 = _reader.GetInt32(1);
                            lblClosedIssues.Text = intOpen2.ToString();
                            if (intOpen2 == 0)
                            {
                                btnType2.Visible = false;
                            }
                            else
                            {
                                btnType2.Visible = true;
                            }

                        }
                    }
                  
                }

            } catch(Exception ex)
            {
                Log.Error("calculateNumbers", ex);
            }
            finally
            {
                if (_reader != null) { _reader.Close(); _reader.Dispose(); }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("rir_admin.aspx");
        }
    }
}