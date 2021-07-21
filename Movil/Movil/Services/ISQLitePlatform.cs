using SQLite;
namespace Movil
{
    public interface ISQLitePlatform
    {
        string NombreDB { get; set; }

        string GetPath();

        SQLiteConnection mGetConnection();

        SQLiteAsyncConnection mGetConnectionAsync();

        void InitTables(SQLiteConnection sQLite);
    }
}
