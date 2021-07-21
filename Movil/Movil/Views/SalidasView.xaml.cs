using Movil.DataSQL;
using Movil.Models.DB;
using Movil.ViewModels;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalidasView : ContentPage
    {
        public SalidasView()
        {
            InitializeComponent();
            BindingContext = new SalidasViewModel(Firma); ;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var vm = (SalidasViewModel)BindingContext;
                await vm.GetDeliveries();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (SalidasViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}