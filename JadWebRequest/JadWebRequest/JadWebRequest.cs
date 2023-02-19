using System.Net;
using System.Text;

namespace JadWebRequest
{
    public class JadWebRequest
    {
        private static System.Net.WebRequest Create(string url)
        {
            return System.Net.WebRequest.Create(url);
        }
        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)JadWebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)JadWebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)(request.GetResponse()))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static async Task<string> PostAsync(string uri, StringContent data1, string method = "POST", string contentType = "application/json")
        {
            var request = (HttpWebRequest)JadWebRequest.Create(uri);

            var postData = data1;
            var data = Encoding.ASCII.GetBytes(postData.ToString());

            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)(await request.GetResponseAsync());
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;

        }
        public static string Post(string uri, StringContent data1, string method = "POST", string contentType = "application/json")
        {
            var request = (HttpWebRequest)JadWebRequest.Create(uri);

            var postData = data1;
            var data = Encoding.ASCII.GetBytes(postData.ToString());

            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)(request.GetResponse());
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;

        }


    }
    public partial class JadWebRequest1
    {
        public bool IsGetting { get; private set; } = false;

        public async Task<string> GetAsync(string uri)
        {
            IsGetting = true;
            var result = await JadWebRequest.GetAsync(uri);
            IsGetting = false;
            return result;
        }
        public string Get(string uri)
        {
            IsGetting = true;
            var result = JadWebRequest.Get(uri);
            IsGetting = false;
            return result;
        }
        public async Task<string> PostAsync(string uri, StringContent data1, string method = "POST", string contentType = "application/json")
        {

            IsGetting = true;
            var result = await JadWebRequest.PostAsync(uri, data1, method, contentType);
            IsGetting = false;
            return result;

        }
        public string Post(string uri, StringContent data1, string method = "POST", string contentType = "application/json")
        {
            IsGetting = true;
            var result = JadWebRequest.Post(uri, data1, method, contentType);
            IsGetting = false;
            return result;
        }

    }
}