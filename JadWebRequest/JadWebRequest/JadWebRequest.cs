using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace JadWebRequest
{
    public class JadWebRequest
    {
        private static readonly HttpClient _httpClient = new();

        private static void WriteExceptions(Exception? ex)
        {
            while (ex is not null)
            {
                Debug.WriteLine($"[JadHttp] Exceptions: {ex.Message}");
                ex = ex.InnerException;
            }
        }
        public static string Send(HttpRequestMessage requestMessage)
        {
            if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

            try
            {
                var response = _httpClient.Send(requestMessage);
                using var reader = new StreamReader(response.Content.ReadAsStream());
                return reader.ReadToEnd();
            }
            catch (HttpRequestException ex)
            {
                WriteExceptions(ex);
            }
            catch (Exception ex)
            {
                WriteExceptions(ex);
            }
            return string.Empty;
        }
        public static async Task<string> SendAsync(HttpRequestMessage requestMessage)
        {
            if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);
                using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
                return await reader.ReadToEndAsync();
            }
            catch (HttpRequestException ex)
            {
                WriteExceptions(ex);
            }
            catch (Exception ex)
            {
                WriteExceptions(ex);
            }
            return string.Empty;
        }

        public static HttpClient GetClient() => _httpClient;

        public static string Get(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            return Send(request);
        }
        public static async Task<string> GetAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await SendAsync(request);
        }
        public static async Task<string> PostAsync(string url, HttpContent content)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            if (content is null) throw new ArgumentNullException(nameof(content));

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            return await SendAsync(request);
        }
        public static string Post(string url, HttpContent content)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            if (content is null) throw new ArgumentNullException(nameof(content));

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            return Send(request);
        }
    }
}