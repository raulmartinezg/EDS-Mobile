using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataArticulo))]

namespace Movil
{
    public class SQLiteDataArticulo : IDataStore<CatalogoArticulo>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataArticulo(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataArticulo()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(CatalogoArticulo item)
        {
            bool Resp = false;

            CatalogoArticulo findbjt = await zGetItemAsync(item.ClaveArticulo);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<CatalogoArticulo> items)
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

        public async Task<bool> zDeleteItemAsync(CatalogoArticulo item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<CatalogoArticulo> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<CatalogoArticulo>().Where(x => x.ClaveArticulo == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<CatalogoArticulo> zGetItemAsync(string zsku)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<CatalogoArticulo>().Where(x => x.SKU == zsku)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CatalogoArticulo>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<CatalogoArticulo>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(CatalogoArticulo item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}