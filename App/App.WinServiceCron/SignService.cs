using Api.Share.User;
using App.WinServiceCron.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Api.Share.Tools.Result;

namespace App.WinServiceCron
{
    public partial class SignService : ServiceBase
    {
        Timer timer = new Timer();
        DateTime schedule_time;
        public SignService()
        {
            InitializeComponent();
            timer = new Timer();
            schedule_time = DateTime.Today.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings.Get("day_run"))).AddHours(Convert.ToInt32(ConfigurationManager.AppSettings.Get("clock_run"))).AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings.Get("minute_run")));
        }

        protected override void OnStart(string[] args)
        {
            
            timer.Interval = schedule_time.Subtract(DateTime.Now).TotalSeconds * 1000; ;
            timer.Elapsed += new ElapsedEventHandler(timer_tick);
            timer.Enabled = true;
            WriteLog("Start");
        }

        private void timer_tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                UserCron user_db = new UserCron();
                GetResultObject<List<UserInfo>> user = user_db.GetUserForChangePassword();
                SendEmailCron send = new SendEmailCron();
                string path_image = string.Format($"{ConfigurationManager.AppSettings.Get("BASEURL_APP")}/assets/media/logos/EnkripaSign.png");
                if (user.result)
                {
                    foreach (var u in user.objek)
                    {
                        string url_change_pass = string.Format($"{ConfigurationManager.AppSettings.Get("BASEURL_APP")}/User/PutPass");
                        string body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1}, Please change your password</p>
                        <p>To change your password, please visit the url below</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #3598DC;color:#5C9BD1;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url_change_pass + ">CHANGE PASSWORD</a></p>" +
                          "<p>---<br />Best regards,<br />EnkripaSign</p><br />" +
                          "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                          "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                          "</body></html>", path_image, u.email_address);

                        string msg = string.Empty;
                        var x = send.SelfMail(u.email_address, "Change Password User EnkripaSign", body, out msg);
                        if (x)
                        {
                            WriteLog("Send Email-Change Password User EnkripaSign to " + u.email_address + " -" + msg);
                        }
                        else
                        {
                            WriteLog("Send Email-Change Password User EnkripaSign to " + u.email_address + " -" + msg);
                        }
                    }
                }
                else
                {
                    WriteLog("From get : " + user.message);
                }

                //send notif perpanjangan sertifikat
                GetResultObject<List<UserInfo>> cert = user_db.GetLogTimeEnroolCertificate(11);
                if (user.result)
                {
                    foreach (var u in user.objek)
                    {
                        string body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1}, Your certificate will expire in the next 30 days. Please renew the certificate</p>
                        <p>To change your password, please visit the url below</p>
                        " +
                          "<p>---<br />Best regards,<br />EnkripaSign</p><br />" +
                          "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                          "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                          "</body></html>", path_image, u.email_address);

                        string msg = string.Empty;
                        var x = send.SelfMail(u.email_address, "Notification renew the certificate EnkripaSign", body, out msg);
                        if (x)
                        {
                            WriteLog("Send Email-Renew Certifikat EnkripaSign to " + u.email_address + " -" + msg);
                        }
                        else
                        {
                            WriteLog("Send Email-Renew Certifikat EnkripaSign to " + u.email_address + " -" + msg);
                        }
                    }
                }
                else
                {
                    WriteLog("From get : " + user.message);
                }

                //delete document
                SignCron doc_db = new SignCron();
                PostResult delete_doc = doc_db.DeleteDoc();
                WriteLog(delete_doc.message);

                //update user log
                PostResult update_log = user_db.UpdateUserLog();
                WriteLog(update_log.message);

                if (timer.Interval != 24 * 60 * 60 * 1000)
                {
                    timer.Interval = 24 * 60 * 60 * 1000;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString());
            }
        }

        protected override void OnStop()
        {
            WriteLog("Stop");
        }

        private static void WriteLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter("c:\\Temp\\EnkripaService.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }
    }
}
