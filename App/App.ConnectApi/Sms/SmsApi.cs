using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.ConnectApi.Sms
{
    public class SmsApi
    {
        private readonly ConnectionApi _con;
        public SmsApi()
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_SMS"));
        }

        public bool Send(string user_name, string mobile, string message, string auth, string trx_id)
        {
            try
            {
                string url = string.Format($"/masking/api/sendSMS.php?username={user_name}&mobile={mobile}&message={message}&auth={auth}&trxid={trx_id}&type=0");
                HttpResponseMessage response = this._con.getconnectapi(url);
                if (response.IsSuccessStatusCode)
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
    }
}
