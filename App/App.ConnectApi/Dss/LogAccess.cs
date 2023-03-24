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
    public class LogAccess
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public LogAccess(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_ENKRIPA_SIGN"));
            this._an = auth_name;
        }

        public async Task<PostResultObject<MsgResponse<MsgResponseData<List<Log>>>>> List(GetList model, string token)
        {
            PostResultObject<MsgResponse<MsgResponseData<List<Log>>>> datas = new PostResultObject<MsgResponse<MsgResponseData<List<Log>>>>();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/log_access/list", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<MsgResponse<MsgResponseData<List<Log>>>>>(data);
                    return datas;
                }
                else
                {
                    datas.message = "Gagal";
                    datas.result = false;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.message = ex.Message.ToString();
                datas.result = false;
                return datas;
            }
        }
    }
}
