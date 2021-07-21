using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataOrdenEmbarque))]
namespace Movil
{
    public class SQLiteDataOrdenEmbarque : IDataStore<OrdenEmbarque>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataOrdenEmbarque(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataOrdenEmbarque()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(OrdenEmbarque item)
        {
            bool Resp = false;

            OrdenEmbarque findbjt = await zGetItemAsync(item.ClaveOrdenEmbarque);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<OrdenEmbarque> items)
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

        public async Task<bool> zDeleteItemAsync(OrdenEmbarque item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<OrdenEmbarque> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                   .Table<OrdenEmbarque>().Where(x => x.ClaveOrdenEmbarque == id)
                   .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrdenEmbarque>> zGetOrnesEmbarqueAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                   .Table<OrdenEmbarque>().Where(x => x.ClaveFolioViaje == id)
                   .ToListAsync();
        }

        public async Task<OrdenEmbarque> zGetOrdenesEmbarqueConcesionarioAsync(int id,int id2)
        {
            return await _platform.mGetConnectionAsync()
                   .Table<OrdenEmbarque>().Where(x => x.ClaveConcesionario == id && x.ClaveFolioViaje == id2)
                   .FirstOrDefaultAsync();
        }

        public async Task<OrdenEmbarque> zGetItemAsync(string NumOE)
        {
            return await _platform.mGetConnectionAsync()
                        .Table<OrdenEmbarque>().Where(x => x.NumOrdenEmbarque == NumOE)
                        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrdenEmbarque>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<OrdenEmbarque>().ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(OrdenEmbarque item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}