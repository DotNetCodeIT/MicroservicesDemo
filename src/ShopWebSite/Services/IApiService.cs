using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebSite.Services
{
    public interface IApiService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> CreateAsync(T item);

        Task<bool> UpdateAsync(int id, T item);
        
        Task<bool> DeleteAsync(int id);


    }
}
