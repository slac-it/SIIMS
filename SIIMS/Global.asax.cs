using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SIIMS
{
    public class Global : System.Web.HttpApplication
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception _ex = Server.GetLastError();

            string url = HttpContext.Current.Request.Url.AbsolutePath.ToString();
            string msg = _ex.Message;
            msg += @"   Inner Exception: " + _ex.InnerException;

            const string lineSearch = ":line ";
            var index = _ex.StackTrace.LastIndexOf(lineSearch);
            string lineNumberText = string.Empty;
            if (index != -1)
            {
                lineNumberText = _ex.StackTrace.Substring(index + lineSearch.Length);
            }
            else
            {
                lineNumberText = "none found";
            }



            //Log.Error(url, _ex);
            //Response.Write("url: " +  url + "   " + "Error: " + _ex.Message.ToString());

            // Clear the error from the server
            Server.ClearError();

            if (_ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.Write(@"<html><head><title>HTML Not Allowed</title> </head>
                            <body style='font-family: Arial, Sans-serif;'><h1>Error Page!</h1>
                            <p>Error: for security reason, some characters are not allowed.</p>
                            <p>Please make sure that your inputs do not contain any angle brackets like &lt; or &gt;.</p>
                            <p><a href='javascript:history.go(-1);'>Go back</a></p></body></html>");
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(@"<html><head><title>Error Page</title> </head>
                            <body style='font-family: Arial, Sans-serif;'><h1>Error Page!</h1>
                            <p>We're sorry. Something unexpected happened. <br /><br /></p>" +
                            @"<p>" + msg + @"<br /><br /></p>" +
                            @"<p> line number: " + lineNumberText + @"<br /><br /></p>" +
                            @"<p>" + url + @"<br /><br /></p>" +
                            @"<p> Please <a href=https://slacspace.slac.stanford.edu/Operations/SCCS/AppDev/request>Contact AppDev team</a> for help.  Thank you!</p></body></html>");
                Response.End();
            }



        }

       

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

    // Create our own utility for exceptions
    public sealed class ExceptionUtility
    {
        // All methods are static, so this can be private
        private ExceptionUtility()
        { }

        // Log an Exception
        public static void LogException(Exception exc, string source)
        {
            // Include enterprise logic for logging exceptions
            // Get the absolute path to the log file
            string logFile = "App_Data/ErrorLog.txt";
            logFile = HttpContext.Current.Server.MapPath(logFile);

            // Open the log file for append and write the log
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine("********** {0} **********", DateTime.Now);
            if (exc.InnerException != null)
            {
                sw.Write("Inner Exception Type: ");
                sw.WriteLine(exc.InnerException.GetType().ToString());
                sw.Write("Inner Exception: ");
                sw.WriteLine(exc.InnerException.Message);
                sw.Write("Inner Source: ");
                sw.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.WriteLine("Inner Stack Trace: ");
                    sw.WriteLine(exc.InnerException.StackTrace);
                }
            }
            sw.Write("Exception Type: ");
            sw.WriteLine(exc.GetType().ToString());
            sw.WriteLine("Exception: " + exc.Message);
            sw.WriteLine("Source: " + source);
            sw.WriteLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                sw.WriteLine(exc.StackTrace);
                sw.WriteLine();
            }
            sw.Close();
        }
    }
}