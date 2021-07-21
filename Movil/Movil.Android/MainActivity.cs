using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Movil.Services;
//using Plugin.CurrentActivity;
using Plugin.Media;
using Plugin.Permissions;
using Xamarin.Forms;

namespace Movil.Droid
{

    [Activity(Label = "Movil", Icon = "@drawable/edslogo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            await CrossMedia.Current.Initialize();
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Perform DependencyService IBarcodeReader registration to BarcodeReaderWrapper_Droid
            DependencyService.Register<IBarcodeReader, BarcodeReaderWrapper_Droid>();

            UserDialogs.Init(this);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDYxOTAyQDMxMzgyZTM0MmUzME9YZE5qY0FkYndkbWtNSUhtU3VWTUhyYUkwaDF1UVJOWW0rb1RSODJRUXc9");

            //CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}