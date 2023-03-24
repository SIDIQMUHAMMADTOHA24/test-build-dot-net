using Api.Share.Dss;
using Api.Share.Ekyc;
using Api.Share.Payment;
using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Dss;
using App.ConnectApi.Ekyc;
using App.ConnectApi.ThisApi;
using App.WebClient.Auth;
using App.WebClient.Helper;
using Newtonsoft.Json;
using OTP;
using Sodium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Api.Share.Tools.Result;

namespace App.WebClient.Controllers
{
    public class UserController : Controller
    {
        private readonly Verification _ea;
        private readonly UserApi _ua;
        private readonly SendEmail _send;
        private readonly UserManage _um_ca;
        public UserController()
        {
            this._ea = new Verification(Const._auth_name);
            this._ua = new UserApi(Const._auth_name);
            this._send = new SendEmail();
            this._um_ca = new UserManage(Const._authorization);
        }
        // GET: User
        [HttpGet]
        public ActionResult SignUp(string token_data)
        {
            ViewBag.token_data = token_data;
            return View();
        }

        [HttpGet]
        public ActionResult SignUpPr()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public JsonResult GenerateImageCaptcha()
        {
            PostResult pr = new PostResult();
            try
            {
                Session[Const.s_captcha] = null;
                string ccode = CaptchaCustom.GenerateCaptchaCode(8);
                Session[Const.s_captcha] = ccode;
                var image = CaptchaCustom.GenerateCaptchaImage(ccode);
                pr.result = true;
                pr.message = image;
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> GenerateToken(ForRegUser model)
        {
            PostResult pr = new PostResult();
            try
            {
                if(model.token_data == null)
                {
                    pr = new PostResult() { result = false, message = "Captcha is not correct" };
                    return Json(pr, JsonRequestBehavior.AllowGet);
                }

                string cc = (string)Session[Const.s_captcha];
                if(model.token_data != cc)
                {
                    pr = new PostResult() { result = false, message = "Captcha is not correct" };
                    return Json(pr, JsonRequestBehavior.AllowGet);
                }

                Session[Const.s_captcha] = null;

                UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                PostResultObject<TokenAccess> token = await _ua.Login(login);

                ValidToken validate_token = new ValidToken();
                bool result = await validate_token.Generate(token.objek.token, model.user.user_info.email_address, model);
                pr.result = result;
                if (pr.result)
                {
                    pr.message = "Success send token, please check SMS";

                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Agree to all applicable terms, conditions, and privacy policy", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_ok }, token.objek.token);
                }
                else
                {
                    pr.message = "Unsuccess send token, please resend token";

                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Unsuccess send token, please resend token", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_alert }, token.objek.token);
                }
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> PostVerification(ForRegUser model)
        {
            PostResult result = new PostResult();
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);

                UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                PostResultObject<TokenAccess> token = await _ua.Login(login);

                ValidToken validate_token = new ValidToken();
                UserValidToken user_valid_token = new UserValidToken() { user_name = model.user.user_info.email_address, valid_token = model.token_otp };
                PostResult validToken = await validate_token.ProcessValidToken(user_valid_token, token.objek.token);
                if (!validToken.result)
                {
                    result = new PostResult() { result = false, message = validToken.message };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Mobile number has been validated", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_ok }, token.objek.token);
                }

                PostResultObject<UserEkyc> x = await _ea.PostVerification(model.input_verification, token.objek.token);
                if (x.result)
                {
                    model.user.user_info.id = Guid.NewGuid();

                    if (model.token_data == null)
                    {
                        model.user.user_info.id_subscriber = model.user.user_info.id;
                        model.user.user_login_info.login_type = LoginType.client;
                    }
                    else
                    {
                        string id = model.token_data;
                        var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(id), key_pair_certificate);
                        string id_subcriber_plain = Encoding.UTF8.GetString(pass_decrypt);
                        model.user.user_info.id_subscriber = Guid.Parse(id_subcriber_plain);
                        model.user.user_login_info.login_type = LoginType.User;
                    }

                    model.user.user_info.input_date = DateTime.UtcNow;
                    model.user.user_info.update_date = DateTime.UtcNow;
                    model.user.user_info.status = StatusUserInfo.pending;
                    model.user.user_login_info.is_active = false;
                    model.user.user_login_info.input_date = DateTime.UtcNow;
                    model.user.user_login_info.update_date = DateTime.UtcNow;
                    model.user.user_login_info.active_date = DateTime.UtcNow;
                    model.user.user_login_info.id = Guid.NewGuid();
                    var min_confirm = float.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("min_confirm"));

                    PostResult _post_result = new PostResult() { result = true, message = string.Empty };

                    string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                    string url = string.Format($"{BASEURL_APP}");
                    string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripasign.png");
                    List<PostResultEkyc> list_confidence = JsonConvert.DeserializeObject<List<PostResultEkyc>>(x.objek.confidence);
                    var false_confidence = list_confidence.Where(o => o.result == false);
                    var face_compare = list_confidence.Where(o => o.message == "face_compare").SingleOrDefault();
                    if (false_confidence.Count() == 0 && Convert.ToDouble(face_compare.face_compare) >= 0.80)
                    {
                        //check ada enkripa_sign
                        var p_ekripa_sign = model.user.list_user_package.Where(o => o.package == TypePackage.enkripa_sign);
                        if (p_ekripa_sign.Count() != 0)
                        {
                            var pass_enkrip = SealedPublicKeyBox.Create(Const.password_ejbca, key_pair_certificate);
                            string pass_ejbca_enkrip = Convert.ToBase64String(pass_enkrip);

                            UserCa user_ca = new UserCa()
                            {
                                ca_name = Const.ca,
                                cp_name = Const.cp,
                                ee_name = Const.ee,
                                email = model.user.user_info.email_address,
                                password_ejbca = pass_ejbca_enkrip,
                                nama = model.user.user_info.name,
                                subject_dn = string.Format($"CN={model.user.user_info.name} ({model.user.user_info.id}),OU=Personal,C=ID"),
                                username = model.user.user_info.email_address,
                                telepon = model.user.user_info.phone_number,
                                kota = "Jakarta",
                                provinsi = "DKI Jakarta"
                            };
                            var add_user_ca = await this._um_ca.AddUser(user_ca, token.objek.token);
                            _post_result = add_user_ca;
                        }

                        if (_post_result.result)
                        {
                            model.user.user_login_info.is_active = true;
                            model.user.user_info.status = StatusUserInfo.active;
                            //send email
                            string html = _send.BodyHtml(url, model.user.user_info.email_address, SendEmail.BodyEmailType.ACTIVE_USER, path_image);
                            _send.SelfMail(model.user.user_info.email_address, "Welcome to Enkripa Product", html);
                        }
                    }
                    else
                    {
                        var valid_token_client = new ValidToken();
                        string url_validate_email = await validate_token.GenerateUrlWithToken(model.user.user_info.email_address, model.user.user_info.id.ToString(), token.objek.token);
                        url = string.Format($"{BASEURL_APP}Login/VerifyEmailClient?token_data={url_validate_email}");
                        string html = _send.BodyHtml(url, model.user.user_info.email_address, SendEmail.BodyEmailType.VALIDATE_USER, path_image);
                        _send.SelfMail(model.user.user_info.email_address, "Validation User Enkripa Product", html);
                    }

                    if (_post_result.result)
                    {
                        model.user.user_from_ekyc = new UserFromEkyc();
                        model.user.user_from_ekyc.id_ekyc = x.objek.id;
                        model.user.user_info.nik = model.input_verification.body_ra.nik;
                        model.user.user_info.license = "annual";
                        model.user.user_info.payment = TypePayment.transfer;
                        PostResult result_post_user = await _ua.Post(model.user, token.objek.token);

                        await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Successful user registration", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_ok }, token.objek.token);
                    }

                    result = _post_result;
                }
                else
                {
                    result = new PostResult() { result = false, message = x.message };

                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = x.message + "- EKYC", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_alert }, token.objek.token);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> PostVerificationPR(ForRegUserPR model)
        {
            PostResult result = new PostResult();
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);

                UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                PostResultObject<TokenAccess> token = await _ua.Login(login);

                ValidToken validate_token = new ValidToken();
                UserValidToken user_valid_token = new UserValidToken() { user_name = model.user.user_info.email_address, valid_token = model.token_otp };
                PostResult validToken = await validate_token.ProcessValidToken(user_valid_token, token.objek.token);
                if (!validToken.result)
                {
                    result = new PostResult() { result = false, message = validToken.message };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Mobile number has been validated", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_ok }, token.objek.token);
                }

                PostResultObject<UserEkyc> x = await _ea.PostVerificationPR(model.input_verification, token.objek.token);
                if (x.result)
                {
                    model.user.user_info.id = Guid.NewGuid();
                    model.user.badan_usaha.id = Guid.NewGuid();

                    if (model.token_data == null)
                    {
                        model.user.user_info.id_subscriber = model.user.user_info.id;
                        model.user.user_login_info.login_type = LoginType.client;
                    }
                    else
                    {
                        string id = model.token_data;
                        var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(id), key_pair_certificate);
                        string id_subcriber_plain = Encoding.UTF8.GetString(pass_decrypt);
                        model.user.user_info.id_subscriber = Guid.Parse(id_subcriber_plain);
                        model.user.user_login_info.login_type = LoginType.User;
                    }

                    model.user.user_info.input_date = DateTime.UtcNow;
                    model.user.user_info.update_date = DateTime.UtcNow;
                    model.user.user_info.status = StatusUserInfo.pending;
                    model.user.user_login_info.is_active = false;
                    model.user.user_login_info.input_date = DateTime.UtcNow;
                    model.user.user_login_info.update_date = DateTime.UtcNow;
                    model.user.user_login_info.active_date = DateTime.UtcNow;
                    model.user.user_login_info.id = Guid.NewGuid();
                    var min_confirm = float.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("min_confirm"));

