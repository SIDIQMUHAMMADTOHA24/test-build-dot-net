using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.ThisApi;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static Api.Share.Tools.Result;

namespace App.Web.Helper
{
    public class AuthToken
    {
        private readonly UserApi _uan;
        private readonly SendEmail _send;
        public AuthToken()
        {
            this._uan = new UserApi(Const._auth_name);
            this._send = new SendEmail();
        }

        public async Task<bool> Generate(string token, string user_name)
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
                //send to email
                string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripa.png");
                string html = _send.BodyHtml(plain, user_name, SendEmail.BodyEmailType.SEND_TOKEN, path_image);
                bool send = _send.SelfMail(user_name, "EnkripaAdmin Token", html);
                //end send to email
                return true;
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
                PostResult pr = new PostResult() { result = false, message = "Bad Req" };
                return pr;
            }
        }
    }
}