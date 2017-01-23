using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Cake.IISAdministration
{
    public class IISManager
    {
        private ManagerOptions _option;

        public IISManager(ManagerOptions option)
        {
            _option = option;
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        }

        private HttpClient CreateClient(ManagerOptions option)
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