                    PostResult _post_result = new PostResult() { result = true, message = string.Empty };

                    string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                    string url = string.Format($"{BASEURL_APP}");
                    string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripasign.png");
                    List<PostResultEkyc> list_confidence = JsonConvert.DeserializeObject<List<PostResultEkyc>>(x.objek.confidence);
                    var false_confidence = list_confidence.Where(o => o.result == false);
                    var face_compare = list_confidence.Where(o => o.message == "face_compare").SingleOrDefault();
                    if (false_confidence.Count() == 0 && Convert.ToDouble(face_compare.face_compare) >= 0.80)
                    {
                        //check ada enkripa_sign
                        var p_ekripa_sign = model.user.list_user_package.Where(o => o.package == TypePackage.enkripa_sign);
                        if (p_ekripa_sign.Count() != 0)
                        {
                            var pass_enkrip = SealedPublicKeyBox.Create(Const.password_ejbca, key_pair_certificate);
                            string pass_ejbca_enkrip = Convert.ToBase64String(pass_enkrip);

                            UserCa user_ca = new UserCa()
                            {
                                ca_name = Const.ca,
                                cp_name = Const.cp_bu,
                                ee_name = Const.ee_bu,
                                email = model.user.user_info.email_address,
                                password_ejbca = pass_ejbca_enkrip,
                                nama = model.user.badan_usaha.nama,
                                subject_dn = string.Format($"CN={model.user.badan_usaha.nama} ({model.user.user_info.id}),C=ID"),
                                username = model.user.user_info.email_address,
                                telepon = model.user.user_info.phone_number,
                                kota = "Jakarta",
                                provinsi = "DKI Jakarta"
                            };
                            var add_user_ca = await this._um_ca.AddUser(user_ca, token.objek.token);
                            _post_result = add_user_ca;
                        }

                        if (_post_result.result)
                        {
                            model.user.user_login_info.is_active = true;
                            model.user.user_info.status = StatusUserInfo.active;
                            //send email
                            string html = _send.BodyHtml(url, model.user.user_info.email_address, SendEmail.BodyEmailType.ACTIVE_USER, path_image);
                            _send.SelfMail(model.user.user_info.email_address, "Welcome to Enkripa Product", html);
                        }
                    }
                    else
                    {
                        var valid_token_client = new ValidToken();
                        string url_validate_email = await validate_token.GenerateUrlWithToken(model.user.user_info.email_address, model.user.user_info.id.ToString(), token.objek.token);
                        url = string.Format($"{BASEURL_APP}Login/VerifyEmailClient?token_data={url_validate_email}");
                        string html = _send.BodyHtml(url, model.user.user_info.email_address, SendEmail.BodyEmailType.VALIDATE_USER, path_image);
                        _send.SelfMail(model.user.user_info.email_address, "Validation User Enkripa Product", html);
                    }

