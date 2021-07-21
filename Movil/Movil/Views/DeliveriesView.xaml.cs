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
    public partial class DeliveriesView : ContentPage
    {
        public DeliveriesView()
        {
            InitializeComponent();

            BindingContext = new DeliveriesViewModel(GridSKU, numericUpDown);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var vm = (DeliveriesViewModel)BindingContext;
                await vm.Initialize();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override async void OnDisappearing()
        {
            try
            {
                var vm = (DeliveriesViewModel)BindingContext;
                await vm.Stop();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (DeliveriesViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar entrega?");
            return true;
        }
    }
}