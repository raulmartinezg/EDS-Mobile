using Movil.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecoleccionesView : ContentPage
    {
        public RecoleccionesViewModel ViewModel = new RecoleccionesViewModel();

        public RecoleccionesView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (RecoleccionesViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}