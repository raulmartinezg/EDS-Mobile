using Movil.Models.Response;
using Movil.ViewModels;
using Splat;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TravelsView : ContentPage
    {
        internal TravelsViewModel ViewModel { get; set; } = Locator.Current.GetService<TravelsViewModel>();

        SearchBar searchBar;

        public TravelsView()
        {
            InitializeComponent();

            BindingContext = ViewModel;

            listView.TapCommand = ViewModel.TappedCommand;
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

            var viaje = obj as dvNewDataSet;
            if (viaje.fvi.ToLower().Contains(searchBar.Text.ToLower())
                 || viaje.rut.ToLower().Contains(searchBar.Text.ToLower()))
                return true;
            else
                return false;
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (TravelsViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar viaje?");
            return true;
        }
    }
}