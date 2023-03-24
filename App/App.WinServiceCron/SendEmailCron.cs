using Api.Share.Tools;
using MailLib.Library;
using Sodium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WinServiceCron
{
    public class SendEmailCron
    {
        public bool SelfMail(string toMail, string subject, string body, out string msg)
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
                string j_provider = e_split[e_split.Length - 1];
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
                    msg = "Ok";
                    return true;

                }
                else
                {
                    msg = message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return false;
            }
        }
    }
}
