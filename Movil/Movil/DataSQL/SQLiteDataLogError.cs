using Movil.Models;
using Movil.Models.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.DataSQL.SQLiteDataLogError))]
namespace Movil.DataSQL
{
    public class SQLiteDataLogError : IDataStore<LogError>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataLogError(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataLogError()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(LogError item)
        {
            return (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<LogError> items)
        {
            try
            {
                await _platform.mGetConnectionAsync().InsertAllAsync(items);

                return new MensageGenerico(true, "OK");
            }
            catch (Exception err)
            {
                return new MensageGenerico(false, err.Message);
            }
        }

        public async Task<bool> zDeleteItemAsync(LogError item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<LogError> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<LogError>().Where(x => x.id == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<LogError>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<LogError>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(LogError item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}
