using Api.Share.Dss;
using Api.Share.ESignApi;
using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Dss;
using App.ConnectApi.ThisApi;
using App.Web.Auth;
using App.Web.Helper;
using ICKEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Api.Share.Tools.Result;

namespace App.Web.Controllers
{
    [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
    public class CertificateController : Controller
    {
        private readonly CertificateManage _cm;
        private readonly UserApi _ua;
        private readonly ConnectApi.ESignApi.UserApi _uan;
        private readonly SendEmail _send;
        public CertificateController()
        {
            this._cm = new CertificateManage(Const._authorization);
            this._ua = new UserApi(Const._auth_name);
            this._uan = new ConnectApi.ESignApi.UserApi(Const._authorization);
            this._send = new SendEmail();
        }
        // GET: Certificate
        public ActionResult Get()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetData(int draw, int start, int length)
        {
            try
            {
                string search = Request.QueryString["search[value]"].ToString();
                string kolom_order = Request.QueryString["order[0][column]"].ToString();
                string order_by = Request.QueryString["order[0][dir]"].ToString();

                string token = (string)Session[Const.s_token];
                GetList get_list = new GetList() {iDisplayLength = length, iDisplayStart = start, search = search };
                PostResultObject<MsgResponse<MsgResponseData<List<CertificateDss>>>> list_ts_data = await this._cm.List(get_list, token);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek.data.aaData)
                {
                    var actions = string.Format($"<div class=\"dropdown dropdown-inline\"><button type=\"button\" class=\"btn btn-light-primary btn-icon btn-sm\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"ki ki-bold-more-ver\"></i></button><div class=\"dropdown-menu\">");

                    ls.Add(new List<string> { actions, l.username, l.validity_start.AddHours(-7).ToString(Const.format_string_date), l.validity_end.ToString(Const.format_string_date), l.status });
                }
                GetResult<List<string>> hasil = new GetResult<List<string>>() { draw = draw, recordsTotal = list_ts_data.objek.data.iTotalRecords, recordsFiltered = list_ts_data.objek.data.iTotalRecords, data = ls };
                return Json(hasil, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDataByUserName(string user_name)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                PostResultObject<MsgResponse<List<CertificateDssGet>>> list_ts_data = this._cm.ListByUserName(user_name, token);
                return Json(list_ts_data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> Revoke(PostRevoke model)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                PostResult pr = await this._cm.Revoke(model, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Revoke Certificate", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);
                this._ua.UpdateStatusCertRevoke(model.username, token);

                //send email
                string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                string url = string.Format($"{BASEURL_APP}");
                string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripa.png");
                string html = _send.BodyHtml(url, model.username, SendEmail.BodyEmailType.REVOKE_CERT, path_image);
                _send.SelfMail(model.username, "Revoke of EnkripaSign Certificate", html);

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> Renewal(CertificateDss model)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                //create link send to user
                var expiry = DateTime.UtcNow.AddHours(8.00);
                var jwtObj = new UserToken()
                {
                    Username = model.username,
                    Expiry = expiry
                };
                var token_send = HttpUtility.UrlEncode(Jwt.Encode(jwtObj, Symmetric.JWTKEY));
                //send email
                string BASEURL_APP = System.Configuration.ConfigurationManager.AppSettings.Get("BASEURL_APP");
                string url = string.Format($"{BASEURL_APP}Certificate/RekeyingProcess?tk={token_send}");
                string path_image = string.Format($"{BASEURL_APP}assets/media/logos/enkripa.png");
                string html = _send.BodyHtml(url, model.username, SendEmail.BodyEmailType.REKEYING_CERT, path_image);
                var x = _send.SelfMail(model.username, "Rekeying of EnkripaSign Certificate", html);
                PostResult pr = new PostResult();
                if (x)
                {
                    await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Accept Rekeying Certificate For " + model.name, status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);
                    this._ua.UpdateStatusCertRekeying(model.username, token);
                    pr.result = true;
                    pr.message = "Success Send Link Rekeying to Email User";
                }
                else
                {
                    pr.result = false;
                    pr.message = "Unsuccess Send Link Rekeying to Email User";
                }

                return Json(pr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GetResultObject<string> pr = new GetResultObject<string>() { result = false, objek = ex.Message.ToString() };
                return Json(pr, JsonRequestBehavior.AllowGet);
            }
        }
    }
}