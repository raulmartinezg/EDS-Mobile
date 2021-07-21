using Movil.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Movil.SQLiteDataFolioViaje))]
namespace Movil
{
    public class SQLiteDataFolioViaje : IDataStore<FolioViaje>
    {
        private readonly ISQLitePlatform _platform;

        public SQLiteDataFolioViaje(ISQLitePlatform platform)
        {
            _platform = platform;
        }

        public SQLiteDataFolioViaje()
        {
            _platform = DependencyService.Get<ISQLitePlatform>();
        }

        public async Task<bool> zAddItemAsync(FolioViaje item)
        {
            bool Resp = false;

            FolioViaje findbjt = await zGetItemAsync(item.ClaveFolioViaje);

            if (findbjt == null)
            {
                Resp = (await _platform.mGetConnectionAsync().InsertAsync(item)) > 0;
            }

            return Resp;
        }

        public async Task<MensageGenerico> zAddItemsAsync(List<FolioViaje> itemfv)
        {
            try
            {
                foreach (var xitem in itemfv)
                {
                    await zAddItemAsync(xitem);
                }

                return new MensageGenerico(true, "OK");
            }
            catch (Exception err)
            {
                return new MensageGenerico(false, "Error al insertar FolioViaje :" + err.Message);
            }
        }

        public async Task<bool> zUpdateItemAsync(FolioViaje item)
        {
            return (await _platform.mGetConnectionAsync().UpdateAsync(item)) > 0;
        }

        public async Task<bool> zDeleteItemAsync(FolioViaje item)
        {
            return (await _platform.mGetConnectionAsync().DeleteAsync(item)) > 0;
        }

        public async Task<FolioViaje> zGetItemAsync(int idcfv)
        {
            return await _platform.mGetConnectionAsync()
                      .Table<FolioViaje>().Where(x => x.ClaveFolioViaje == idcfv)
                      .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FolioViaje>> zGetFoliosOperadorAsync(int idcop)
        {
            return await _platform.mGetConnectionAsync()
                      .Table<FolioViaje>().Where(x => x.ClaveOperador == idcop)
                      .ToListAsync();
        }

        public IEnumerable<FolioViaje> zGetFoliosOperador(int idcop)
        {
            return _platform.mGetConnection()
                      .Table<FolioViaje>().Where(x => x.ClaveOperador == idcop)
                      .ToList();
        }

        public async Task<FolioViaje> zGetItemAsync(string id)
        {
            return await _platform.mGetConnectionAsync()
                     .Table<FolioViaje>().Where(x => x.FolioDeViaje == id)
                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FolioViaje>> zGetItemsAsync()
        {
            return await _platform.mGetConnectionAsync().Table<FolioViaje>().ToListAsync();
        }

        public async Task<List<FolioViaje>> zGetPendientesSincronizar()
        {
            return await _platform.mGetConnectionAsync().Table<FolioViaje>().Where(x => x.Sincronizado == 0).ToListAsync();
        }
    }
}