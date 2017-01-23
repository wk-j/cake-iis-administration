using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cake.IISAdministration
{
    public class Website
    {
        public string Name { set; get; }
        public string Id { set; get; }
        public string Status { set; get; }
    }

    public class IISManageOptions
    {
        public string Url { set; get; }
        public string Token { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
    }


    public class IISManage
    {
        private IISManageOptions _option;

        public IISManage(IISManageOptions option)
        {
            _option = option;
        }

        public IEnumerable<Website> GetWebsites()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

            var url = _option.Url;
            var userName = _option.UserName;
            var password = _option.Password;
            var token = _option.Token;

            var api = $"{url}/api/webserver/websites";


            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential(userName, password, url)
            };

            var apiClient = new HttpClient(httpClientHandler);
            apiClient.DefaultRequestHeaders.Add("Access-Token", $"Bearer {token}");


            var res = apiClient.GetAsync(api).Result;
            if (res.StatusCode != HttpStatusCode.OK)
            {
                return new List<Website>();
            }

            var sites = JObject.Parse(res.Content.ReadAsStringAsync().Result).GetValue("websites");
            var rs = sites.ToObject<Website[]>();
            return rs;
        }
    }
}
