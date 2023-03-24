using Api.Repo;
using Api.Share;
using Api.Share.Payment;
using Api.Share.Tools;
using Api.Share.User;
using Api.Web.Auth;
using ICKEncryption;
using OTP;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using static Api.Share.Tools.Result;

namespace Api.Web.Controllers
{
    public class UserController : ApiController
    {
        private readonly UserDb _udb;
        public UserController()
        {
            this._udb = new UserDb();
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_super_admin }), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post")]
        public IHttpActionResult Post(User model)
        {
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.user_login_info.password_login), key_pair_certificate);
                string pass_plain = Encoding.UTF8.GetString(pass_decrypt);
                model.user_login_info.password_login = Hashing.PasswordHash(pass_plain);
                var result = _udb.Post(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_super_admin }), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post_pr")]
        public IHttpActionResult PostPR(UserPR model)
        {
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.user_login_info.password_login), key_pair_certificate);
                string pass_plain = Encoding.UTF8.GetString(pass_decrypt);
                model.user_login_info.password_login = Hashing.PasswordHash(pass_plain);
                var result = _udb.PostPR(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("api/user/login_user"), ResponseType(typeof(PostResultObject<TokenAccess>))]
        public IHttpActionResult LoginUser(UserToLogin user_to_login)
        {
            try
            {
                PostResultObject<TokenAccess> model = new PostResultObject<TokenAccess>();
                
                var login_error = _udb.GetLoginError(user_to_login.user_login);
                var max_login_error = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("max_login_error"));
                if (login_error.objek.sum >= max_login_error && login_error.objek.update_date.AddHours(1) > DateTime.UtcNow)
                {
                    model.message = "You have exceeded the maximum number of login attempts. Please try one hour later";
                    model.objek = null;
                    model.result = false;
                    return Ok(model);
                }
                var result = _udb.GetUserLoginInfo(user_to_login.user_login);
                if (result.result)
                {
                    if ((!result.objek.is_active || result.objek.active_date < DateTime.UtcNow) && (result.objek.login_type == LoginType.client || result.objek.login_type == LoginType.User))
                    {
                        model.message = "Account not active";
                        model.objek = null;
                        model.result = false;
                        return Ok(model);
                    }

                    var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                    var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                    KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                    var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(user_to_login.pass_login), key_pair_certificate);
                    string pass_plain = Encoding.UTF8.GetString(pass_decrypt);
                    bool check_pass = Hashing.ValidatePasswordHash(pass_plain, result.objek.password_login);
                    if (!check_pass)
                    {
                        _udb.PostLoginError(login_error.objek.sum + 1, user_to_login.user_login);
                        model.message = "Check user name or password";
                        model.objek = null;
                        model.result = false;
                        return Ok(model);
                    }

                    var expiry = DateTime.UtcNow.AddHours(8.00);
                    var jwtObj = new UserToken()
                    {
                        id_user_info = result.objek.id_user_info,
                        login_type = result.objek.login_type,
                        user_login = user_to_login.user_login,
                        expiry = expiry,
                        is_active = result.objek.is_active,
                        type_user = (TypeUser)result.objek.login_type,
                        id_user_datas = result.objek.id_user_info,
                        Id = result.objek.id_user_info.ToString(),
                        Username = user_to_login.user_login,
                        Expiry = expiry,
                        LoginType = result.objek.login_type,
                        id_subscriber = result.objek.id_subscriber
                    };
                    var token = Jwt.Encode(jwtObj, Symmetric.JWTKEY);
                    var tokendatas = new TokenAccess()
                    {
                        token = token
                    };

                    UserLoginDatas user_login = new UserLoginDatas() { id = Guid.NewGuid(), expiry = expiry, token = tokendatas.token, user_name = user_to_login.user_login, last_login = DateTime.UtcNow };
                    _udb.PostUserLoginDatas(user_login);

                    _udb.PostLoginError(0, user_to_login.user_login);
                    model.message = "Success to login";
                    model.objek = tokendatas;
                    model.result = true;
                    return Ok(model);
                }
                else
                {
                    model.message = "Check user name or password";
                    model.objek = null;
                    model.result = false;
                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list"), ResponseType(typeof(GetResultData<List<UserInfo>>))]
        public IHttpActionResult GetListUser(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserInfo>> models = this._udb.Get(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlah(search);
                    GetResultData<List<UserInfo>> result = new GetResultData<List<UserInfo>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpPut, Route("api/user/update_status"), ResponseType(typeof(PostResult))]
        public IHttpActionResult UpdateStatus(User model)
        {
            try
            {
                bool result = _udb.UpdateStatus(model);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("bad request");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_user, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get"), ResponseType(typeof(GetResultData<User>))]
        public IHttpActionResult GetUser(string id_user_info)
        {
            try
            {
                GetResultObject<User> models = this._udb.GetUser(Guid.Parse(id_user_info));
                if (models.result)
                {
                    return Ok(models);
                }
                else
                {
                    return BadRequest(models.message);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_log"), ResponseType(typeof(GetResultData<List<UserLog>>))]
        public IHttpActionResult GetListUserLog(int start, int length, int kolom, string order_by, string search, string from_date, string to_date)
        {
            try
            {
                GetResultObject<List<UserLog>> models = this._udb.GetLog(start, length, kolom, order_by, search, from_date, to_date);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahLog(search, from_date, to_date);
                    GetResultData<List<UserLog>> result = new GetResultData<List<UserLog>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpPost, Route("api/user/update_log_status")]
        public IHttpActionResult UpdateUserLogStatus(UserLog model)
        {
            try
            {
                if (_udb.UpdateUserLogStatus(model))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_user, Const.akun_super_admin })]
        [HttpPost, Route("api/user/post_log")]
        public IHttpActionResult PostUserLog(UserLog model)
        {
            try
            {
                model.id = Guid.NewGuid();
                if (_udb.PostUserLog(model))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_user, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_user_login")]
        public IHttpActionResult GetUserLogin(string user_name)
        {
            try
            {
                string authenticationToken = HttpContext.Current.Response.Headers.Get("authenticationToken");
                bool result = _udb.GetUserLogin(user_name, authenticationToken);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_user, Const.akun_super_admin })]
        [HttpPut, Route("api/user/update")]
        public IHttpActionResult Update(UserInfo model)
        {
            try
            {
                bool result = _udb.Update(model);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("bad request");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_user, Const.akun_super_admin })]
        [HttpPut, Route("api/user/change_password")]
        public IHttpActionResult ChangePassword(UserToChangePassword model)
        {
            try
            {
                PostResult pr = new PostResult();
                var result = _udb.GetUserLoginInfo(model.user_login);
                if (result.result)
                {

                    var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                    var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                    KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                    var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.pass_login), key_pair_certificate);
                    string pass_plain = Encoding.UTF8.GetString(pass_decrypt);
                    bool check_pass = Hashing.ValidatePasswordHash(pass_plain, result.objek.password_login);
                    if (!check_pass)
                    {
                        pr.message = "Check password";
                        pr.result = false;
                        return Ok(pr);
                    }

                    var new_pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.new_pass_login), key_pair_certificate);
                    string new_pass_plain = Encoding.UTF8.GetString(new_pass_decrypt);
                    var password_login = Hashing.PasswordHash(new_pass_plain);

                    UserLoginInfo user_login_info = new UserLoginInfo() {id_user_info = model.id_user_info, password_login = password_login, update_date = DateTime.UtcNow };
                    bool result_update = _udb.UpdatePassword(user_login_info);
                    if (result_update)
                    {
                        pr.message = "Change password sucess";
                        pr.result = true;
                        return Ok(pr);
                    }
                    else
                    {
                        pr.message = "Change password fail";
                        pr.result = false;
                        return Ok(pr);
                    }
                }
                else
                {
                    pr.message = "Change password fail";
                    pr.result = false;
                    return Ok(pr);
                }
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_super_admin })]
        [HttpPost, Route("api/user/post_user_access")]
        public IHttpActionResult PostUserAccess(UserAccess model)
        {
            try
            {
                if (_udb.PostUserAccess(model))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_user_access"), ResponseType(typeof(GetResultData<List<UserAccess>>))]
        public IHttpActionResult GetListUserAccess(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserAccess>> models = this._udb.GetUserAccess(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahTotalUserAccess(search);
                    GetResultData<List<UserAccess>> result = new GetResultData<List<UserAccess>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_user_access_ht"), ResponseType(typeof(GetResultData<List<UserAccess>>))]
        public IHttpActionResult GetListUserAccessHt(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserAccess>> models = this._udb.GetUserAccessHt(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahTotalUserAccessHt(search);
                    GetResultData<List<UserAccess>> result = new GetResultData<List<UserAccess>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_user, Const.akun_super_admin })]
        [HttpPut, Route("api/user/min_user_access")]
        public IHttpActionResult MinUserAccess(UserAccess model)
        {
            try
            {
                if (_udb.MinUserAccess(model))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_user, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_user_info_access"), ResponseType(typeof(UserInfoAccess))]
        public IHttpActionResult GetUserInfoAccess(Guid id_user_info, TypePackage package)
        {
            try
            {
                UserInfoAccess models = this._udb.GetUserInfoAccess(id_user_info, package);
                if (models != null)
                {
                    return Ok(models);
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_admin"), ResponseType(typeof(GetResultData<List<UserInfo>>))]
        public IHttpActionResult GetListUserAdmin(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserInfo>> models = this._udb.GetAdmin(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahAdmin(search);
                    GetResultData<List<UserInfo>> result = new GetResultData<List<UserInfo>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin}), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post_admin")]
        public IHttpActionResult PostAdmin(User model)
        {
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.user_login_info.password_login), key_pair_certificate);
                string pass_plain = Encoding.UTF8.GetString(pass_decrypt);
                model.user_login_info.password_login = Hashing.PasswordHash(pass_plain);
                var result = _udb.PostAdmin(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin })]
        [HttpDelete, Route("api/user/delete_admin"), ResponseType(typeof(PostResult))]
        public IHttpActionResult DeleteAdmin(string id_user_info)
        {
            try
            {
                bool result = _udb.DeleteUserAdmin(Guid.Parse(id_user_info));
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("bad request");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpDelete, Route("api/user/delete"), ResponseType(typeof(PostResult))]
        public IHttpActionResult DeleteUser(string id_user_info)
        {
            try
            {
                bool result = _udb.DeleteUser(Guid.Parse(id_user_info));
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("bad request");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet, Route("api/user/get_sum_dash"), ResponseType(typeof(GetResultObject<SumDash>))]
        public IHttpActionResult GetSumDash()
        {
            try
            {
                GetResultObject<SumDash> result = new GetResultObject<SumDash>();
                SumDash sum_dash = new SumDash();
                var models = this._udb.GetSumStatus();
                sum_dash.sum_status = models.objek;
                var sum_per_weeks = this._udb.GetSumWeeks();
                sum_dash.list_sum_per_weeks = sum_per_weeks.objek;
                result.objek = sum_dash;
                result.result = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin, Const.akun_statis })]
        [HttpPost, Route("api/user/post_log_admin")]
        public IHttpActionResult PostUserLogAdmin(UserLogAdmin model)
        {
            try
            {
                model.id = Guid.NewGuid();
                if (_udb.PostUserLogAdmin(model))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Bad Request");
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_log_admin"), ResponseType(typeof(GetResultData<List<UserLogAdmin>>))]
        public IHttpActionResult GetListUserLogAdmin(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserLogAdmin>> models = this._udb.GetLogAdmin(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahLogAdmin(search);
                    GetResultData<List<UserLogAdmin>> result = new GetResultData<List<UserLogAdmin>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet, Route("api/user/get_last_login"), ResponseType(typeof(GetResultData<List<UserLoginDatas>>))]
        public IHttpActionResult GetListUserLastLogin(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserLoginDatas>> models = this._udb.GetUserLastLogin(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahLastLogin(search);
                    GetResultData<List<UserLoginDatas>> result = new GetResultData<List<UserLoginDatas>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_user, Const.akun_super_admin, Const.akun_statis }), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post_cert_revoke")]
        public IHttpActionResult PostRevokeCert(UserCertRevoke model)
        {
            try
            {
                var result = _udb.PostUserCertRevoke(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin }), ResponseType(typeof(PostResult))]
        [HttpGet, Route("api/user/update_status_cert_revoke")]
        public IHttpActionResult UpdateStatusRevokeCert(string email)
        {
            try
            {
                var result = _udb.UpdateStatusUserCertRevoke(email);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_cert_revoke"), ResponseType(typeof(GetResultData<List<UserCertRevoke>>))]
        public IHttpActionResult GetListCertRevoke(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserCertRevoke>> models = this._udb.GetUserCertRevoke(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahCertRevoke(search);
                    GetResultData<List<UserCertRevoke>> result = new GetResultData<List<UserCertRevoke>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_user, Const.akun_super_admin }), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post_cert_rekeying")]
        public IHttpActionResult PostRekeyingCert(UserCertRevoke model)
        {
            try
            {
                var result = _udb.PostUserCertRekeying(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin }), ResponseType(typeof(PostResult))]
        [HttpGet, Route("api/user/update_status_cert_rekeying")]
        public IHttpActionResult UpdateStatusRekeyingCert(string email)
        {
            try
            {
                var result = _udb.UpdateStatusUserCertRekeying(email);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet, Route("api/user/get_list_cert_rekeying"), ResponseType(typeof(GetResultData<List<UserCertRevoke>>))]
        public IHttpActionResult GetListCertRekeying(int start, int length, int kolom, string order_by, string search)
        {
            try
            {
                GetResultObject<List<UserCertRevoke>> models = this._udb.GetUserCertRekeying(start, length, kolom, order_by, search);
                if (models.result)
                {
                    long jumlah = this._udb.GetJumlahCertRekeying(search);
                    GetResultData<List<UserCertRevoke>> result = new GetResultData<List<UserCertRevoke>>() { objek = models.objek, total = jumlah, result = true };
                    return Ok(result);
                }
                else
                {
                    return BadRequest(models.message);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis })]
        [HttpPut, Route("api/user/forgot_password")]
        public IHttpActionResult ForgotPassword(UserToChangePassword model)
        {
            try
            {
                PostResult pr = new PostResult();
                var result = _udb.GetUserLoginInfo(model.user_login);
                if (result.result)
                {

                    var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                    var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                    KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                    
                    var new_pass_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.new_pass_login), key_pair_certificate);
                    string new_pass_plain = Encoding.UTF8.GetString(new_pass_decrypt);
                    var password_login = Hashing.PasswordHash(new_pass_plain);

                    UserLoginInfo user_login_info = new UserLoginInfo() { user_name = model.user_login, password_login = password_login, update_date = DateTime.UtcNow };
                    bool result_update = _udb.UpdatePasswordForgot(user_login_info);
                    if (result_update)
                    {
                        pr.message = "Change password sucess";
                        pr.result = true;
                        return Ok(pr);
                    }
                    else
                    {
                        pr.message = "Change password fail";
                        pr.result = false;
                        return Ok(pr);
                    }
                }
                else
                {
                    pr.message = "Change password fail";
                    pr.result = false;
                    return Ok(pr);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis, Const.akun_admin, Const.akun_super_admin })]
        [HttpPost, Route("api/user/post_valid_token"), ResponseType(typeof(GetResultObject<UserValidToken>))]
        public IHttpActionResult PostUserValidToken(UserValidToken model)
        {
            try
            {
                var secret64Key = "Yfl+WUzC7baYjqPF/BksdW1UBUc=";
                byte[] byte_secret = Convert.FromBase64String(secret64Key);
                string token = HMAC256_Numeric.Generate_OTP(model.user_name, byte_secret);
                model.valid_token = Hashing.PasswordHash(token);
                model.id = Guid.NewGuid();
                model.create_date = DateTime.UtcNow;
                model.active_date = DateTime.UtcNow.AddMinutes(15);
                model.is_active = true;
                
                if (_udb.PostUserValidToken(model))
                {
                    var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                    var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                    KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                    var token_encrypt = SealedPublicKeyBox.Create(Convert.FromBase64String(token), key_pair_certificate);
                    string chiper = Convert.ToBase64String(token_encrypt);
                    UserValidToken send_user_valid_token = new UserValidToken() {valid_token = chiper };
                    GetResultObject<UserValidToken> result = new GetResultObject<UserValidToken>() {result = true, objek = send_user_valid_token };
                    return Ok(result);
                }
                else
                {
                    return BadRequest("bad request");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis, Const.akun_admin, Const.akun_super_admin })]
        [HttpPost, Route("api/user/validate_token"), ResponseType(typeof(PostResult))]
        public IHttpActionResult ValidateToken(UserValidToken model)
        {
            try
            {
                var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                var token_decrypt = SealedPublicKeyBox.Open(Convert.FromBase64String(model.valid_token), key_pair_certificate);
                string plain = string.Empty;
                if(model.type_validate == TypeValidate.server)
                {
                    plain = Convert.ToBase64String(token_decrypt);
                }
                else
                {
                    plain = Encoding.UTF8.GetString(token_decrypt);
                }
                model.valid_token = Hashing.PasswordHash(plain);

                var secret64Key = "Yfl+WUzC7baYjqPF/BksdW1UBUc=";
                byte[] byte_secret = Convert.FromBase64String(secret64Key);
                //var outResult = "";

                //bool validToken = HMAC256_Numeric.Is_OTP_Valid(plain, ref outResult, model.user_name, 15, byte_secret);
                PostResult post_result = new PostResult();
                //if (validToken)
                //{
                    GetResultObject<UserValidToken> user_valid_token = _udb.GetUserValidToken(model.user_name);
                    UserValidToken user_token = user_valid_token.objek;

                    if (!user_token.is_active)
                    {
                        post_result.result = false;
                        post_result.message = "Token Not Active";
                        return Ok(post_result);
                    }

                    if (user_token.active_date < DateTime.UtcNow)
                    {
                        post_result.result = false;
                        post_result.message = "Token Not Active";
                        return Ok(post_result);
                    }

                    if (Hashing.ValidatePasswordHash(plain, user_token.valid_token))
                    {
                        _udb.ChangeIsActiveUserValidToken(model.user_name);
                        post_result.result = true;
                        post_result.message = "Successfull validate token";
                        return Ok(post_result);
                    }
                    else
                    {
                        post_result.result = false;
                        post_result.message = "Token Invalid";
                        return Ok(post_result);
                    }

                //}
                //else
                //{
                //    post_result.result = false;
                //    post_result.message = "Token Invalid";
                //    return Ok(post_result);
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AuthApi(Roles = new[] { Const.akun_client}), ResponseType(typeof(PostResult))]
        [HttpPost, Route("api/user/post_payment")]
        public IHttpActionResult PostPayment(TransactionDetail model)
        {
            try
            {
                var result = _udb.PostUserPayment(model);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis }), ResponseType(typeof(PostResult))]
        [HttpPut, Route("api/user/update_status_payment")]
        public IHttpActionResult UpdateStatusPayment(TransactionDetail model)
        {
            try
            {
                var result = _udb.UpdateStatusUserPayment(model);
                GetResultObject<TransactionDetail> trans_detail = _udb.GetUserPayment(Guid.Parse(model.order_id));
                UserAccess user_access_temp =_udb.GetUserAccessByIdUserPackage(trans_detail.objek.id_user_info, TypePackage.enkripa_sign);

                var price = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings.Get("price"));
                var tax = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("tax"));
                var gross_amount = Convert.ToInt64(model.gross_amount) / (1 + tax);
                var new_sum_access = Convert.ToInt64(gross_amount / price);
                UserAccess user_access = new UserAccess() {id = Guid.NewGuid(), date_update = DateTime.UtcNow, id_user_package = user_access_temp.id_user_package, sum_access = user_access_temp.sum_access + new_sum_access};
                bool x = _udb.PostUserAccess(user_access);
                return Ok(x);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis }), ResponseType(typeof(PostResult))]
        [HttpGet, Route("api/user/check_nik")]
        public IHttpActionResult CheckNik(string nik)
        {
            try
            {
                PostResultValid pr = new PostResultValid();
                var result = _udb.IsExistNik(nik);
                pr.valid = result;
                if (result)
                {
                    pr.message = "NIK/No ID Card is exist";
                    pr.valid = false;
                }
                else
                {
                    pr.message = "";
                    pr.valid = true;
                }
                return Ok(pr);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_statis }), ResponseType(typeof(PostResult))]
        [HttpGet, Route("api/user/check_user_name")]
        public IHttpActionResult CheckUserName(string user_name)
        {
            try
            {
                PostResultValid pr = new PostResultValid();
                var result = _udb.IsExistUserLoginInfo(user_name);
                pr.valid = result;
                if (result)
                {
                    pr.message = "Email is exist";
                    pr.valid = false;
                }
                else
                {
                    pr.message = "";
                    pr.valid = true;
                }
                return Ok(pr);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AuthApi(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_statis, Const.akun_super_admin, Const.akun_user }), ResponseType(typeof(PostResultValid))]
        [HttpGet, Route("api/user/check_badan_usaha")]
        public IHttpActionResult CheckBadanUsaha(string id_user_info)
        {
            try
            {
                PostResultValid pr = new PostResultValid();
                var result = _udb.IsBadanUsaha(Guid.Parse(id_user_info));
                pr.valid = result;
                return Ok(pr);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
