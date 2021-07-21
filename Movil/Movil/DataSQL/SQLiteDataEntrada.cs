using Movil.Models;
using Movil.Models.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.DataSQL.SQLiteDataEntrada))]
namespace Movil.DataSQL
{
    public class SQLiteDataEntrada : IDataStore<Entrada>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataEntrada(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataEntrada()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Entrada item)
        {
            return (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<Entrada> items)
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

        public async Task<bool> zDeleteItemAsync(Entrada item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Entrada> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Entrada>().Where(x => x.Id == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<Entrada> zGetItemClaveConcesionarioAsync(int ClaveConcesionario)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Entrada>().Where(x => x.ClaveConcesionario == ClaveConcesionario)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Entrada>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<Entrada>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(Entrada item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}
