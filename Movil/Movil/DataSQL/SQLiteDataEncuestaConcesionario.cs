using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataEncuestaConcesionario))]
namespace Movil
{
    public class SQLiteDataEncuestaConcesionario : IDataStore<EncuestaConcesionario>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataEncuestaConcesionario(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataEncuestaConcesionario()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(EncuestaConcesionario item)
        {
            bool Resp = false;

            EncuestaConcesionario findbjt = await zGetItemAsync(item.ClaveEncCon);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;

        }

        public async Task<MensageGenerico> zAddItemsAsync(List<EncuestaConcesionario> items)
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

        public async Task<bool> zUpdateItemAsync(EncuestaConcesionario item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        public Task<bool> zDeleteItemAsync(EncuestaConcesionario item)
        {
            throw new NotImplementedException();
        }

        public async Task<EncuestaConcesionario> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<EncuestaConcesionario>().Where(x => x.ClaveEncCon == id)
                     .FirstOrDefaultAsync();
        }

        public Task<IEnumerable<EncuestaConcesionario>> zGetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<EncuestaConcesionario>> zGetAllItemsAsync(int ClaveFolioViaje, int ClaveConcesionario)
        {

            return await _platform.mGetConnectionAsync()
                     .Table<EncuestaConcesionario>().Where(x => x.ClaveFolioViaje == ClaveFolioViaje && x.ClaveConcesionario == ClaveConcesionario)
                     .ToListAsync();
        }
    }
}