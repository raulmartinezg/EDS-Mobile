using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Movil
{
    public class Repository<T> where T : class, new()
    {
        private readonly ISQLitePlatform _platform;
        public Repository(ISQLitePlatform platform)
        {
            _platform = platform;
            var con = _platform.mGetConnection();
            con.Close();
        }
        public Repository()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
            var con = _platform.mGetConnection();
            con.Close();
        }

        public async Task<bool> AddItemAsync(T item)
        {
            return (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        public async Task<bool> DeleteItemAsync(T item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<T>().ToListAsync();
        }
    }
}
