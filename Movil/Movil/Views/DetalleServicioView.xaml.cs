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
    public partial class DetalleServicioView : ContentPage
    {
        DetalleServicioViewModel DServicioViewModel = new DetalleServicioViewModel();

        public DetalleServicioView()
        {
            InitializeComponent();
            BindingContext = DServicioViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                await DServicioViewModel.Initialize();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (DetalleServicioViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}