using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShopWebSite.Services
{
    public class ApiService<T> : IApiService<T>
    {
        private readonly HttpClient _httpClient;
        //private readonly string _remoteServiceBaseUrl;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> CreateAsync(T item)
        {
            string uri = $"{_httpClient.BaseAddress}/";

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(uri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            return default(T);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string uri = $"{_httpClient.BaseAddress}/{id}";

            var response = await _httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string uri = $"{_httpClient.BaseAddress}/";
            var responseString = await _httpClient.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<List<T>>(responseString);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            string uri = $"{_httpClient.BaseAddress}/{id}";
            var responseString = await _httpClient.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public async Task<bool> UpdateAsync(int id, T item)
        {
            string uri = $"{_httpClient.BaseAddress}/{id}";

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PutAsync(uri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
