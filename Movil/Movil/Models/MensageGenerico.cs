namespace Movil.Models
{
    public class MensageGenerico
    {
        public MensageGenerico()
        { }
        public MensageGenerico(bool zpstatus, string zpMsg)
        {
            status = zpstatus;
            Mensaje = zpMsg;
        }
        public bool status { get; set; }
        public string Mensaje { get; set; }
    }
}
