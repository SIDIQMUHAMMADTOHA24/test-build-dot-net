using Api.Share.Tools;
using MailLib.Library;
using Sodium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace App.Web.Helper
{
    public class SendEmail
    {
        public bool SelfMail(string toMail, string subject, string body)
        {
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var pass_dekr = SealedPublicKeyBox.Open(Convert.FromBase64String(ConfigurationManager.AppSettings.Get("email_cred")), key_pair_certificate);
                string email_cred = Encoding.UTF8.GetString(pass_dekr);

                string email_user = ConfigurationManager.AppSettings.Get("email_user");
                int smpt_port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("smtp_port"));
                string smtp_host = ConfigurationManager.AppSettings.Get("smtp_host");
                int imap_port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("imap_port"));
                string imap_host = ConfigurationManager.AppSettings.Get("imap_host");

                string plain = email_cred;
                string from_mail = email_user;
                var e_split = email_user.Split('@');
                string j_provider = e_split[e_split.Length-1];
                string[] a = new string[1];
                a[0] = toMail;
                Email email = new Email { To = a, Subject = subject, Body = body };
                JenisProvider jenis_provider = JenisProvider.other;
                ProviderModel provider_model = new ProviderModel();

                if (j_provider == Const.gmail)
                {
                    jenis_provider = JenisProvider.gmail;
                }
                else if (j_provider == Const.outlook)
                {
                    jenis_provider = JenisProvider.other;
                    provider_model = new ProviderModel() { Name = "WebMail", SmtpPort = smpt_port, SmtpServer = smtp_host, ImapPort = imap_port, ImapServer = imap_host };
                }
                else if (j_provider == Const.yahoo)
                {
                    jenis_provider = JenisProvider.yahoo;
                }
                else
                {
                    jenis_provider = JenisProvider.other;
                    provider_model = new ProviderModel() { Name = "WebMail", SmtpPort = smpt_port, SmtpServer = smtp_host, ImapPort = imap_port, ImapServer = imap_host };
                }
                //Todo : send mail for other
                string message = new SendMail(AuthType.NoOauth, null, from_mail, plain, jenis_provider, provider_model, email).Compose();
                if (message == MailBee.ErrorCodes.OK.ToString())
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string BodyHtml(string url, string fromName, BodyEmailType type, string path_image)
        {
            string body = string.Empty;

            if (type == BodyEmailType.VERIF_SUBSCRIBER)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1}, Welcome to Enkripa Product!</p>
                       <p>Please confirm your account by clicking link below.</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #3598DC;color:#5C9BD1;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">CONFIRM EMAIL</a></p>" +
                       "<p>---<br />Best regards,<br />EnkripaSign</p><br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName);
            }
            else if (type == BodyEmailType.CHANGE_PASSWORD)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello, Welcome to EnkripaSign!</p>
                       <p>Recently a request was submitted to reset your password account. If you did not request this, please ignore this email.</p>
                       <p>To reset your password, please visit the url below</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #3598DC;color:#5C9BD1;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">CHANGE PASSWORD</a></p>" +
                       "<p>When you visit the link above, you will have the opportunity to choose a new password.</p><br />" +
                       "<p>---<br />Best regards,<br />EnkripaSign</p><br />" +
                       "<p><center><small>Copyright © EnkripaSign, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image);
            }
            else if (type == BodyEmailType.UPDATE_PASSWORD)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello, Welcome to EnkripaSign!</p>
                       <p>Recently a request was submitted to update your password account. If you did not request this, please contact your administrator.</p><br />" +
                       "<p>Ignore this message if you changed the password.</p><br />" +
                       "<p>---<br />Best regards,<br />EnkripaSign</p><br />" +
                       "<p><center><small>Copyright © EnkripaSign, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image);
            }
            else if (type == BodyEmailType.SEND_SIGN)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""></td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello!</p>
                       <p>You have just received a document to view or sign.</p>
                       <p>Please download this document by clicking link below.</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #22313F;color:#2F353B;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">OPEN FILE</a></p>" +
                       "<p>---<br />Best regards,<br />" + fromName + "</p><br />" +
                       "<p><center><small>Copyright © EnkripaSign, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image);
            }
            else if (type == BodyEmailType.SEND_DOCS)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""></td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello!</p>
                       <p>You have just received a document to view.</p>
                       <p>Please download this document by clicking link below.</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #22313F;color:#2F353B;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">DOWNLOAD DOCUMENT</a></p>" +
                       "<p>---<br />Best regards,<br />" + fromName + "</p><br />" +
                       "<p><center><small>Copyright © EnkripaSign, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image);
            }
            else if (type == BodyEmailType.SHARE_SIGN)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""></td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello!</p>
                       <p>Please download this document by clicking link below.</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #22313F;color:#2F353B;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">DOWNLOAD FILE</a></p>" +
                       "<p>---<br />Best regards,<br />" + fromName + "</p><br />" +
                       "<p><center><small>Copyright © EnkripaSign, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image);
            }
            else if(type == BodyEmailType.ACTIVE_USER)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1}, Welcome to Enkripa Product!</p>
                       <p>Your Account is active</p>
                       <p style=""text-align:center""><a style=""border: 1px solid #3598DC;color:#5C9BD1;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">LOGIN</a></p>" +
                       "---<br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName);
            }
            else if (type == BodyEmailType.VALIDATE_USER)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1}, Welcome to Enkripa Product!</p>
                       <p>Your account is currently in the validation process</p>
                       " +
                       "---<br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName);
            }
            else if (type == BodyEmailType.REVOKE_CERT)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1},</p>
                       <p>your electronic certificate has been successfully revoked</p>
                       " +
                       "---<br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName);
            }
            else if (type == BodyEmailType.REKEYING_CERT)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1},</p>
                       <p>Your request for rekeying certificate has been approved. Please click link to next process</p>
                        <p style=""text-align:center""><a style=""border: 1px solid #3598DC;color:#5C9BD1;display:inline-block;font-size:14px;letter-spacing:1px;padding:12px 15px;text-decoration:none;border-radius:3px"" href=" + url + ">NEXT PROCESS</a></p>" +
                       "---<br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName);
            }
            else if (type == BodyEmailType.SEND_TOKEN)
            {
                body = string.Format(@"<html><body>
                       <div id="":km"" class=""ii gt""><div id="":kn"" class=""a3s aXjCH m1623f13b0062337b""><u></u>
                       <div bgcolor=""#efefef"" marginwidth=""0"" marginheight=""0"" style=""margin:0;padding:0;background:#efefef;font-family:'Helvetica',Helvetica Neue,Segoe UI,Arial,sans-serif"">
                       <table cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#efefef"" style=""background:#efefef;margin:0 auto"">
                       <table border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""padding: 10px"" class=""m_-1759678229138108633email-container"">
                       <tbody><tr><td style=""border-bottom:3px solid #BFCAD1;background:#FFFFFF;padding:30px;border-radius:3px 3px 0 0;text-align:center""> <img src=""{0}"" style=""width:181.5;height:60px""> </td></tr> 
                       <tr><td><table  border=""0"" width=""100%"" cellpadding=""0"" cellspacing""0"" align=""center"" style=""background:#fff"">
                       <tbody><tr><td style=""padding:30px; color:#333333""><div>
                       <p>Hello {1},</p>
                       <p>this is your token : {2}</p>
                       " +
                       "---<br />" +
                       "<p><center><small>Copyright © PT Enkripa Teknologi Indonesia, All rights reserved</small></center></p>" +
                       "</div></td></tr></tbody></table></td></tr></tbody></table></table></div></div></div>" +
                       "</body></html>", path_image, fromName, url);
            }



            return body;
        }

        public enum BodyEmailType
        {
            VERIF_SUBSCRIBER = 1,
            SEND_SIGN = 2,
            SHARE_SIGN = 3,
            CHANGE_PASSWORD = 4,
            SEND_DOCS = 5,
            UPDATE_PASSWORD = 6,
            VALIDATE_USER = 7,
            ACTIVE_USER = 8,
            REVOKE_CERT = 9,
            REKEYING_CERT = 10,
            SEND_TOKEN = 11
        }
    }
}