using Movil.DataSQL;
using Movil.Models.DB;
using Newtonsoft.Json;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        private string _Subtitulo { get; set; }

        private ShellItem Login { get; set; }
        private ShellItem Bienvenida { get; set; }
        private ShellItem Pendientes { get; set; }
        private ShellItem Concluidos { get; set; }
        private ShellItem Incidentes { get; set; }
        private ShellItem Llegada { get; set; }
        private ShellItem Entregas { get; set; }
        private ShellItem Recolecciones { get; set; }
        private ShellItem Salidas { get; set; }
        private ShellItem Encuesta { get; set; }
        private ShellItem Conclusion { get; set; }
        private ShellItem Faltante { get; set; }
        private ShellItem Entregados { get; set; }
        private ShellItem IncidenteServicio { get; set; }
        private ShellItem CerrarSesion { get; set; }
        private ShellItem CerrarViaje { get; set; }
        private ShellItem CerrarServicio { get; set; }
        private ShellItem CerrarEntrega { get; set; }
        private ShellItem TravelDetail { get; set; }
        private ShellItem DetalleServicio { get; set; }

        public string Subtitulo
        {
            get { return _Subtitulo; }
            set
            {
                if (_Subtitulo == value)
                    return;

                _Subtitulo = value;
                OnPropertyChanged(nameof(Subtitulo));
            }
        }

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("travels", typeof(TravelsView));
            Routing.RegisterRoute("welcome", typeof(Bienvenida));

            ItemsInit();

            BindingContext = this;
        }

        public void ItemsInit()
        {
            try
            {
                Login = Items.Where(x => x.Title == "Login").FirstOrDefault();
                Bienvenida = Items.Where(x => x.Title == "Bienvenida").FirstOrDefault();

                TravelDetail = Items.Where(x => x.Title == "Detalle del viaje").FirstOrDefault();
                Pendientes = Items.Where(x => x.Title == "Pendientes").FirstOrDefault();
                Concluidos = Items.Where(x => x.Title == "Concluidos").FirstOrDefault();
                Incidentes = Items.Where(x => x.Title == "Incidentes").FirstOrDefault();

                DetalleServicio = Items.Where(x => x.Title == "Detalle del servicio").FirstOrDefault();
                Llegada = Items.Where(x => x.Title == "Llegada").FirstOrDefault();
                Entregas = Items.Where(x => x.Title == "Entregas").FirstOrDefault();
                Recolecciones = Items.Where(x => x.Title == "Recolecciones").FirstOrDefault();
                Salidas = Items.Where(x => x.Title == "Salidas").FirstOrDefault();
                Encuesta = Items.Where(x => x.Title == "Encuesta").FirstOrDefault();
                Conclusion = Items.Where(x => x.Title == "Conclusión").FirstOrDefault();

                Faltante = Items.Where(x => x.Title == "Faltante").FirstOrDefault();
                Entregados = Items.Where(x => x.Title == "Entregados").FirstOrDefault();
                IncidenteServicio = Items.Where(x => x.Title == "Incidente Servicio").FirstOrDefault();

                CerrarSesion = Items.Where(x => x.Title == "Cerrar Sesión").FirstOrDefault();
                CerrarViaje = Items.Where(x => x.Title == "Cerrar Viaje").FirstOrDefault();
                CerrarServicio = Items.Where(x => x.Title == "Cerrar Servicio").FirstOrDefault();
                CerrarEntrega = Items.Where(x => x.Title == "Cerrar Entrega").FirstOrDefault();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override async void OnNavigated(ShellNavigatedEventArgs shellNavigatedEventArgs)
        {
            base.OnNavigated(shellNavigatedEventArgs);

            try
            {
                string CurrentLocation = shellNavigatedEventArgs.Current.Location.OriginalString;
                string PreviousLocation = shellNavigatedEventArgs.Previous != null ? shellNavigatedEventArgs.Previous.Location.OriginalString : "";

                ShellSection page = Current?.CurrentItem?.CurrentItem;

                switch (page.Title)
                {
                    case "Login":
                        Login.IsVisible = true;
                        Bienvenida.IsVisible = true;
                        break;
                    case "Bienvenida":
                        Subtitulo = "Bienvenida";

                        TravelDetail.IsVisible = false;
                        Pendientes.IsVisible = false;
                        Concluidos.IsVisible = false;
                        Incidentes.IsVisible = false;

                        DetalleServicio.IsVisible = false;
                        Llegada.IsVisible = false;
                        Entregas.IsVisible = false;
                        Recolecciones.IsVisible = false;
                        Salidas.IsVisible = false;
                        Encuesta.IsVisible = false;
                        Conclusion.IsVisible = false;

                        Faltante.IsVisible = false;
                        Entregados.IsVisible = false;
                        IncidenteServicio.IsVisible = false;

                        CerrarServicio.IsVisible = false;
                        CerrarViaje.IsVisible = false;
                        CerrarSesion.IsVisible = true;
                        CerrarEntrega.IsVisible = false;

                        Login.IsVisible = false;
                        break;
                    case "Cerrar Sesión":
                        Login.IsVisible = true;
                        Bienvenida.IsVisible = true;
                        break;
                    case "TravelDetail":
                        Subtitulo = "Folio de viaje";

                        TravelDetail.IsVisible = true;
                        Pendientes.IsVisible = true;
                        Concluidos.IsVisible = true;
                        Incidentes.IsVisible = true;

                        CerrarViaje.IsVisible = true;
                        CerrarSesion.IsVisible = false;
                        break;
                    case "DetalleServicio":
                        Subtitulo = "Servicio Menú";

                        TravelDetail.IsVisible = false;
                        Pendientes.IsVisible = false;
                        Concluidos.IsVisible = false;
                        Incidentes.IsVisible = false;

                        DetalleServicio.IsVisible = true;
                        Llegada.IsVisible = true;
                        Entregas.IsVisible = true;
                        Recolecciones.IsVisible = true;
                        Salidas.IsVisible = true;
                        Encuesta.IsVisible = true;
                        Conclusion.IsVisible = true;

                        CerrarViaje.IsVisible = false;
                        CerrarServicio.IsVisible = true;
                        break;
                    case "Entregas":
                        Subtitulo = "Entregas";

                        DetalleServicio.IsVisible = false;
                        Llegada.IsVisible = false;
                        Entregas.IsVisible = true;
                        Recolecciones.IsVisible = false;
                        Salidas.IsVisible = false;
                        Encuesta.IsVisible = false;
                        Conclusion.IsVisible = false;

                        Faltante.IsVisible = true;
                        Entregados.IsVisible = true;
                        IncidenteServicio.IsVisible = true;

                        CerrarServicio.IsVisible = false;
                        CerrarEntrega.IsVisible = true;
                        break;
                    case "Cerrar Servicio":
                        Subtitulo = "Folio de viaje";

                        TravelDetail.IsVisible = true;
                        Pendientes.IsVisible = true;
                        Concluidos.IsVisible = true;
                        Incidentes.IsVisible = true;

                        DetalleServicio.IsVisible = false;
                        Llegada.IsVisible = false;
                        Entregas.IsVisible = false;
                        Recolecciones.IsVisible = false;
                        Salidas.IsVisible = false;
                        Encuesta.IsVisible = false;
                        Conclusion.IsVisible = false;

                        CerrarViaje.IsVisible = true;
                        CerrarServicio.IsVisible = false;
                        break;
                    case "Cerrar Entrega":
                        Subtitulo = "Servicio Menú";

                        DetalleServicio.IsVisible = true;
                        Llegada.IsVisible = true;
                        Entregas.IsVisible = true;
                        Recolecciones.IsVisible = true;
                        Salidas.IsVisible = true;
                        Encuesta.IsVisible = true;
                        Conclusion.IsVisible = true;

                        Faltante.IsVisible = false;
                        Entregados.IsVisible = false;
                        IncidenteServicio.IsVisible = false;

                        CerrarServicio.IsVisible = true;
                        CerrarEntrega.IsVisible = false;
                        break;
                }

                if ((CurrentLocation.Equals("//Bienvenida/travels") && PreviousLocation.Equals("//Bienvenida/travels")) ||
                    (CurrentLocation.Equals("//main/traveldetail/travels") && PreviousLocation.Equals("//main/traveldetail/travels")) ||
                    (CurrentLocation.Equals("//PagePendientes/travels") && PreviousLocation.Equals("//PagePendientes/travels")) ||
                    (CurrentLocation.Equals("//PageConcluidos/travels") && PreviousLocation.Equals("//PageConcluidos/travels")) ||
                    (CurrentLocation.Equals("//PageIncidentes/travels") && PreviousLocation.Equals("//PageIncidentes/travels")) ||
                    (CurrentLocation.Equals("//CerrarSesion") && PreviousLocation.Equals("//Bienvenida")) ||
                    (CurrentLocation.Equals("//CerrarServicio/travels") && PreviousLocation.Equals("//CerrarServicio/travels")))
                {
                    Bienvenida.IsVisible = true;
                }
                else if (!CurrentLocation.Equals("//login") && !PreviousLocation.Equals(""))
                {
                    Bienvenida.IsVisible = false;
                }

                if (CurrentLocation.Equals("//Bienvenida/travels") && PreviousLocation.Equals("//Bienvenida/travels"))
                {
                    TravelDetail.IsVisible = true;
                }

                if ((CurrentLocation.Equals("//CerrarServicio/travels") && PreviousLocation.Equals("//DetalleServicioView")) ||
                    (CurrentLocation.Equals("//CerrarServicio/travels") && PreviousLocation.Equals("//CerrarEntrega")))
                {
                    Current.GoToAsync("//traveldetail");
                }

                if (CurrentLocation.Equals("//CerrarEntrega") && PreviousLocation.Equals("//PageEntregas"))
                {
                    Current.GoToAsync("//PageEncuesta");
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                Current.GoToAsync("/travels");
                Current.FlyoutIsPresented = false;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }
    }
}