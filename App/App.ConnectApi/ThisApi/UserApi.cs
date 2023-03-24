using Api.Share.Payment;
using Api.Share.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Api.Share.Tools.Result;

namespace App.ConnectApi.ThisApi
{
    public class UserApi
    {
        private readonly ConnectionApi _con;
        private readonly string _an;
        public UserApi(string auth_name)
        {
            this._con = new ConnectionApi(System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_API"));
            this._an = auth_name;
        }

        public async Task<PostResult> Post(User model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/post");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> PostPR(UserPR model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/post_pr");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResultObject<TokenAccess>> Login(UserToLogin model)
        {
            PostResultObject<TokenAccess> datas = new PostResultObject<TokenAccess>();
            try
            {
                string url_api = string.Format($"api/user/login_user");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultObject<TokenAccess>>(data);
                    return datas;
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultData<List<UserInfo>> GetList(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserInfo>> datas = new GetResultData<List<UserInfo>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserInfo>>>(data);
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
                datas = new GetResultData<List<UserInfo>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<bool> UpdateStatus(User model, string token)
        {
            try
            {
                string url_api = string.Format($"/api/user/update_status");
                HttpResponseMessage response = await _con.putconnectapi(url_api, model, token, _an);
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

        public GetResultObject<User> GetData(string token, string id_user_info)
        {
            GetResultObject<User> datas = new GetResultObject<User>();
            try
            {
                string url_api = string.Format($"/api/user/get?id_user_info={id_user_info}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultObject<User>>(data);
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
                datas = new GetResultObject<User>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<PostResult> PostUserLog(UserLog model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                model.apps = "Registration";
                string url_api = string.Format($"/api/user/post_log");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> UpdateUserLogStatus(UserLog model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/update_log_status");
                HttpResponseMessage response = await _con.postconnectapinoreplace(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultData<List<UserLog>> GetListLog(string token, int start, int length, int kolom, string order_by, string search, string from_date, string to_date)
        {
            GetResultData<List<UserLog>> datas = new GetResultData<List<UserLog>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_log?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}&from_date={from_date}&to_date={to_date}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserLog>>>(data);
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
                datas = new GetResultData<List<UserLog>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public GetResultData<List<UserAccess>> GetListUserAccess(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserAccess>> datas = new GetResultData<List<UserAccess>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_user_access?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserAccess>>>(data);
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
                datas = new GetResultData<List<UserAccess>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public GetResultData<List<UserAccess>> GetListUserAccessHt(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserAccess>> datas = new GetResultData<List<UserAccess>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_user_access_ht?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserAccess>>>(data);
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
                datas = new GetResultData<List<UserAccess>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<PostResult> PostUserAccess(UserAccess model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/post_user_access");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultData<List<UserInfo>> GetListAdmin(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserInfo>> datas = new GetResultData<List<UserInfo>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_admin?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserInfo>>>(data);
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
                datas = new GetResultData<List<UserInfo>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<PostResult> PostAdmin(User model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/post_admin");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> DeleteAdmin(string id_user_info, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/delete_admin?id_user_info={id_user_info}");
                HttpResponseMessage response = await _con.deleteconnectapi(url_api, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                    datas.message = "success";
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> Delete(string id_user_info, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/delete?id_user_info={id_user_info}");
                HttpResponseMessage response = await _con.deleteconnectapi(url_api, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                    datas.message = "success";
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> ChangePassword(UserToChangePassword model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/change_password");
                HttpResponseMessage response = await _con.putconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                    return datas;
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultObject<SumDash> GetSumStatus(string token)
        {
            GetResultObject<SumDash> datas = new GetResultData<SumDash>();
            try
            {
                string url_api = string.Format($"/api/user/get_sum_dash");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<SumDash>>(data);
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
                datas = new GetResultObject<SumDash>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public async Task<PostResult> PostUserLogAdmin(UserLogAdmin model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/post_log_admin");
                HttpResponseMessage response = await _con.postconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultData<List<UserLogAdmin>> GetListLogAdmin(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserLogAdmin>> datas = new GetResultData<List<UserLogAdmin>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_log_admin?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserLogAdmin>>>(data);
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
                datas = new GetResultData<List<UserLogAdmin>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public GetResultData<List<UserLoginDatas>> GetLastLogin(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserLoginDatas>> datas = new GetResultData<List<UserLoginDatas>>();
            try
            {
                string url_api = string.Format($"/api/user/get_last_login?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserLoginDatas>>>(data);
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
                datas = new GetResultData<List<UserLoginDatas>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public GetResultData<List<UserCertRevoke>> GetListCertRevoke(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserCertRevoke>> datas = new GetResultData<List<UserCertRevoke>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_cert_revoke?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserCertRevoke>>>(data);
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
                datas = new GetResultData<List<UserCertRevoke>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public PostResult UpdateStatusCertRevoke(string email, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response =  _con.getconnectapi(string.Format($"api/user/update_status_cert_revoke?email={email}"), token, _an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultData<List<UserCertRevoke>> GetListCertRekeying(string token, int start, int length, int kolom, string order_by, string search)
        {
            GetResultData<List<UserCertRevoke>> datas = new GetResultData<List<UserCertRevoke>>();
            try
            {
                string url_api = string.Format($"/api/user/get_list_cert_rekeying?start={start}&length={length}&kolom={kolom}&order_by={order_by}&search={search}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultData<List<UserCertRevoke>>>(data);
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
                datas = new GetResultData<List<UserCertRevoke>>() { message = ex.Message.ToString(), result = false };
                return datas;
            }
        }

        public PostResult UpdateStatusCertRekeying(string email, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = _con.getconnectapi(string.Format($"api/user/update_status_cert_rekeying?email={email}"), token, _an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<GetResultObject<UserValidToken>> PostUserValidToken(UserValidToken model, string token)
        {
            GetResultObject<UserValidToken> datas = new GetResultObject<UserValidToken>();
            try
            {
                HttpResponseMessage response = await _con.postconnectapi("api/user/post_valid_token", model, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<GetResultObject<UserValidToken>>(data);
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> ValidateToken(UserValidToken model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                HttpResponseMessage response = await _con.postconnectapi("/api/user/validate_token", model, token, _an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResult>(data);
                }
                else
                {
                    datas.result = false;
                    datas.message = "Fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public async Task<PostResult> UpdateStatusPayment(TransactionDetail model, string token)
        {
            PostResult datas = new PostResult();
            try
            {
                string url_api = string.Format($"/api/user/update_status_payment");
                HttpResponseMessage response = await _con.putconnectapi(url_api, model, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    datas.result = true;
                }
                else
                {
                    datas.result = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public PostResultValid CheckNik(string nik, string token)
        {
            PostResultValid datas = new PostResultValid();
            try
            {
                string url_api = string.Format($"/api/user/check_nik?nik={nik}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultValid>(data);
                }
                else
                {
                    datas.valid = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.valid = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public PostResultValid CheckUserName(string user_name, string token)
        {
            PostResultValid datas = new PostResultValid();
            try
            {
                string url_api = string.Format($"/api/user/check_user_name?user_name={user_name}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultValid>(data);
                }
                else
                {
                    datas.valid = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.valid = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public PostResultValid CheckBadanUsaha(string id_user_info, string token)
        {
            PostResultValid datas = new PostResultValid();
            try
            {
                string url_api = string.Format($"/api/user/check_badan_usaha?id_user_info={id_user_info}");
                HttpResponseMessage response = _con.getconnectapi(url_api, token, this._an);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<PostResultValid>(data);
                }
                else
                {
                    datas.valid = false;
                    datas.message = "fail";
                }
                return datas;
            }
            catch (Exception ex)
            {
                datas.valid = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }
    }
}
