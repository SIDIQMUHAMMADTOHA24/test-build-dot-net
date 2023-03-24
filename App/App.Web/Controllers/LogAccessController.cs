using Api.Share.Dss;
using Api.Share.Tools;
using App.ConnectApi.Dss;
using App.Web.Auth;
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
    public class LogAccessController : Controller
    {
        private readonly LogAccess _cm;
        public LogAccessController()
        {
            this._cm = new LogAccess(Const._authorization);
        }
        // GET: LogAccess
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
                GetList get_list = new GetList() { iDisplayLength = length, iDisplayStart = start, search = search };
                PostResultObject<MsgResponse<MsgResponseData<List<Log>>>> list_ts_data = await this._cm.List(get_list, token);
                List<List<string>> ls = new List<List<string>>();
                foreach (var l in list_ts_data.objek.data.aaData)
                {
                    ls.Add(new List<string> { l.username, l.timestamp.AddHours(-7).ToString(Const.format_string_date), l.action});
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
    }
}