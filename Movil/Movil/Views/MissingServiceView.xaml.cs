using Movil.DataSQL;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.ViewModels;
using Newtonsoft.Json;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MissingServiceView : ContentPage
    {
        MissingServiceViewModel ViewModel = new MissingServiceViewModel();

        SearchBar searchBar;

        public MissingServiceView()
        {
            try
            {
                InitializeComponent();

                BindingContext = ViewModel;

                listView.TapCommand = ViewModel.TappedCommand;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        private void filterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                listView.DataSource.Filter = FilterContacts;
                listView.DataSource.RefreshFilter();
            }
        }

        private bool FilterContacts(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var articulo = obj as ArticulosRequest;
            if (articulo.SKU.ToLower().Contains(searchBar.Text.ToLower())
                 || articulo.Descripcion.ToLower().Contains(searchBar.Text.ToLower()))
                return true;
            else
                return false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                ViewModel.Initialize();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (MissingServiceViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar entrega?");
            return true;
        }
    }
}