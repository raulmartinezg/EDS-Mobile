using Movil.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TravelDetailView : ContentPage
    {
        public TravelDetailView()
        {
            InitializeComponent();
            BindingContext = new TravelDetailViewModel(GridProximo);
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (TravelDetailViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar viaje?");
            return true;
        }
    }
}