using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static UnitedAdobeEditor.Views.Pages.ExploreConfigsPage;

namespace UnitedAdobeEditor.Components.Firebase
{
    public class PublicConfigManager
    {
        private readonly static string paginate_format =
            "https://get-public-configs-pff5uateoa-uc.a.run.app/?page={0}&filter={1}&order={2}";

        private readonly static string create_url =
            "https://create-config-pff5uateoa-uc.a.run.app";

        private readonly static string updateStat_url =
            "https://add-stat-pff5uateoa-uc.a.run.app/?id={0}&type={1}";

        private readonly static string getConfig_url =
         "https://get-config-pff5uateoa-uc.a.run.app/?id={0}";

        public static async Task<string> GetAsync(string url, Dictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var item in headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }
            return await JadWebRequest.JadWebRequest.SendAsync(request);
        }
        public static async Task<string> PostAsync(string url,HttpContent content, Dictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            if (content is null) throw new ArgumentNullException(nameof(content));

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            foreach (var item in headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }
            return await JadWebRequest.JadWebRequest.SendAsync(request);
        }
        private static Dictionary<string, string> GetUserHeader()
        {
            return new Dictionary<string, string>()
            {
                { "userId" , FirebaseAuthControl.Instance?.Client?.User?.Uid  ?? "" }
            };
        }
        public static async Task<Pagination?> Paginate(int page,string filter,string order)
        {
            try
            {
                var url = string.Format(paginate_format, page, filter, order);
                Debug
                    .WriteLine(url);
                var response = await GetAsync(url, GetUserHeader());
                Debug
                    .WriteLine(response);
                var pagi = JsonConvert.DeserializeObject<Pagination>(response);
                return pagi;
            }
            catch (Exception)
            {

            }
            return null;
        }

        internal static async Task<(bool success,string url,string error)> CreateConfig(string image_base64, string appType)
        {
            try
            {
                var createData = new
                {
                    appType = appType,
                    image = image_base64
                };
                var response = await PostAsync(create_url,new StringContent(JsonConvert.SerializeObject(createData)), GetUserHeader());
               Debug.WriteLine(response);
                var token = JToken.Parse(response);
                var url = token.Value<string>("url") ?? "";
                var error = token.Value<string>("error") ?? "";
                var success = token.Value<bool>("success");
                return (success,url,error);
            }
            catch (Exception)
            {

            }
            return (false,"","");
        }

        public enum StatType
        {
            download_config, download_ss, run_config
        }
        public enum OrderType
        {
            most_used,
            createdAt,
        }
        public static async Task UpdateStat(string id, StatType type)
        {
            try
            {
                var url = string.Format(updateStat_url, id, type.ToString());
                var response = await GetAsync(url, GetUserHeader());
                Debug.WriteLine(response);
            }
            catch (Exception ex)
            {

            }
        }

        internal static async Task<PublicConfig?> GetConfig(string configId)
        {
            try
            {
                var url = string.Format(getConfig_url, configId);
                var response = await GetAsync(url, GetUserHeader());
                
                return JsonConvert.DeserializeObject<PublicConfig>(response);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
