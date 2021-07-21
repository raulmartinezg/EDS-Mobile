using Movil.Droid;
using Movil.Models.DB;
using SQLite;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(MyDBSqlite))]
namespace Movil.Droid
{
    public class MyDBSqlite : ISQLitePlatform
    {
        string zvarNombreDB = "SidMovil.sqlite";

        public string NombreDB
        {
            get
            { return zvarNombreDB; }
            set
            { zvarNombreDB = value; }
        }

        public string GetPath()
        {
            //var zstrRuta =  System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);            
            var zstrRuta = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var zstrRutaCombine = Path.Combine(zstrRuta, zvarNombreDB);
            if (!File.Exists(zstrRutaCombine))
            {
                Directory.CreateDirectory(zstrRuta);
            }
            return zstrRutaCombine;
        }

        public SQLiteConnection mGetConnection()
        {
            return new SQLiteConnection(GetPath());
        }

        public SQLiteAsyncConnection mGetConnectionAsync()
        {
            return new SQLiteAsyncConnection(GetPath());
        }

        public void InitTables(SQLiteConnection sQLite)
        {
            if (!CheckIfTableExists(sQLite, nameof(Operador)))
            {
                sQLite.CreateTable<Operador>();
            }
            if (!CheckIfTableExists(sQLite, nameof(CatalogoArticulo)))
            {
                sQLite.CreateTable<CatalogoArticulo>();
            }
            if (!CheckIfTableExists(sQLite, nameof(Concesionario)))
            {
                sQLite.CreateTable<Concesionario>();
            }
            if (!CheckIfTableExists(sQLite, nameof(FolioViaje)))
            {
                sQLite.CreateTable<FolioViaje>();
            }
            if (!CheckIfTableExists(sQLite, nameof(OrdenEmbarque)))
            {
                sQLite.CreateTable<OrdenEmbarque>();
            }
            if (!CheckIfTableExists(sQLite, nameof(OrdenEmbarqueDetalle)))
            {
                sQLite.CreateTable<OrdenEmbarqueDetalle>();
            }
            if (!CheckIfTableExists(sQLite, nameof(Encuesta)))
            {
                sQLite.CreateTable<Encuesta>();
            }
            if (!CheckIfTableExists(sQLite, nameof(EncuestaConcesionario)))
            {
                sQLite.CreateTable<EncuestaConcesionario>();
            }
            if (!CheckIfTableExists(sQLite, nameof(OpcionPreguntas)))
            {
                sQLite.CreateTable<OpcionPreguntas>();
            }
            if (!CheckIfTableExists(sQLite, nameof(LogError)))
            {
                sQLite.CreateTable<LogError>();
            }
            if (!CheckIfTableExists(sQLite, nameof(Incidencia)))
            {
                sQLite.CreateTable<Incidencia>();
            }
            if (!CheckIfTableExists(sQLite, nameof(Entrada)))
            {
                sQLite.CreateTable<Entrada>();
            }
            if (!CheckIfTableExists(sQLite, nameof(Salida)))
            {
                sQLite.CreateTable<Salida>();
            }
        }

        private bool CheckIfTableExists(SQLiteConnection conn, string tableName)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn)
            {
                CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';"
            };
            int resultCount = cmd.ExecuteScalar<int>();
            if (resultCount > 0)
                return true;

            return false;
        }
    }
}