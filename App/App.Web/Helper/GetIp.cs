using Api.Share.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace App.Web.Helper
{
    public class GetIp
    {
        public static string Process(HttpRequestBase re)
        {
            try
            {
                IPAddress ip;
                bool ValidateIP = IPAddress.TryParse(re.Headers.Get(Const.ip_user), out ip);
                if (ValidateIP)
                {
                    return re.Headers.Get(Const.ip_user);
                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            //string VisitorsIPAddr = string.Empty;
            //if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //{
            //    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //}
            //else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            //{
            //    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            //}
            //return VisitorsIPAddr;

        }
    }
}