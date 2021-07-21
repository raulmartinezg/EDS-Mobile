using Movil.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConclusionView : ContentPage
    {
        public ConclusionViewModel ViewModel = new ConclusionViewModel();

        public ConclusionView()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (ConclusionViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}