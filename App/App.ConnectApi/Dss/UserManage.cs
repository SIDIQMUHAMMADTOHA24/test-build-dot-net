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
    public class UserManage
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public UserManage(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_ENKRIPA_SIGN"));
            this._an = auth_name;
        }

        public async Task<PostResult> AddUser(UserCa model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/user/add", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                    return datas;
                }
                else
                {
                    datas = new PostResult() { message = "Gagal", result = false };
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas = new PostResult() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }
    }
}
