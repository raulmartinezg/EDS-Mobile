using Movil.Models;
using Movil.Models.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.DataSQL.SQLiteDataIncidencia))]
namespace Movil.DataSQL
{
    public class SQLiteDataIncidencia : IDataStore<Incidencia>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataIncidencia(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataIncidencia()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Incidencia item)
        {
            return (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<Incidencia> items)
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

        public async Task<bool> zDeleteItemAsync(Incidencia item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Incidencia> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Incidencia>().Where(x => x.id == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Incidencia>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<Incidencia>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(Incidencia item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}
