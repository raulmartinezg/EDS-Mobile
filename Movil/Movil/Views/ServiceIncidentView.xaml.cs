using Movil.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceIncidentView : ContentPage
    {
        ServiceIncidentViewModel ViewModel = new ServiceIncidentViewModel();

        public ServiceIncidentView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (ServiceIncidentViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar entrega?");
            return true;
        }
    }
}