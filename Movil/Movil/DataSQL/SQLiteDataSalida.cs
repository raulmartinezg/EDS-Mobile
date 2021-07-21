using Movil.Models;
using Movil.Models.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.DataSQL.SQLiteDataSalida))]
namespace Movil.DataSQL
{
    public class SQLiteDataSalida : IDataStore<Salida>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataSalida(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataSalida()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Salida item)
        {
            return (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<Salida> items)
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

        public async Task<bool> zDeleteItemAsync(Salida item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Salida> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Salida>().Where(x => x.Id == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<Salida> zGetItemClaveConcesionarioAsync(int ClaveConcesionario)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Salida>().Where(x => x.ClaveConcesionario == ClaveConcesionario)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Salida>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<Salida>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(Salida item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}