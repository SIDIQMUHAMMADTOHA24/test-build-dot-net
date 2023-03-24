using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Dss;
using App.ConnectApi.ThisApi;
using App.Web.Auth;
using App.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Api.Share.Tools.Result;

namespace App.Web.Controllers
{
    [AuthApp(Roles = new[] { Const.akun_super_admin })]
    public class CertificateRevokeController : Controller
    {
        private readonly CertificateManage _cm;
        private readonly UserApi _ua;
        public CertificateRevokeController()
        {
            this._cm = new CertificateManage(Const._authorization);
            this._ua = new UserApi(Const._auth_name);
        }

        [HttpGet]
        public ActionResult Get()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> RevokeAll()
        {
            try
            {
                string token = (string)Session[Const.s_token];
                PostResult pr = await this._cm.RevokeAll(token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Revoke All Certificate", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);
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