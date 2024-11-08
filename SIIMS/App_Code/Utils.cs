using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace SIIMS.App_Code
{

   

    public class Helpers
    {

        protected static readonly ILog Log = LogManager.GetLogger("Helpers");

        // get 
        string _connStr;

        public Helpers()
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
        }


        public bool IsLabManager(int loginSID)
        {
            bool isManager = false;

            string _sql = "select sid from siims_manager where sid=:loginSID and is_active='Y' ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    isManager = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("isLabManager", ex);
            }

            return isManager;
        }

        public bool IsIIP(int loginSID)
        {
            bool is_IIP = false;

            string _sql = "select sid from siims_manager where sid=:loginSID and is_iip='Y' and is_active='Y' ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    is_IIP = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsLabIIP", ex);
            }

            return is_IIP;
        }

        public bool IsSMTLeader(int loginSID)
        {
            bool Is_SMTLeade = false;

            string _sql = "select org_id from siims_org where IS_ACTIVE='Y' and (poc_sid=:loginSID or manager_sid=:loginSID ) " +
                  " or exists (select org_id from SIIMS_SMT_DELEGATE where IS_ACTIVE='Y' and ORG_ID=siims_org.org_id and sid=:loginSID)";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Is_SMTLeade = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsSMTLeader", ex);
            }

            return Is_SMTLeade;
        }

        public int Num_OpenIssues(int loginSID)
        {
            int num_issues = 0;

            string _sql = "select count(mn.key) as num from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path " +
                        " FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP'and level>1  " +
            " START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID " +
                        " join siims_issue issue on issue.owner_sid=mn.key and issue.IS_ACTIVE='Y' and issue.status_id=11" ;

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    num_issues = reader.GetInt32(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Num_OpenIssues", ex);
            }

            return num_issues;
        }

        public int Num_OpenActions(int loginSID)
        {
            int num_actions = 0;

            string _sql = "select count(mn.key) as num from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path " +
                        " FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP' and level>1  " +
            " START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID " +
                        "  join siims_action act on act.owner_sid=mn.key and act.IS_ACTIVE='Y' and act.status_id=22";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    num_actions = reader.GetInt32(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Num_OpenActions", ex);
            }

            return num_actions;
        }

        public int Num_Reports(int loginSID)
        {
            int num_Reports = 0;

            string _sql = "select count(mn.key) as num from (SELECT emp.key, emp.name, emp.supervisor_id as mgr_SID, dept.description as deptName, level, SYS_CONNECT_BY_PATH(initcap(emp.lname), '/') as Path " +
                        " FROM persons.person emp join SID.organizations dept on emp.dept_id=dept.org_id WHERE emp.gonet = 'ACTIVE' and emp.status='EMP' and level>1  " +
            " START WITH emp.key = :SID CONNECT BY PRIOR emp.key = emp.supervisor_id) mn  join persons.person sup on sup.key=mn.mgr_SID ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    num_Reports = reader.GetInt32(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Num_Reports", ex);
            }

            return num_Reports;
        }

        public bool IsPOC(int loginSID, int _Issue_ID)
        {
            bool is_POC = false;

            string _sql = "select org.org_id from siims_org org join  SIIMS_ISSUE issue on issue.org_id=org.org_id and issue.issue_id=:IID " +
                        " where org.is_active='Y' and (org.MANAGER_SID=:loginSID or POC_SID=:loginSID) union  " +
                        " select del.org_id as poc_sid from siims_smt_delegate del join  SIIMS_ISSUE issue on issue.org_id=del.org_id " +
                        "  and issue.issue_id=:IID where del.sid=:loginSID and del.IS_ACTIVE='Y'  ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = _Issue_ID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    is_POC = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsPOC", ex);
            }

            return is_POC;
        }


        public bool IsPOCOfAction(int loginSID, int _Action_ID)
        {
            bool is_POC = false;

            string _sql = "select org.org_id from SIIMS_ACTION act JOIN SIIMS_ISSUE issue  on act.issue_id=issue.ISSUE_ID  " +
                        " join siims_org org on issue.org_id=org.ORG_ID and org.is_active='Y' and (org.MANAGER_SID=:loginSID or POC_SID=:loginSID) " +
                        "  where act.action_id=:AID union  " +
                        " select del.org_id from SIIMS_ACTION act JOIN SIIMS_ISSUE issue  on act.issue_id=issue.ISSUE_ID  " +
                        " join siims_smt_delegate del on issue.org_id=del.ORG_ID and del.is_active='Y' and del.sid=:loginSID  where act.action_id=:AID ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = _Action_ID;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    is_POC = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsPOCOfAction", ex);
            }

            return is_POC;
        }


        public bool IsActionOwner(int loginSID, int action_id)
        {
            bool Is_ActionOwner = false;

            string _sql = "select owner_sid from siims_action where owner_sid=:loginSID and action_id=:AID";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = action_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Is_ActionOwner = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsActionOwner", ex);
            }

            return Is_ActionOwner;
        }

        public bool IsIOwnerOfAction(int loginSID, int action_id)
        {
            bool Is_IOwner = false;

            string _sql = "select issue.owner_sid from siims_issue issue join siims_action action on action.issue_id=issue.issue_id and action.is_active='Y' where issue.owner_sid=:loginSID and action.action_id=:AID";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = action_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Is_IOwner = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsIOwnerOfAction", ex);
            }

            return Is_IOwner;
        }


        public bool IsIOwnerOrCreatorOfAction(int loginSID, int action_id)
        {
            bool Is_IOwner = false;

            string _sql = @"select issue.owner_sid from siims_issue issue join siims_action action 
                            on action.issue_id = issue.issue_id and action.is_active = 'Y'
                            where(issue.owner_sid =:loginSID or issue.created_by =:loginSID) and action.action_id =:AID";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":AID", OracleDbType.Int32).Value = action_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Is_IOwner = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsIOwnerOfAction", ex);
            }

            return Is_IOwner;
        }

        public bool IsIOwner(int loginSID, int issue_id)
        {
            bool Is_IOwner = false;

            string _sql = "select owner_sid from siims_issue where owner_sid=:loginSID and issue_id=:IID ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":loginSID", OracleDbType.Int32).Value = loginSID;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = issue_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Is_IOwner = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsIOwner", ex);
            }

            return Is_IOwner;
        }

        public bool IsP1Closue(int issue_id)
        {
            bool is_P1Closure = false;

            string _sql = "select sub_status_id from siims_issue where  issue_id=:IID and sub_status_id=115 and sig_level='P1' ";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":IID", OracleDbType.Int32).Value = issue_id;

                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    is_P1Closure = true;
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("IsP1Closue", ex);
            }

            return is_P1Closure;
        }


        public bool IsUpperManager(int mgr_sid, int emp_sid)
        {
            bool isUpper = false;

            try
            {
                string _sql = "select mn.lv from (select level as lv, supervisor_id from persons.person connect by nocycle prior supervisor_id = key start with key = :SID) mn where mn.supervisor_id=:Login";
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;

                cmd.Parameters.Add(":SID", OracleDbType.Int32).Value = emp_sid;
                cmd.Parameters.Add(":Login", OracleDbType.Int32).Value = mgr_sid;
                OracleDataReader _reader = cmd.ExecuteReader();

                if (_reader.HasRows)
                {
                    isUpper = true;
                }

                _reader.Close();
                con.Close();

            }
            catch (Exception ex)
            {
                Log.Error("IsUpperManager", ex);
            }
           
            return isUpper;
        }

    }  // end of class def


    public class FileUtil
    {
        protected static readonly ILog Log = LogManager.GetLogger("FileUtil");

        // get 
        string _connStr;

        public FileUtil()
        {
            _connStr = ConfigurationManager.ConnectionStrings["SLAC_WEB"].ConnectionString;
        }

        public byte[] GetByteArrayFromFileField(HttpPostedFile filMyFile)
        {
            // Returns a byte array from the passed 
            // file field controls file
            int intFileLength = 0;
            byte[] bytData = null;
            System.IO.Stream objStream = null;
            if (FileFieldSelected(filMyFile))
            {
                intFileLength = filMyFile.ContentLength;
                bytData = new byte[intFileLength];
                objStream = filMyFile.InputStream;
                objStream.Read(bytData, 0, intFileLength);
                return bytData;
            }
            else
                return null;
        }



        public bool FileFieldSelected(HttpPostedFile filMyFile)
        {
            //Returns true if the passed has the posted file
            if ((filMyFile == null) || (filMyFile.ContentLength == 0))
            {
                return false;
            }
            else
                return true;


        }

        public bool downLoadAttachment(int _keyID, int tableType)
        {
            bool isError = false;
            string fileName = "";
            int fileSize = 0;
            string content_type = "";
            byte[] byteArray = null;

            using (OracleConnection _conn = new OracleConnection(_connStr))
            {
                string _sql = "";
                if (tableType == 1)
                { // this is for issue attachment
                    _sql = " select  FILE_NAME,FILE_SIZE ,CONTENT_TYPE,FILE_DATA from siims_issue_att WHERE ISATT_ID=:ATTID ";
                }
                else if (tableType == 2)
                {
                    // this is for action attachment
                    _sql = " select  FILE_NAME,FILE_SIZE ,CONTENT_TYPE,FILE_DATA from siims_action_att WHERE ACATT_ID=:ATTID ";
                }


                OracleCommand _cmd = new OracleCommand(_sql, _conn);
                _cmd.Parameters.Add(":ATTID", OracleDbType.Int32).Value = _keyID;

                try
                {
                    _conn.Open();
                    using (OracleDataReader dataReader = _cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                fileName = (string)dataReader["FILE_NAME"];
                                fileSize = Convert.ToInt32(dataReader["FILE_SIZE"]);
                                content_type = (string)dataReader["CONTENT_TYPE"];
                                byteArray = (Byte[])dataReader["FILE_DATA"];

                            }
                        }
                        else
                        {
                            isError = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.Error("downLoadAttachment", ex);
                }
                finally
                {
                    _cmd.Dispose();
                }
            }


            if (byteArray != null && !string.IsNullOrEmpty(fileName))
            {
                DeliverFile(byteArray, content_type, fileSize, fileName);
            }
            else
            {
                isError = true;
                Log.ErrorFormat("downLoadAttachment Error: empty data file for KEYID: " + _keyID + " and attachment type is : " + tableType);
            }

            return isError;

        }

        protected void DeliverFile(byte[] Data, string Type, int Length, string DownloadFileName)
        {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearHeaders();
            response.Clear();
            response.ContentType = Type;
            if (!string.IsNullOrEmpty(DownloadFileName))
            {
                //Add filename to header 

                response.AddHeader("Connection", "keep-alive");
                response.AddHeader("Content-Length", Convert.ToString(Length));

                response.AddHeader("Content-Disposition", "attachment; filename=\"" + DownloadFileName + "\"");

                switch (Type)
                {
                    case "False":
                        response.ContentType = "application/octet-stream";
                        response.Charset = "UTF-8";
                        break;
                    default:
                        response.ContentType = Type;
                        break;
                }
            }
            response.OutputStream.Write(Data, 0, Length);
            response.End();
        }

        public string checkFilePDF(string filename, int file_size)
        {
            string ErrorMsg = "";
            if (filename.Length > 90)
            {

                ErrorMsg = "Error: File Name is too long. Please pick a shorter name and try attaching the file again. ";
            }


            if (file_size / 1024 > 31240)
            {

                ErrorMsg = "Error: File size exceeded the allowed limit of 30MB. Please pick a different file and try again. ";
            }

            string _lowName = filename.ToLower();
            bool isSupportedFile = false;
            if (_lowName.EndsWith(".pdf"))
            {
                isSupportedFile = true;
            }

            if (!isSupportedFile)
            {
                ErrorMsg = "The uploaded file format is not supported.";
            }

            return ErrorMsg;
        }
    }
}