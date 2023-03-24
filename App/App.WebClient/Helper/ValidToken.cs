using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Sms;
using App.ConnectApi.ThisApi;
using ICKEncryption;
using Newtonsoft.Json;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Api.Share.Tools.Result;

namespace App.WebClient.Helper
{
    public class ValidToken
    {
        private readonly UserApi _uan;
        private readonly SmsApi _sms;
        public ValidToken()
        {
            this._uan = new UserApi(Const._auth_name);
            this._sms = new SmsApi();
        }
        public async Task<bool> Generate(string token, string user_name, ForRegUser model)
        {
            try
            {
                UserValidToken user_token = new UserValidToken() { user_name = user_name };
                var result_user_token = await _uan.PostUserValidToken(user_token, token);
                string token_otp = result_user_token.objek.valid_token;

                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var token_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(token_otp), key_pair_certificate);
                string plain = Convert.ToBase64String(token_decrypt);
                //send to sms
                byte[] data_auth = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format($"{Const.sms_user_name}{Const.sms_pass}{model.user.user_info.phone_number}")));
                string auth = BitConverter.ToString(data_auth);
                auth = auth.Replace(@"-", "").ToLower();
                string message = Const.GenerateSmsContent(plain);
                var x = this._sms.Send(Const.sms_user_name, model.user.user_info.phone_number, message, auth, Guid.NewGuid().ToString());
                //end send to sms
                return x;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<PostResult> ProcessValidToken(UserValidToken model, string token)
        {
            try
            {
                model.type_validate = TypeValidate.client;
                var result = await _uan.ValidateToken(model, token);
                return result;
            }
            catch (Exception)
            {
                PostResult pr = new PostResult() { result = false, message = "Bad Req"};
                return pr;
            }
        }

        public async Task<string> GenerateUrlWithToken(string user_name, string id_user, string token)
        {
            try
            {
                UserValidToken user_token = new UserValidToken() { user_name = user_name };
                var result_user_token = await _uan.PostUserValidToken(user_token, token);
                string tokenId = result_user_token.objek.valid_token;
                var jwtObj = new FlowSend()
                {
                    Id = user_name,
                    Token = tokenId,
                    IdFlow = id_user
                };
                var jwtID = Jwt.Encode(jwtObj, Symmetric.JWTKEY);
                string id = HttpUtility.UrlEncode(jwtID);
                return id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class FlowSend
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("IdFlow")]
        public string IdFlow { get; set; }
        [JsonProperty("Token")]
        public string Token { get; set; }
    }
}