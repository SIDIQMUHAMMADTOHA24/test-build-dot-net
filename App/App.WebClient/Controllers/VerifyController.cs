using Api.Share.Dss;
using Api.Share.Tools;
using Api.Share.User;
using App.ConnectApi.Dss;
using App.ConnectApi.ThisApi;
using App.WebClient.Auth;
using App.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Api.Share.Tools.Result;

namespace App.WebClient.Controllers
{
    public class VerifyController : Controller
    {
        private readonly UserApi _ua;
        private readonly SignVer sign_ver;
        public VerifyController()
        {
            this._ua = new UserApi(Const._auth_name);
            this.sign_ver = new SignVer(Const._authorization);
        }
        // GET: Verify
        public ActionResult Process()
        {
            return View();
        }

        [ValidateJsonAntiForgeryTokenAttribute]
        [HttpPost]
        public async Task<JsonResult> ProcessData(VerifyFile model)
        {
            MsgResponse<VerifyData> verify_data = new MsgResponse<VerifyData>();
            try
            {
                if (!CheckHeaderFile.ProcessPdf(model.base64data))
                {
                    verify_data.success = false;
                    verify_data.message = "Check data type";
                    return Json(verify_data, JsonRequestBehavior.AllowGet);
                }
                UserToLogin login = new UserToLogin() { user_login = Const.user_client_register, pass_login = Const.pass_client_register };
                PostResultObject<TokenAccess> token = await _ua.Login(login);
                verify_data = await sign_ver.Verify(model, token.objek.token);
                return Json(verify_data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                verify_data.success = false;
                verify_data.message = ex.Message.ToString();
                return Json(verify_data, JsonRequestBehavior.AllowGet);
            }
        }
    }
}