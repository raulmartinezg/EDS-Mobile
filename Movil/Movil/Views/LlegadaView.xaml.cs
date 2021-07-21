using Movil.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LlegadaView : ContentPage
    {
        public LlegadaView()
        {
            InitializeComponent();

            BindingContext = new LlegadaViewModel(Firma);
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (LlegadaViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}