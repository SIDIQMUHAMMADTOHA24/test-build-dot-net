using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.ConnectApi
{
    public class ConnectionApi
    {
        public readonly string _BASEURL_API;
        public ConnectionApi(string base_url)
        {
            this._BASEURL_API = base_url;
        }

        public HttpResponseMessage getconnectapi(string addressapi, string token, string nameauth)
        {
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(nameauth, token);
            HttpResponseMessage response = client.GetAsync(ReplaceUrl(addressapi)).Result;
            return response;
        }

        public HttpResponseMessage getconnectapi(string addressapi)
        {
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(ReplaceUrl(addressapi)).Result;
            return response;
        }

        public async Task<HttpResponseMessage> postconnectapi(string addressapi, dynamic objects)
        {
            objects = ReplaceDynamicObject(objects);
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(addressapi, content);
            return response;
        }

        public async Task<HttpResponseMessage> postconnectapi(string addressapi, dynamic objects, string token, string nameauth)
        {
            objects = ReplaceDynamicObject(objects);
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(nameauth, token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(addressapi, content);
            return response;
        }

        public async Task<HttpResponseMessage> postconnectapinoreplace(string addressapi, dynamic objects, string token, string nameauth)
        {
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(nameauth, token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(addressapi, content);
            return response;
        }

        public async Task<HttpResponseMessage> putconnectapi(string addressapi, dynamic objects, string token, string nameauth)
        {
            objects = ReplaceDynamicObject(objects);
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(nameauth, token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(addressapi, content);
            return response;
        }

        public async Task<HttpResponseMessage> putconnectapi(string addressapi, dynamic objects)
        {
            objects = ReplaceDynamicObject(objects);
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(addressapi, content);
            return response;
        }

        public async Task<HttpResponseMessage> deleteconnectapi(string addressapi, string token, string nameauth)
        {

            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(nameauth, token);
            HttpResponseMessage response = await client.DeleteAsync(ReplaceUrl(addressapi));
            return response;
        }

        public async Task<HttpResponseMessage> deleteconnectapi(string addressapi)
        {

            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.DeleteAsync(ReplaceUrl(addressapi));
            return response;
        }

        public async Task<HttpResponseMessage> postconnectapisign(string addressapi, dynamic objects, string token, string nameauth)
        {
            objects = ReplaceDynamicObject(objects);
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation(nameauth, token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(objects), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(addressapi, content);
            return response;
        }

        public HttpResponseMessage getconnectapisign(string addressapi, string token, string nameauth)
        {
            HttpClientHandler hndlr = new HttpClientHandler();
            hndlr.UseDefaultCredentials = true;
            var client = new HttpClient(hndlr) { BaseAddress = new Uri(_BASEURL_API) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation(nameauth, token);
            HttpResponseMessage response = client.GetAsync(ReplaceUrl(addressapi)).Result;
            return response;
        }

        private dynamic ReplaceDynamicObject(dynamic objek)
        {
            string a = JsonConvert.SerializeObject(objek);
            a = a.Replace("'", "''");
            objek = JsonConvert.DeserializeObject(a);
            return objek;
        }

        private string ReplaceUrl(string url)
        {
            string a = url.Replace("'", "''");
            return a;
        }
    }
}
