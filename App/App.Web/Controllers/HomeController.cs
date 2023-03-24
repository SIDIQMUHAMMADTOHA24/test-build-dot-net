using Api.Share.Tools;
using App.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class HomeController : Controller
    {
        [AuthApp(Roles = new[] { Const.akun_admin, Const.akun_super_admin })]
        public ActionResult Index()
        {
            return View();
        }
    }
}