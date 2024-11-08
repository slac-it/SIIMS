using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using log4net;
using System.Configuration;

namespace SIIMS
{
    public partial class user : System.Web.UI.MasterPage
    {
        int _userSID;
        string _connStr;

        protected static readonly ILog Log = LogManager.GetLogger("user");

        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
            string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["isUAT"];
            string adminList = System.Web.Configuration.WebConfigurationManager.AppSettings["AdminList"];

            if (_isUAT == "1")
            {
                lblWarning.Text = "This is a test site and is not intended for actual use.";
            }

            if (Session["LoginSID"] == null || Session["LoginName"] ==null)
            {
                Response.Redirect("permission.aspx", true);
            }
            else
            {
            _userSID = int.Parse(Session["LoginSID"].ToString());

            }

            if (_userSID == 0)
            {
                Response.Redirect("permission.aspx", true);
            }

            if (adminList.Contains(_userSID.ToString()))
            {
                adminLink.Visible = true;
            }
            
            litName.Text = Session["LoginName"].ToString();
            BindToDoCount();

        }

        private void BindToDoCount()
        {

            int count = 0;

            string _sql = @"select '1' as type, count(iss.issue_id) as TODO  from siims_issue iss  join persons.person p on iss.created_by = p.key 
                       join siims_status sta on sta.status_id=iss.status_id  where iss.is_active='Y' and iss.created_by=:UserID 
                          and iss.status_id=10  and (iss.is_rir is null or iss.is_rir='N')
                     union 
                    select '2' as type,  count(iss.issue_id) as TODO from siims_issue iss join persons.person p on iss.created_by = p.key 
                     join siims_status sta on sta.status_id=iss.status_id where iss.is_active='Y' and  iss.status_id=11  and iss.sub_status_id in (110, 111,115)
                     and exists (select sid from siims_manager where SID=:UserID and IS_IIP='Y'  and is_active='Y')  
                    union  
                    select  '3' as type,   count(act.action_id) as TODO  from siims_action act join siims_issue iss on iss.issue_id=act.issue_id
                     join persons.person p on act.created_by = p.key  
                    join siims_status sta on sta.status_id=act.status_id  where act.is_active='Y' and act.created_by=:UserID and act.status_id=20   
                        and (iss.is_rir is null or iss.is_rir='N')
                    union 
                    select  '4' as type,    count(act.action_id) as TODO  from siims_action act  join persons.person p on act.owner_sid = p.key 
                     join siims_status sta on sta.status_id=act.status_id where act.is_active='Y' and act.owner_sid=:UserID and act.status_id=21 and  act.sub_status_id is null 
                    union 
                    select '5' as type,  count(act.action_id) as TODO from siims_action_change ch join siims_action act on ch.action_id =act.action_id and act.status_id=22 
                    join persons.person p on p.key=ch.created_by where ch.is_active='Y' and  ch.sub_status_id in (223, 221,222,220,224)  and (ch.IS_BY_IOWNER='Y' or ch.IS_IOWNER_APPROVED='Y') 
                    and exists (select sid from siims_manager where SID=:UserID and IS_IIP='Y' and is_active='Y')
                    union 
                    select '6' as type,    count(act.action_id) as TODO from siims_action_change ch join siims_action act on ch.action_id =act.action_id  and act.status_id=22 
                     join persons.person p on p.key=ch.created_by join siims_issue issue on act.issue_id=issue.issue_id and issue.owner_sid=:UserID 
                    where ch.is_active='Y' and  ch.sub_status_id in (221,222,220,224) and ch.IS_BY_IOWNER='N' and  (ch.IS_IOWNER_APPROVED='N' or ch.IS_IOWNER_APPROVED is null)  
                    union 
                    select '8' as type,  count(iss.issue_id) as TODO from siims_issue iss  join persons.person p on iss.created_by = p.key 
                     join siims_status sta on sta.status_id=iss.status_id where iss.is_active='Y' and  iss.status_id=11 and (select count(*) from siims_action where issue_id=iss.issue_id and is_active='Y')=0 
                    and iss.owner_sid=:UserID  and trunc(sysdate) - iss.CREATED_ON > 30  
                    UNION 
                    select '9' as type, count(act.action_id)   from siims_action act  join siims_action_change ch on ch.action_id=act.action_id and ch.sub_status_id=210 and ch.is_active='Y' and ch.NEW_OWNER_SID=:UserID 
                     join persons.person p on p.key = ch.NEW_OWNER_SID join siims_status sta on sta.status_id=act.status_id where act.is_active='Y'  and act.status_id=21 and act.sub_status_id =210
                    UNION
                    select '20' as type, count(iss.issue_id)    from siims_issue iss join siims_rir_report rir on rir.issue_id=iss.issue_id and rir.rir_status='D'
                        where iss.is_active='Y' and iss.created_by=:UserID and iss.status_id=10 and  iss.IS_RIR='Y'
                    UNION
                     select '22' as type, count(iss.issue_id)    from siims_issue iss join siims_rir_report rir on rir.issue_id=iss.issue_id and rir.rir_status='R'
                        where iss.is_active='Y'  and iss.status_id=10 and  iss.IS_RIR='Y'
                         and EXISTS (SELECT * FROM SIIMS_RIR_REVIEWER
                            WHERE reviewer_sid=:UserID and is_active='Y' and is_owner='N')
                    ";
            
           
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":UserID", OracleDbType.Int32).Value = _userSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count += reader.GetInt32(1);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("BindToDoCount", ex);
            }

            litCount.Text = count.ToString();

        }

       
    }
}