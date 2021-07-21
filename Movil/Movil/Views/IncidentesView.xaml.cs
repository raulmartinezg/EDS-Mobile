using Movil.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncidentesView : ContentPage
    {
        IncidentesViewModel vm = new IncidentesViewModel();

        public IncidentesView()
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (IncidentesViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar viaje?");
            return true;
        }
    }
}