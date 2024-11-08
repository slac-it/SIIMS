using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIIMS
{
    public partial class test1 : System.Web.UI.Page
    {
        string _connStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            _connStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=SLACDEV2.slac.stanford.edu)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SLACDEV2))); User Id=apps_admin;Password=s3cure.a.2017.02;";
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            string txt1 = txtInput.Text;
            string outdata = ReplaceWordChars(txt1);
         
            insertDB(txt1, outdata);

            readDB();
        }

        private void readDB()
        {
            string _sql;
            _sql = "select CONVERTED from SIIMS_TESTING ";
           

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;

                string converted;
               
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    converted = (string)reader["CONVERTED"];
                    txtOutput.Text = converted;
                }

               

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
               
            }
        }

        private void insertDB(string txt1, string outdata)
        {
            string _sql;

            _sql = @"BEGIN delete from SIIMS_TESTING; commit;    insert into SIIMS_TESTING (INPUT_STRING, CONVERTED) values(:input,:converted) ;  END; "  ;
         
            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":input", OracleDbType.Varchar2).Value = txt1;
                cmd.Parameters.Add(":converted", OracleDbType.Varchar2).Value = outdata;

                cmd.ExecuteNonQuery();
              
                con.Close();
            }
            catch (Exception ex)
            {
               
            }
        }

        public string ReplaceWordChars(string text)
        {
            var s = text;
            // smart single quotes and apostrophe
            s = Regex.Replace(s, "[\u2018\u2019\u201A]", "'");
            // smart double quotes
            s = Regex.Replace(s, "[\u201C\u201D\u201E]", "\"");
            // ellipsis
            s = Regex.Replace(s, "\u2026", "...");
            // dashes
            s = Regex.Replace(s, "[\u2013\u2014]", "-");
            // circumflex
            s = Regex.Replace(s, "\u02C6", "^");
            // open angle bracket
            s = Regex.Replace(s, "\u2039", "<");
            // close angle bracket
            s = Regex.Replace(s, "\u203A", ">");
            // spaces
            s = Regex.Replace(s, "[\u02DC\u00A0]", " ");

            return s;
        }
    }
}