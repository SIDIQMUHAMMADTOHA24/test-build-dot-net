using Api.Share.Ekyc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Api.Share.Tools.Result;

namespace App.ConnectApi.Ekyc
{
    public class Ra
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public Ra(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_EKYC"));
            this._an = auth_name;
        }

        public async Task<PostResultObject<ResponseARI>> Verification(BodyARI model, string token)
        {
            PostResultObject<ResponseARI> datas = new PostResultObject<ResponseARI>();
            try
            {
                string url_api = string.Format($"api/ra/verif");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<ResponseARI>>(data);
                    return datas;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

    }
}
