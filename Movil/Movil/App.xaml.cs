using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Movil.Models.Response;
using Movil.Services.Identity;
using Movil.Services.Routing;
using Movil.ViewModels;
using Movil.Views;
using Splat;
using SQLite;
using Xamarin.Forms;

namespace Movil
{
    public partial class App : Application
    {
        #region vars globales
        public static int zClaveOp { get; set; }

        public static string zUsuario { get; set; }

        public static string zPassword { get; set; }

        public static string zToken { get; set; }

        public static int zClaveFV { get; set; }

        public static int zClaveCon { get; set; }

        private static ISQLitePlatform _sQLite { get; set; }

        public static dvNewDataSet CurrentTravel { get; set; }

        public static dvNewDataSet NextTravel { get; set; }
        #endregion

        public static SQLiteConnection zDb
        {
            get
            {
                return _sQLite.mGetConnection();
            }
        }

        public App()
        {
            InitializeComponent();
            InitializeDi();

            _sQLite = DependencyService.Get<ISQLitePlatform>();
            _sQLite.InitTables(_sQLite.mGetConnection());

            MainPage = new AppShell();
        }

        private void InitializeDi()
        {
            // Services
            Locator.CurrentMutable.RegisterLazySingleton<IRoutingService>(() => new ShellRoutingService());
            Locator.CurrentMutable.RegisterLazySingleton<IIdentityService>(() => new IdentityServiceStub());

            // ViewModels
            Locator.CurrentMutable.Register(() => new LoginViewModel());
            Locator.CurrentMutable.Register(() => new BienvenidaViewModel());
            Locator.CurrentMutable.Register(() => new TravelsViewModel());
            Locator.CurrentMutable.Register(() => new DetalleServicioViewModel());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android=76be0227-9515-4ac9-b2ca-7664782a1a15;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));

        }

        protected override void OnSleep()
        {
            //zscannCN51.CloseBarcodeReader();
            // Handle when your app sleeps
            Application.Current.Properties["idoperador"] = zClaveOp;
            Application.Current.Properties["usuario"] = zUsuario;
            Application.Current.Properties["password"] = zPassword;
            Application.Current.Properties["cfv"] = zClaveFV;
        }

        protected override void OnResume()
        {
            //zscannCN51.OpenBarcodeReader();
            // Handle when your app resumes
            if (Application.Current.Properties.ContainsKey("idoperador"))
                zClaveOp = (int)Application.Current.Properties["idoperador"];
            if (Application.Current.Properties.ContainsKey("usuario"))
                zUsuario = (string)Application.Current.Properties["usuario"];
            if (Application.Current.Properties.ContainsKey("password"))
                zPassword = (string)Application.Current.Properties["password"];
            if (Application.Current.Properties.ContainsKey("cfv"))
                zClaveFV = (int)Application.Current.Properties["cfv"];
        }
    }
}
