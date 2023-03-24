using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Tools
{
    public class Const
    {
        public const string _auth_name = "authenticationToken";
        public const string _authorization = "Authorization";
        public const string s_token = "token";
        public const string s_id_user_info = "id_user_info";
        public const string s_nama = "nama";
        public const string s_user_login = "user_login";
        public const string s_login_type = "login_type";
        public const string s_valid_token = "valid_token";
        public const string s_captcha = "captcha_custom";

        public const string akun_admin = "admin";
        public const string akun_super_admin = "super_admin";
        public const string akun_client = "client";
        public const string akun_statis = "statis";
        public const string akun_user = "user";

        public const string ip_user = "_ip_user";

        public const string format_string_date = "yyyy-MM-dd HH:mm:ss.fff";
        public const string format_string_date_utc = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

        public static string private_key_certificate = DpApi.Decrypt(GetTextPk());
        public const string public_key_certificate = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
        public const string nonce = "Q0SYxnlOZub1KkMo9dzL/n2N/PjnwNvD";

        public const string user_client_register = "client_register@enkripa.id";
        public const string pass_client_register = "7tzkQukFPXXRWmyOS0a93VQy2JBX+C4E1TKvDM8uQyMgJhgExhYo/v9GwxvANmCcezE9ydxkj7c=";

        public const string gmail = "gmail.com";
        public const string outlook = "outlook.com";
        public const string yahoo = "yahoo.com";

        public const string cp = "ENKRIPACA-TTE-CERT-PROFILE";
        public const string ee = "ENKRIPACA-TTE-PERSONAL-EE-PROFILE";
        public const string ca = "ENKRIPA CA DS G1";
        public const string password_ejbca = "enkrip@123";

        public const string cp_bu = "ENKRIPACA-TTE-SEGEL";
        public const string ee_bu = "ENKRIPACA-TTE-SEGEL";

        public const string log_ok = "Ok";
        public const string log_alert = "Alert";
        public const string log_reviewed = "Reviewed";

        public const string sms_user_name = "enkripa";
        public const string sms_pass = "AtEPXVxq";

        public const string bank_bca = "bca";
        public const string bank_bni = "bni";
        public const string bank_bri = "bri";

        public const string payment_type_bank_transfer = "bank_transfer";
        public const string payment_type_echannel_mandiri = "echannel";
        public const string payment_type_permata = "permata";

        public const string bill_info1 = "Payment:";
        public const string bill_info2 = "Online purchase";

        public const string status_payment_capture = "capture";
        public const string status_payment_settlement = "settlement";

        public static string GenerateSmsContent(string token)
        {
            return string.Format($"Masukkan kode OTP {token} pada Aplikasi ENKRIPA. JANGAN DIBERIKAN KEPADA SIAPAPUN. Abaikan SMS ini. Jika Anda tidak melakukan transaksi.");
        }

        private static string GetTextPk()
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader("c:\\Temp\\Pk.txt", true);
                var key = sr.ReadToEnd();
                sr.Close();
                return key;
            }
            catch
            {
                return null;
            }
        }
    }
}
