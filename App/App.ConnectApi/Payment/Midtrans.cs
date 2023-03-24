using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ConnectApi.Payment
{
    public class Midtrans
    {
        private readonly ConnectionApi _con;
        public Midtrans()
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_PAYMENT"));
        }
    }
}
