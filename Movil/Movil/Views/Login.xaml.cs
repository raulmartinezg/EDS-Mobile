using Movil.DataSQL;
using Movil.Models.DB;
using Newtonsoft.Json;
using Splat;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        internal LoginViewModel ViewModel { get; set; } = Locator.Current.GetService<LoginViewModel>();

        public Login()
        {
            try
            {

                InitializeComponent();
                BindingContext = ViewModel;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            ctrlUsuario.Focus();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}