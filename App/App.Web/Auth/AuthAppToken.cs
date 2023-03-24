using Api.Share.Tools;
using Api.Share.User;
using ICKEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Auth
{
    public class AuthAppToken : AuthorizeAttribute
    {
        public new string[] Roles
        {
            get { return base.Roles.Split(','); }
            set { base.Roles = string.Join(",", value); }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var authenticationToken = (string)httpContext.Session[Const.s_token];
                UserToken jwt = Jwt.Decode<UserToken>(authenticationToken, Symmetric.JWTKEY);

                if (Convert.ToDateTime(jwt.expiry) < DateTime.UtcNow)
                {
                    return false;
                }
                else
                {
                    bool result = false;
                    for (int i = 0; i < Roles.Count(); i++)
                    {
                        if (Roles[i] == Const.akun_admin && jwt.login_type == LoginType.admin)
                        {
                            result = true;
                        }
                        else if (Roles[i] == Const.akun_super_admin && jwt.login_type == LoginType.superadmin)
                        {
                            result = true;
                        }
                        else if (Roles[i] == Const.akun_client && jwt.login_type == LoginType.client)
                        {
                            result = true;
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}