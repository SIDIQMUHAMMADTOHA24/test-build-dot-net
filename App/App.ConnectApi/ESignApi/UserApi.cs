using Api.Share.ESignApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.ConnectApi.ESignApi
{
    public class UserApi
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public UserApi(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_ENKRIPA_SIGN_API"));
            this._an = auth_name;
        }

        public UserPhrase GetPhraseApi(string iduser, string token)
        {
            try
            {
                HttpResponseMessage response = this._con.getconnectapisign("/api/user/get_phrase?iduser=" + iduser, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    UserPhrase datas = JsonConvert.DeserializeObject<UserPhrase>(data);
                    return datas;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> PostPhraseApi(UserPhrase model, string token)
        {
            try
            {
                HttpResponseMessage response = await this._con.postconnectapisign("/api/user/post_phrase", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
