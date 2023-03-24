using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Api.DpApi
{
    public class Get
    {
        private readonly string _BASEURL_API;
        public Get(string baseurl_api_user)
        {
            this._BASEURL_API = baseurl_api_user;
        }

        public string GetPrivateKey()
        {
            string datas = string.Empty;
            try
            {
                string url_api = string.Format($"/api/user/get_pk");
                HttpResponseMessage response = getconnectapi(url_api);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    datas = JsonConvert.DeserializeObject<string>(data);
                    return datas;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private HttpResponseMessage getconnectapi(string addressapi)
        {
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(addressapi).Result;
            return response;
        }
    }
}
