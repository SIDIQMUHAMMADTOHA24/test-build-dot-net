using Api.Share;
using Api.Share.Tools;
using Api.Share.User;
using ICKEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Api.Web.Auth
{
    public class AuthApi : AuthorizeAttribute
    {
        public new string[] Roles
        {
            get { return base.Roles.Split(','); }
            set { base.Roles = string.Join(",", value); }
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var headerAuth = actionContext.Request.Headers.GetValues("authenticationToken");
                var authenticationToken = Convert.ToString(headerAuth.FirstOrDefault());
                UserToken jwt = Jwt.Decode<UserToken>(authenticationToken, Symmetric.JWTKEY);
                if (Convert.ToDateTime(jwt.expiry) < DateTime.UtcNow)
                {
                    return Unauthorized(actionContext, authenticationToken);
                }
                else
                {
                    var result = false;
                    for (int i = 0; i < Roles.Count(); i++)
                    {
                        if (Roles[i] == Const.akun_admin && jwt.login_type == LoginType.admin)
                        {
                            result = Authorized(authenticationToken);
                        }
                        else if (Roles[i] == Const.akun_super_admin && jwt.login_type == LoginType.superadmin)
                        {
                            result = Authorized(authenticationToken);
                        }
                        else if (Roles[i] == Const.akun_client && jwt.login_type == LoginType.client)
                        {
                            result = Authorized(authenticationToken);
                        }
                        else if (Roles[i] == Const.akun_statis && jwt.login_type == LoginType.statis)
                        {
                            result = Authorized(authenticationToken);
                        }
                        else if (Roles[i] == Const.akun_user && jwt.login_type == LoginType.User)
                        {
                            result = Authorized(authenticationToken);
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return Unauthorized(actionContext, "No Token");
            }
        }

        private static bool Authorized(string authenticationToken)
        {
            HttpContext.Current.Response.AddHeader("authenticationToken", authenticationToken);
            HttpContext.Current.Response.AddHeader("AuthenticationStatus", "Authorized");
            return true;
        }

        private static bool Unauthorized(HttpActionContext actionContext, string authenticationToken)
        {
            HttpContext.Current.Response.AddHeader("authenticationToken", authenticationToken);
            HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            return false;
        }
    }
}