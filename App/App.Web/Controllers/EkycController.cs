using Api.Share.Ekyc;
using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Ekyc;
using App.ConnectApi.ThisApi;
using App.Web.Auth;
using App.Web.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Api.Share.Tools.Result;

namespace App.Web.Controllers
{
    public class EkycController : Controller
    {
        private readonly Verification _ea;
        private readonly Ra _ra;
        private readonly UserApi _ua;
        public EkycController()
        {
            this._ea = new Verification(Const._auth_name);
            this._ra = new Ra(Const._auth_name);
            this._ua = new UserApi(Const._auth_name);
        }

        [HttpGet]
        public JsonResult GetRandomQuestion()
        {
            try
            {
                RandomQuestion result = this._ea.GetRandomQuestion();
                RandomQuestion rq = new RandomQuestion() { id = result.id, question = result.question };
                return Json(rq, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public string GetData(string id_ekyc)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                GetResultObject<VideoAndQuestion> result = this._ea.GetData(token, id_ekyc);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                GetResultObject<VideoAndQuestion> pr = new GetResultObject<VideoAndQuestion>() { result = false, message = ex.Message.ToString() };
                return JsonConvert.SerializeObject(pr);
            }
        }

        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpGet]
        public ActionResult RaVerification(string id_ekyc)
        {
            ViewBag.id_ekyc = id_ekyc;
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpPost]
        public async Task<string> PostRaVerification(string id_ekyc, BodyARI model)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                PostResultObject<ResponseARI> result = await this._ra.Verification(model, token);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                PostResultObject<ResponseARI> pr = new PostResultObject<ResponseARI>() { result = false, message = ex.Message.ToString() };
                return JsonConvert.SerializeObject(pr);
            }
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        [HttpPost]
        public async Task<string> PostVerifManual(InputVerifManual model)
        {
            try
            {
                string token = (string)Session[Const.s_token];
                PostResult result = await this._ea.PostVerifManual(model, token);

                await _ua.PostUserLogAdmin(new UserLogAdmin() { user_name = (string)Session[Const.s_user_login], activity = "Upload Manual Verification", status = GetIp.Process(Request), time_input = DateTime.UtcNow }, (string)Session[Const.s_token]);

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                PostResultObject<ResponseARI> pr = new PostResultObject<ResponseARI>() { result = false, message = ex.Message.ToString() };
                return JsonConvert.SerializeObject(pr);
            }
        }
    }
}