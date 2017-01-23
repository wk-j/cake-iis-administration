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
    public class Site
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

    public class NewSite
    {
        public string Name { set; get; } = "NewSite";
        public string PhysicalPath { set; get; } = "D:\\IISApplication\\NewSite";
        public int Port { set; get; } = 9010;
    }

    public class Result<T>
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public T Data { set; get; }
    }

    public class Results<T>
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Data { set; get; }
    }


    public class IISManage
    {
        private IISManageOptions _option;

        public IISManage(IISManageOptions option)
        {
            _option = option;
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        }

        private HttpClient CreateClient(IISManageOptions option)
        {
            var url = option.Url;
            var userName = option.UserName;
            var password = option.Password;
            var token = option.Token;


            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential(userName, password, url)
            };

            var apiClient = new HttpClient(httpClientHandler);
            apiClient.DefaultRequestHeaders.Add("Access-Token", $"Bearer {token}");

            return apiClient;
        }

        public Result<Site> CreateSite(NewSite site)
        {
            var apiClient = CreateClient(_option);
            var newSite = new
            {
                name = site.Name,
                physical_path = site.PhysicalPath,
                bindings = new[] {
                    new {
                        protocol = "http",
                        port = site.Port,
                        is_https = false,
                        ip_address = "*"
                    }
                }
            };

            var api = $"{_option.Url}/api/webserver/websites";
            var json = JsonConvert.SerializeObject(newSite);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = apiClient.PostAsync(api, content).Result;
            var resultString = res.Content.ReadAsStringAsync().Result;
            if (res.StatusCode != HttpStatusCode.Created)
            {
                return new Result<Site>
                {
                    Success = false,
                    Message = resultString
                };
            }

            var info = JObject.Parse(resultString);
            var obj = info.ToObject<Site>();

            return new Result<Site>
            {
                Success = true,
                Data = obj
            };
        }


        public Results<Site> GetWebsites()
        {
            var api = $"{_option.Url}/api/webserver/websites";

            var apiClient = CreateClient(_option);

            var res = apiClient.GetAsync(api).Result;
            var resultString = res.Content.ReadAsStringAsync().Result;

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return new Results<Site>
                {
                    Success = false,
                    Message = resultString
                };
            }

            var sites = JObject.Parse(resultString).GetValue("websites");
            var rs = sites.ToObject<Site[]>();

            return new Results<Site>
            {
                Success = true,
                Data = rs
            };
        }
    }
}
