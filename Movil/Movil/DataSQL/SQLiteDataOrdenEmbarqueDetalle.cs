
using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataOrdenEmbarqueDetalle))]
namespace Movil
{
    public class SQLiteDataOrdenEmbarqueDetalle : IDataStore<OrdenEmbarqueDetalle>,IDisposable
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataOrdenEmbarqueDetalle(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataOrdenEmbarqueDetalle()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(OrdenEmbarqueDetalle item)
        {
            
            bool Resp = false;

            OrdenEmbarqueDetalle findbjt = await zGetItemAsync(item.ClaveOrdenEmbDet);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<OrdenEmbarqueDetalle> items)
        {
            try
            {
                foreach (var xitem in items)
                {
                    _ = await zAddItemAsync(xitem);
                }

                return new MensageGenerico(true, "OK");
            }
            catch (Exception err)
            {
                return new MensageGenerico(false, err.Message);
            }
        }

        public async Task<bool> zDeleteItemAsync(OrdenEmbarqueDetalle item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<OrdenEmbarqueDetalle> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                   .Table<OrdenEmbarqueDetalle>().Where(x => x.ClaveOrdenEmbDet == id)
                   .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrdenEmbarqueDetalle>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<OrdenEmbarqueDetalle>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(OrdenEmbarqueDetalle item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        public void Dispose()
        {
            
        }
    }
}