using Api.Share.Dss;
using Api.Share.Ekyc;
using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Dss;
using App.ConnectApi.Ekyc;
using App.ConnectApi.ThisApi;
using App.Web.Auth;
using App.Web.Helper;
using ICKEncryption;
using Newtonsoft.Json;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static Api.Share.Tools.Result;

namespace App.Web.Controllers
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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> PostLogin(UserToLogin login)
        {
            PostResult hasil = new PostResult();
            try
            {
                PostResultObject<TokenAccess> token = await _ua.Login(login);
                if (token.result)
                {
                    UserToken jwt = Jwt.Decode<UserToken>(token.objek.token, Symmetric.JWTKEY);
                    Session[Const.s_user_login] = jwt.user_login;
                    Session[Const.s_nama] = jwt.nama;
                    Session[Const.s_token] = token.objek.token;
                    Session[Const.s_id_user_info] = jwt.id_user_info.ToString();
                    Session[Const.s_login_type] = Convert.ToInt32(jwt.login_type).ToString();

                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = jwt.Username, activity = "Login", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, token.objek.token);

                    hasil = new PostResult() { result = true, message = token.message };

                    //generate token and send email;
                    AuthToken auth_token = new AuthToken();
                    bool result_send_mail = await auth_token.Generate(token.objek.token, jwt.user_login);

                    return Json(hasil, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    UserToLogin login_static = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                    PostResultObject<TokenAccess> token_static = await _ua.Login(login_static);
                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = login.user_login, activity = "Login fail", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, token_static.objek.token);

                    hasil = new PostResult() { result = false, message = token.message };
                    return Json(hasil, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                hasil = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthAppToken(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> ValidateToken(AuthTokenLogin model)
        {
            string token = (string)Session[Const.s_token];
            PostResult hasil = new PostResult();
            try
            {
                AuthToken validate_token = new AuthToken();
                UserValidToken user_valid_token = new UserValidToken() { user_name = (string)Session[Const.s_user_login], valid_token = model.value_token };
                hasil = await validate_token.ProcessValidToken(user_valid_token, token);
                if (hasil.result)
                {
                    Session[Const.s_valid_token] = true;

                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = hasil.message, status = GetIp.Process(Request), time_input = DateTime.UtcNow}, token);
                }
                else
                {
                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = hasil.message, status = GetIp.Process(Request), time_input = DateTime.UtcNow}, token);
                }

                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                hasil = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_client, Const.akun_super_admin })]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            try
            {
                Session.Abandon();
                Session.RemoveAll();
                Session.Clear();
                FormsAuthentication.SignOut();
                PostResult pr = new PostResult() { result = true, message = "Logout berhasil" };

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Logout", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PostResult pr = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }

        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public ActionResult Get()
        {
            return View();
        }

        [AuthApp(Roles = new[] {Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetData(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];
               

                GetResultData<List<UserInfo>> list_ts_data = this._ua.GetList(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {

                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\"><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id}\" onclick=\"ModalUpdateStatus(this.id)\">Update Status</a><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id_ekyc}\" onclick=\"ModalPreviewEKYC(this.id)\">Preview EKYC</a><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id}\" onclick=\"ModalPreviewDetail(this.id)\">Preview Detail</a>");

                    GetResultObject<User> user = _ua.GetData(token, l.id.ToString());
                    var p_ekripa_sign = user.objek.list_user_package.Where(o => o.package == TypePackage.enkripa_sign);

                    if(p_ekripa_sign.Count() != 0)
                    {
                        actions += string.Format($"<a class=\"dropdown-item\" <a href=\"#\" id = \"{l.email_address}\" onclick=\"ModalPreviewCertificate(this.id, '{l.id}', '{l.name}')\">Manage Certificate</a>");
                    }

                    string _status_badge = "success";

                    if (l.status == StatusUserInfo.non_active)
                    {
                        _status_badge = "danger";
                        actions += string.Format($"<a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id}\" onclick=\"ModalDeleteUser(this.id)\">Delete</a>");
                    }
                    else if (l.status == StatusUserInfo.pending)
                    {
                        _status_badge = "warning";
                    }

                    string status = string.Format($"<span class=\"label label-lg font-weight-bold label-light-{_status_badge} label-inline\">{l.status_string}</span>");

                    ls.Add(new List<string> { actions, l.name, l.email_address, l.phone_number, l.input_date.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"), status});
                    
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> UpdateStatus(User model)
        {
            PostResult pr = new PostResult();
            try
            {
                model.user_login_info = new UserLoginInfo();

                model.user_login_info.is_active = false;
                if (model.user_info.status == StatusUserInfo.active)
                {
                    model.user_login_info.is_active = true;
                }

                string token = (string)Session[Const.s_token];
                model.user_info.update_date = DateTime.UtcNow;
                model.user_login_info.update_date = DateTime.UtcNow;
                bool result = await this._ua.UpdateStatus(model, token);
                if (result)
                {
                    GetResultObject<User> user =  _ua.GetData(token, model.user_info.id.ToString());
                    var p_ekripa_sign = user.objek.list_user_package.Where(o => o.package == TypePackage.enkripa_sign);

                    PostResult _post_result = new PostResult() { result = true, message = string.Empty };
                    if (p_ekripa_sign.Count() != 0 && model.user_info.status == StatusUserInfo.active)
                    {
                        var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
                        var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
                        KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
                        var pass_enkrip = SealedPublicKeyBox.Create(Const.password_ejbca, key_pair_certificate);
                        string pass_ejbca_enkrip = Convert.ToBase64String(pass_enkrip);

                        string subject_dn = string.Format($"CN={user.objek.user_info.name} ({model.user_info.id}),OU=Personal,C=ID");
                        string ca = Const.ca;
                        string cp = Const.cp;
                        string ee = Const.ee;
                        PostResultValid check_badan_usaha = _ua.CheckBadanUsaha(model.user_info.id.ToString(), token);
                        if (check_badan_usaha.valid)
                        {
                            subject_dn = string.Format($"CN={user.objek.user_info.name} ({model.user_info.id}),C=ID");
                            cp = Const.cp_bu;
                            ee = Const.ee_bu;
                        }

                        UserCa user_ca = new UserCa()
                        {
                            ca_name = ca,
                            cp_name = cp,
                            ee_name = ee,
                            email = user.objek.user_info.email_address,
                            password_ejbca = pass_ejbca_enkrip,
                            nama = user.objek.user_info.name,
                            subject_dn = subject_dn,
                            username = user.objek.user_info.email_address,
                            telepon = user.objek.user_info.phone_number,
                            kota = "Jakarta",
                            provinsi = "DKI Jakarta"
                        };
                        var add_user_ca = await this._um_ca.AddUser(user_ca, token);
                        _post_result = add_user_ca;
                    }
                    else
                    {
                        _post_result.result = true;
                    }

                    if (_post_result.result)
                    {
                        //send email
                        string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                        string url = string.Format($"{BASEURL_APP}");
                        string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripa.png");
                        string html = _send.BodyHtml(url, user.objek.user_info.email_address, SendEmail.BodyEmailType.ACTIVE_USER, path_image);
                        _send.SelfMail(user.objek.user_info.email_address, "Welcome to Enkripa Product", html);
                    }

                }
                pr = new PostResult() { result = result };

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Update Status User", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = "fail" };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public string GetDataDetail(string id_user_info)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                GetResultObject<User> result = this._ua.GetData(token, id_user_info);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                GetResultObject<User> pr = new GetResultObject<User>() { result = false, message = ex.Message.ToString() };
                return JsonConvert.SerializeObject(pr);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public ActionResult GetLog()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<ActionResult> UpdateUserLog(UserLog model)
        {
            PostResult pr = new PostResult();
            try
            {
                string token = (string)Session[Const.s_token];
                pr = await _ua.UpdateUserLogStatus(model, token);

                if (pr.result)
                {
                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Update user log status", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, token);

                }

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = "fail" };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetDataLog(string from_date, string to_date, int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserLog>> list_ts_data =  this._ua.GetListLog(token, start, length, Convert.ToInt32(kolom_order), order_by, search, from_date, to_date);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    string html = string.Format($"<label class=\"checkbox checkbox-single\"><input type =\"checkbox\" id = \"{l.id}\" class=\"checkable\"/><span></span></label>");
                    if(l.status_log == Const.log_reviewed)
                    {
                        html = string.Empty;
                    }
                    ls.Add(new List<string> { html, l.user_name, l.activity, l.time_input.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.status, l.apps, l.status_log});
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetDataUserAccess(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserAccess>> list_ts_data = this._ua.GetListUserAccess(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\"><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id_user_package}\" onclick=\"ModalUpdateUserAccess(this.id)\">Update User Access</a>");

                    ls.Add(new List<string> { actions, l.user_data_nama, l.sum_access.ToString(), l.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.user_package});
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetDataUserAccessHt(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserAccess>> list_ts_data = this._ua.GetListUserAccessHt(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
               
                    ls.Add(new List<string> { l.user_data_nama, l.sum_access.ToString(), l.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.user_package });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public ActionResult GetUserAccess()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public ActionResult GetUserAccessHt()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpPost]
        public async Task<JsonResult> PostUserAccess(UserAccess model)
        {
            PostResult hasil = new PostResult();
            try
            {
                model.id = Guid.NewGuid();
                model.date_update = DateTime.UtcNow;
                string token = (string)Session[Const.s_token];
                hasil = await _ua.PostUserAccess(model, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Add User Access", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                hasil = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpGet]
        public ActionResult GetAdmin()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetDataAdmin(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];


                GetResultData<List<UserInfo>> list_ts_data = this._ua.GetListAdmin(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {

                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\"><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id}\" onclick=\"ModalUpdateStatus(this.id)\">Update Status</a><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id}\" onclick=\"ModalDeleteAdmin(this.id)\">Delete</a>");

                    string _status_badge = "success";

                    if (l.status == StatusUserInfo.non_active)
                    {
                        _status_badge = "danger";
                    }
                    else if (l.status == StatusUserInfo.pending)
                    {
                        _status_badge = "warning";
                    }

                    string status = string.Format($"<span class=\"label label-lg font-weight-bold label-light-{_status_badge} label-inline\">{l.status_string}</span>");

                    ls.Add(new List<string> { actions, l.name, l.email_address, l.phone_number, l.input_date.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"), status });

                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpGet]
        public ActionResult PostAdmin()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpPost]
        public async Task<JsonResult> PostDataAdmin(ForRegUser model)
        {
            PostResult result = new PostResult();
            try
            {
                    string token = (string)Session[Const.s_token];

                    model.user.user_info.payment = TypePayment.credit_card;
                    model.user.user_info.license = string.Empty;
                    model.user.user_info.id = Guid.NewGuid();
                    model.user.user_info.id_subscriber = model.user.user_info.id;
                    model.user.user_info.input_date = DateTime.UtcNow;
                    model.user.user_info.update_date = DateTime.UtcNow;
                    model.user.user_info.status = StatusUserInfo.active;
                    model.user.user_info.nik = string.Empty;
                    
                    model.user.user_login_info.login_type = LoginType.admin;
                    model.user.user_login_info.is_active = true;
                    model.user.user_login_info.input_date = DateTime.UtcNow;
                    model.user.user_login_info.update_date = DateTime.UtcNow;
                    model.user.user_login_info.active_date = DateTime.UtcNow;
                    model.user.user_login_info.id = Guid.NewGuid();
                    model.user.user_login_info.user_name = model.user.user_info.email_address;

                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Add User Admin", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                    result = await _ua.PostAdmin(model.user, token);
                    
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> UpdateStatusAdmin(User model)
        {
            PostResult pr = new PostResult();
            try
            {
                model.user_login_info = new UserLoginInfo();

                model.user_login_info.is_active = false;
                if (model.user_info.status == StatusUserInfo.active)
                {
                    model.user_login_info.is_active = true;
                }
                string token = (string)Session[Const.s_token];
                model.user_info.update_date = DateTime.UtcNow;
                model.user_login_info.update_date = DateTime.UtcNow;
                bool result = await this._ua.UpdateStatus(model, token);
                pr = new PostResult() { result = result };

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Update Status User Admin", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = "fail" };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> DeleteAdmin(string id_user_info)
        {
            PostResult pr = new PostResult();
            try
            {
                string token = (string)Session[Const.s_token];
                pr = await this._ua.DeleteAdmin(id_user_info, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Delete User Admin", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = "fail" };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> Delete(string id_user_info)
        {
            PostResult pr = new PostResult();
            try
            {
                string token = (string)Session[Const.s_token];
                pr = await this._ua.Delete(id_user_info, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Delete User", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = "fail" };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> ChangePasswordData(UserToChangePassword model)
        {
            PostResult pr = new PostResult();
            try
            {
                string token = (string)Session[Const.s_token];
                string id_user_info = (string)Session[Const.s_id_user_info];
                string user_name = (string)Session[Const.s_user_login];
                model.id_user_info = Guid.Parse(id_user_info);
                model.user_login = user_name;
                pr = await _ua.ChangePassword(model, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Change Password", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new PostResult() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public JsonResult GetSumStatus()
        {
            GetResultObject<SumDash> pr = new GetResultObject<SumDash>();
            try
            {
                string token = (string)Session[Const.s_token];
                pr = _ua.GetSumStatus(token);
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                pr = new GetResultObject<SumDash>() { result = false, message = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpGet]
        public ActionResult GetLogAdmin()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin })]
        [HttpGet]
        public JsonResult GetDataLogAdmin(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserLogAdmin>> list_ts_data = this._ua.GetListLogAdmin(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    ls.Add(new List<string> { l.user_name, l.activity, l.time_input.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.status });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public ActionResult GetLastLogin()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public JsonResult GetDataLastLogin(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserLoginDatas>> list_ts_data = this._ua.GetLastLogin(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    ls.Add(new List<string> { l.user_name, l.last_login.ToString("yyyy-MM-dd HH:mm:ss.fff") });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public ActionResult GetCertRevoke()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public JsonResult GetDataCertRevoke(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserCertRevoke>> list_ts_data = this._ua.GetListCertRevoke(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\"><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id_ekyc}\" onclick=\"ModalPreviewEKYC(this.id)\">Preview EKYC</a><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.user_info.email_address}\" onclick=\"ModalPreviewCertificate(this.id)\">Manage Certificate</a>");

                    actions += string.Format($"");

                    string _status_badge = "info";

                    if (l.status == StatusUserCertRevoke.revoke)
                    {
                        _status_badge = "danger";
                    }
                   
                    string status = string.Format($"<span class=\"label label-lg font-weight-bold label-light-{_status_badge} label-inline\">{l.status_value}</span>");

                    ls.Add(new List<string> { actions, l.user_info.name, l.user_info.email_address, l.user_info.phone_number, l.reason, l.date_input.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"), status });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public ActionResult GetCertRekeying()
        {
            return View();
        }

        [AuthApp(Roles = new[] { Const.akun_super_admin, Const.akun_admin })]
        [HttpGet]
        public JsonResult GetDataCertRekeying(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];

                GetResultData<List<UserCertRevoke>> list_ts_data = this._ua.GetListCertRekeying(token, start, length, Convert.ToInt32(kolom_order), order_by, search);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek)
                {
                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\"><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.id_ekyc}\" onclick=\"ModalPreviewEKYC(this.id)\">Preview EKYC</a><a class=\"dropdown-item\" <a href=\"#\" id = \"{l.user_info.email_address}\" onclick=\"ModalPreviewCertificate(this.id, '{l.id_user_info}', '{l.user_info.name}')\">Manage Certificate</a>");

                    actions += string.Format($"");

                    string _status_badge = "info";
                    string status_value = "Req Rekeying";

                    if (l.status == StatusUserCertRevoke.revoke)
                    {
                        _status_badge = "danger";
                        status_value = "Rekeying";
                    }

                    string status = string.Format($"<span class=\"label label-lg font-weight-bold label-light-{_status_badge} label-inline\">{status_value}</span>");

                    ls.Add(new List<string> { actions, l.user_info.name, l.user_info.email_address, l.user_info.phone_number, l.date_input.ToString("yyyy-MM-dd HH:mm:ss.fff"), l.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"), status });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.total, recordsFiltered = list_ts_data.total, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

    }
}