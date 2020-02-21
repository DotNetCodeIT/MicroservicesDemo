using Newtonsoft.Json;
using ShopWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShopWebSite.Services
{
    public class SupplierService:ISupplierService {
        private readonly HttpClient _httpClient;
  
        public SupplierService(HttpClient httpClient)
        {
            _httpClient = httpClient;
          //  _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_SUPPLIER_ADDRESS"));
        }

        public async Task<Supplier> CreateAsync(Supplier item)
        {
            string uri = $"{_httpClient.BaseAddress}/";

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var response = await _httpClient.PostAsync(uri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                Supplier supplier = JsonConvert.DeserializeObject<Supplier>(await response.Content.ReadAsStringAsync());
                return supplier;
            }
            return null;

        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            string uri = $"{_httpClient.BaseAddress}/";
            var responseString = await _httpClient.GetStringAsync(uri);

            return  JsonConvert.DeserializeObject<List<Supplier>>(responseString);

        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            string uri = $"{_httpClient.BaseAddress}/{id}";
            var responseString = await _httpClient.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<Supplier>(responseString);
        }

        public Task<bool> UpdateAsync(int id, Supplier item)
        {
            throw new NotImplementedException();
        }
    }


}
