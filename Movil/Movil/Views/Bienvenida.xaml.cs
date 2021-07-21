
using Movil.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty("zexhorto", "exhorto")]
    public partial class Bienvenida : ContentPage
    {
        public Bienvenida()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new BienvenidaViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (BienvenidaViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar sesión?");
            return true;
        }
    }
}