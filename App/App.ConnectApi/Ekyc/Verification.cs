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
    public class Verification
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public Verification(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_EKYC"));
            this._an = auth_name;
        }

        public async Task<PostResultObject<UserEkyc>> PostVerification(InputVerification model, string token)
        {
            PostResultObject<UserEkyc> datas = new PostResultObject<UserEkyc>();
            try
            {
                string url_api = string.Format($"/api/ekyc/post");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<UserEkyc>>(data);
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

        public async Task<PostResultObject<UserEkyc>> PostVerificationPR(InputVerificationPR model, string token)
        {
            PostResultObject<UserEkyc> datas = new PostResultObject<UserEkyc>();
            try
            {
                string url_api = string.Format($"/api/ekyc/post_pr");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<UserEkyc>>(data);
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

        public RandomQuestion GetRandomQuestion()
        {
            try
            {
                string url_api = string.Format($"/api/random_question/get");
                HttpResponseMessage response = _con.getconnectapi(url_api);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<RandomQuestion>(data);
                    return result;
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

        public GetResultObject<VideoAndQuestion> GetData(string token, string id_ekyc)
        {
            GetResultObject<VideoAndQuestion> datas = new GetResultObject<VideoAndQuestion>();
            try
            {
                string url_api = string.Format($"/api/ekyc/get_data?id_ekyc={id_ekyc}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultObject<VideoAndQuestion>>(data);
                    return datas;
                }
                else
                {
                    datas.result = false;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas = new GetResultObject<VideoAndQuestion>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<PostResult> PostVerifManual(InputVerifManual model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/ekyc/post_verif_manual");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
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
