using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataOpcionPreguntas))]
namespace Movil
{
    public class SQLiteDataOpcionPreguntas : IDataStore<OpcionPreguntas>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataOpcionPreguntas(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataOpcionPreguntas()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(OpcionPreguntas item)
        {
            bool Resp = false;

            OpcionPreguntas findbjt = await zGetItemAsync(item.ClaveOpcion);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<OpcionPreguntas> items)
        {
            try
            {
                foreach (var xitem in items)
                {
                    await this.zAddItemAsync(xitem);
                }

                return new MensageGenerico(true, "OK");
            }
            catch (Exception err)
            {
                return new MensageGenerico(false, err.Message);
            }
        }

        public Task<bool> zDeleteItemAsync(OpcionPreguntas item)
        {
            throw new NotImplementedException();
        }

        public async Task<OpcionPreguntas> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                      .Table<OpcionPreguntas>().Where(x => x.ClaveOpcion == id)
                      .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OpcionPreguntas>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<OpcionPreguntas>().ToListAsync();
        }

        public async Task<List<OpcionPreguntas>> zGetListItemsAsync(int numopc)
        {
            return await _platform.mGetConnectionAsync()
                      .Table<OpcionPreguntas>().Where(x => x.NumOpc == numopc)
                      .ToListAsync();
        }

        public async Task<bool> zUpdateItemAsync(OpcionPreguntas item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }
    }
}