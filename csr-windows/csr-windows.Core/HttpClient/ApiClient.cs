using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace csr_windows.Core
{
    public sealed class ApiClient
    {
        private const string NoteMessage = "出了点问题，请再次点击";
        // 私有静态变量用于保存类的唯一实例
        private static readonly Lazy<ApiClient> lazyInstance = new Lazy<ApiClient>(() => new ApiClient());
        /// <summary>
        /// Server域名
        /// </summary>
        public string ServerUrl;

        // 私有构造函数，防止外部实例化
        private ApiClient()
        {
            _httpClient = new HttpClient();
        }

        // 公共静态属性提供对类唯一实例的访问
        public static ApiClient Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        // 私有的HttpClient实例
        private readonly HttpClient _httpClient;

        // 设置Token的方法
        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("x-token", token);
        }

        // GET请求方法
        public async Task<string> GetAsync(string url)
        {
            url = ServerUrl + url;
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine(response.StatusCode);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(NoteMessage, MessengerConstMessage.OpenPromptNoteMessageToken);
                return string.Empty;
            }
            return await response.Content.ReadAsStringAsync();
        }

        // POST请求方法
        public async Task<string> PostAsync(string url, HttpContent content)
        {
            url = ServerUrl + url;
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            try
            {
                response.EnsureSuccessStatusCode();

            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(NoteMessage, MessengerConstMessage.OpenPromptNoteMessageToken);
                return string.Empty;
            }
            return await response.Content.ReadAsStringAsync();
        }

        // GET请求方法
        public async Task<string> GetAsync(string url, IDictionary<string, string> parameters = null)
        {
            if (parameters != null)
            {
                var query = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url = $"{url}?{query}";
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(NoteMessage, MessengerConstMessage.OpenPromptNoteMessageToken);
                return string.Empty;
            }
            return await response.Content.ReadAsStringAsync();
        }

        // POST请求方法
        public async Task<string> PostAsync(string url, IDictionary<string, string> parameters)
        {
            url = ServerUrl + url;
            var content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(NoteMessage, MessengerConstMessage.OpenPromptNoteMessageToken);
                return string.Empty;
            }
            return await response.Content.ReadAsStringAsync();
        }

        // PUT请求方法
        public async Task<string> PutAsync(string url, IDictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _httpClient.PutAsync(url, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                WeakReferenceMessenger.Default.Send(NoteMessage, MessengerConstMessage.OpenPromptNoteMessageToken);
                return string.Empty;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
