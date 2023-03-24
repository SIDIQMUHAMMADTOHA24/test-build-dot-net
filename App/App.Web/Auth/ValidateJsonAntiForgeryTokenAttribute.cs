using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace App.Web.Auth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = new JsonAntiForgeryHttpContextWrapper(HttpContext.Current);

            try
            {
                string cookieToken = string.Empty;
                string formToken = string.Empty;
                var tokenHeaders = request.Headers["__RequestVerificationToken"];
                string[] tokens = tokenHeaders.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
                AntiForgery.Validate(cookieToken, formToken);
            }
            catch (HttpAntiForgeryException ex)
            {
                //Append where the request url.
                var msg = string.Format("{0} from {1}", ex.Message, httpContext.Request.Url);
                throw new HttpAntiForgeryException(msg);
            }
        }

        private class JsonAntiForgeryHttpContextWrapper : HttpContextWrapper
        {
            readonly HttpRequestBase _request;
            public JsonAntiForgeryHttpContextWrapper(HttpContext httpContext)
                : base(httpContext)
            {
                _request = new JsonAntiForgeryHttpRequestWrapper(httpContext.Request);
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return _request;
                }
            }
        }

        private class JsonAntiForgeryHttpRequestWrapper : HttpRequestWrapper
        {
            readonly NameValueCollection _form;

            public JsonAntiForgeryHttpRequestWrapper(HttpRequest request)
                : base(request)
            {
                _form = new NameValueCollection(request.Form);
                if (request.Headers["__RequestVerificationToken"] != null)
                {
                    _form["__RequestVerificationToken"] = request.Headers["__RequestVerificationToken"];
                }
            }

            public override NameValueCollection Form
            {
                get
                {
                    return _form;
                }
            }
        }
    }
}