                    if (_post_result.result)
                    {
                        model.user.user_from_ekyc = new UserFromEkyc();
                        model.user.user_from_ekyc.id_ekyc = x.objek.id;
                        model.user.user_info.nik = model.input_verification.body_ra.nik;
                        model.user.user_info.license = "annual";
                        model.user.user_info.payment = TypePayment.transfer;
                        PostResult result_post_user = await _ua.PostPR(model.user, token.objek.token);

                        await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = "Successful user registration", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_ok }, token.objek.token);
                    }

                    result = _post_result;
                }
                else
                {
                    result = new PostResult() { result = false, message = x.message };

                    await _ua.PostUserLog(new UserLog() { user_name = model.user.user_info.email_address, activity = x.message + "- EKYC", status = string.Empty, time_input = DateTime.UtcNow, status_log = Const.log_alert }, token.objek.token);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        //public async Task<JsonResult> CheckNik(string nik)
        //{
        //    PostResultValid pr = new PostResultValid();
        //    try
        //    {
        //        UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
        //        PostResultObject<TokenAccess> token = await _ua.Login(login);

        //        pr = this._ua.CheckNik(nik, token.objek.token);

        //        return Json(pr, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        pr = new PostResultValid() { valid = false, message = ex.Message.ToString() };
        //        return Json(pr, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> CheckUserName(string email)
        //{
        //    PostResultValid pr = new PostResultValid();
        //    try
        //    {
        //        UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
        //        PostResultObject<TokenAccess> token = await _ua.Login(login);

        //        pr = this._ua.CheckUserName(email, token.objek.token);

        //        return Json(pr, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        pr = new PostResultValid() { valid = false, message = ex.Message.ToString() };
        //        return Json(pr, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> UpdateStatusPayment(PushNotif model)
        {
            PostResult pr = new PostResult();
            try
            {
                var mids_server_key = ConfigurationManager.AppSettings.Get("mids_server_key");
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var pass_enk = SealedPublicKeyBox.Open(Convert.FromBase64String(mids_server_key), key_pair_certificate);
                string mids_server_key_dekrip = Encoding.UTF8.GetString(pass_enk).Replace(":", "");

                var sign_key = model.signature_key;
                var sha512 = BitConverter.ToString(CryptoHash.Sha512(string.Format($"{model.order_id}{model.status_code}{model.gross_amount}{mids_server_key_dekrip}"))).Replace(@"-", "").ToLower();
                var x_next = sha512.Equals(sign_key);

                if ((model.transaction_status == Const.status_payment_settlement || model.transaction_status == Const.status_payment_capture) && x_next)
                {
                    UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                    PostResultObject<TokenAccess> token = await _ua.Login(login);
                    var a = Convert.ToInt64(model.gross_amount.Split('.')[0]);
                    TransactionDetail trans_detail = new TransactionDetail() { order_id = model.order_id, update_date = DateTime.UtcNow, status = model.transaction_status, gross_amount = a };
                    pr = await this._ua.UpdateStatusPayment(trans_detail, token.objek.token);
                    return Json(pr, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    pr = new PostResult() { result = false, message = "" };
                    return Json(pr, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }
    }
}