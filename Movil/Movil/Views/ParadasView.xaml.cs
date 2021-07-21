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
    public partial class ParadasView : ContentPage
    {
        ParadasViewModel zVMParadas;

        private string Tipo { get; set; }

        SearchBar searchBar;

        public ParadasView(string tipo)
        {
            try
            {
                InitializeComponent();
                Tipo = tipo;
                zVMParadas = new ParadasViewModel(App.zClaveFV.ToString(), tipo);
                BindingContext = zVMParadas;
                listView.TapCommand = zVMParadas.TappedCommand;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        private void zMSearB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                searchBar = (sender as SearchBar);
                if (listView.DataSource != null)
                {
                    listView.DataSource.Filter = FilterContacts;
                    listView.DataSource.RefreshFilter();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        private bool FilterContacts(object obj)
        {
            try
            {
                if (searchBar == null || searchBar.Text == null)
                    return true;

                var concesionario = obj as Concesionario;
                if (concesionario.NombreConcesionario.ToLower().Contains(searchBar.Text.ToLower())
                     || concesionario.Direccion.ToLower().Contains(searchBar.Text.ToLower())
                     || concesionario.NumeroConcesionario.ToString().Contains(searchBar.Text.ToLower()))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
                return false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                zVMParadas.Initialize(App.zClaveFV.ToString(), Tipo);
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (ParadasViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar viaje?");
            return true;
        }
    }
}