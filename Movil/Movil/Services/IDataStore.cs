using Movil.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movil
{
    public interface IDataStore<T>
    {
        Task<bool> zAddItemAsync(T item);

        Task<MensageGenerico> zAddItemsAsync(List<T> items);
        Task<bool> zUpdateItemAsync(T item);
        Task<bool> zDeleteItemAsync(T item);
        Task<T> zGetItemAsync(int id);
        //Task<T> zGetItemAsync(string id);
        Task<IEnumerable<T>> zGetItemsAsync();
    }
}
