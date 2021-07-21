using Movil.Helpers;
using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataConcesionario))]
namespace Movil
{
    public class SQLiteDataConcesionario : ObservableRangeCollection<Concesionario>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataConcesionario(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataConcesionario()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Concesionario item)
        {
            bool Resp = false;

            Concesionario findbjt = await zGetItemAsync(item.ClaveConcesionario);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<Concesionario> items)
        {
            try
            {
                foreach (var xitem in items)
                {
                    await zAddItemAsync(xitem);
                }

                return new MensageGenerico(true, "OK");
            }
            catch (Exception err)
            {
                return new MensageGenerico(false, err.Message);
            }
        }

        public async Task<bool> zDeleteItemAsync(Concesionario item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Concesionario> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Concesionario>().Where(x => x.ClaveConcesionario == id)
                     .FirstOrDefaultAsync();
        }

        public Concesionario zGetItem(int id)
        {
            return _platform.mGetConnection()
                      .Table<Concesionario>().Where(x => x.ClaveConcesionario == id)
                      .FirstOrDefault();
        }

        public async Task<IEnumerable<Concesionario>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<Concesionario>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(Concesionario item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}