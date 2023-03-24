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
    public class CertificateManage
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public CertificateManage(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_ENKRIPA_SIGN"));
            this._an = auth_name;
        }

        public async Task<PostResult> Enrool(CertificateDss model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/certificate/enroll", model, token, this._an);
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

        public async Task<PostResultObject<MsgResponse<MsgResponseData<List<CertificateDss>>>>> List(GetList model, string token)
        {
            PostResultObject<MsgResponse<MsgResponseData<List<CertificateDss>>>> datas = new PostResultObject<MsgResponse<MsgResponseData<List<CertificateDss>>>>();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/certificate/list", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<MsgResponse<MsgResponseData<List<CertificateDss>>>>>(data);
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

        public PostResultObject<MsgResponse<List<CertificateDssGet>>> ListByUserName(string user_name, string token)
        {
            PostResultObject<MsgResponse<List<CertificateDssGet>>> datas = new PostResultObject<MsgResponse<List<CertificateDssGet>>>();
            try
            {
                string url = string.Format($"/api/certificate/list_by_username?user_name={user_name}");
                HttpResponseMessage response = _con.getconnectapisign(url, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<MsgResponse<List<CertificateDssGet>>>>(data);
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

        public async Task<PostResult> Revoke(PostRevoke model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/certificate/revoke", model, token, this._an);
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

        public async Task<PostResult> RevokeAll(string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/certificate/revoke_all", null, token, this._an);
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

        public async Task<PostResultObject<UserCa>> Renewal(CertificateDss model, string token)
        {
            PostResultObject<UserCa> datas = new PostResultObject<UserCa>();
            try
            {
                HttpResponseMessage response = await _con.postconnectapisign("/api/certificate/renewal", model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<UserCa>>(data);
                    return datas;
                }
                else
                {
                    datas = new PostResultObject<UserCa>() { message = "Gagal", result = false };
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas = new PostResultObject<UserCa>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

    }
}
