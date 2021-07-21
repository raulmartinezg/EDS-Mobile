using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataOperador))]
namespace Movil
{
    public class SQLiteDataOperador : IDataStore<Operador>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataOperador(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataOperador()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(Operador item)
        {
            bool Resp = false;

            Operador findbjt = await zGetItemAsync(item.FiOperador);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;

        }

        public async Task<bool> zUpdateItemAsync(Operador item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        public async Task<bool> zDeleteItemAsync(Operador item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<Operador> zGetItemAsync(int id)
        {
            return await _platform.mGetConnectionAsync()
                .Table<Operador>().Where(x => x.FiOperador == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Operador> zGetItemAsync(string numOp)
        {
            var result = await _platform.mGetConnectionAsync()
                  .Table<Operador>().Where(x => x.FnOperador == numOp)
                  .FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<Operador>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<Operador>().ToListAsync();
        }

        public Operador zGetItem(string numOp)
        {
            var result = _platform.mGetConnection().Table<Operador>().Where(x => x.FnOperador == numOp).FirstOrDefault();
            return result;
        }

        public Operador zGetItem(int idOp)
        {
            var result = _platform.mGetConnection().Table<Operador>().Where(x => x.FiOperador == idOp).FirstOrDefault();

            return result;
        }

        public Task<MensageGenerico> zAddItemsAsync(List<Operador> items)
        {
            throw new NotImplementedException();
        }
    }
}
