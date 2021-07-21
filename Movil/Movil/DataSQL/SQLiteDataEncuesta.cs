using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataEncuesta))]
namespace Movil
{
    public class SQLiteDataEncuesta : IDataStore<Encuesta>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataEncuesta(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataEncuesta()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Encuesta item)
        {
            bool Resp = false;

            Encuesta findbjt = await zGetItemNumAsync(item.ClaveEncuesta, item.NumP);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<Encuesta> items)
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

        public async Task<bool> zDeleteItemAsync(Encuesta item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Encuesta> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Encuesta>().Where(x => x.ClaveEncuesta == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<Encuesta> zGetItemNumAsync(int ClaveEncuesta, int NumP)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<Encuesta>().Where(x => x.ClaveEncuesta == ClaveEncuesta && x.NumP == NumP)
                     .FirstOrDefaultAsync();
        }

        public async Task<List<Encuesta>> zGetItemsAsync()
        {
            var zenc = await _platform.mGetConnectionAsync().Table<Encuesta>().ToListAsync();
            return zenc;
        }

        public async Task<bool> zUpdateItemAsync(Encuesta item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        Task<IEnumerable<Encuesta>> IDataStore<Encuesta>.zGetItemsAsync()
        {
            throw new NotImplementedException();
        }
    }
}