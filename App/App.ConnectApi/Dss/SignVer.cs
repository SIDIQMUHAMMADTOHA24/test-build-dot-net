using Api.Share.Dss;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Api.Share.Tools.Result;

namespace App.ConnectApi.Dss
{
    public class SignVer
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public SignVer(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_ENKRIPA_SIGN"));
            this._an = auth_name;
        }

        public async Task<MsgResponse<VerifyData>> Verify(VerifyFile model, string token)
        {
            MsgResponse<VerifyData> datas = new MsgResponse<VerifyData>();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/trans/verify_sign_pdf", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<MsgResponse<VerifyData>>(data);
                    return datas;
                }
                else
                {
                    datas = new MsgResponse<VerifyData>() { message = "Gagal", success = false };
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas = new MsgResponse<VerifyData> { message = ex.Message.ToString(), success = false };
                return datas;
            }
        }
    }
}
