using Movil.Models;
using System;
namespace Movil
{
    [Serializable]
    public class UserLoginRequest
    {
        private Data zvardata = new Data();
        private Filter zvarfilter = new Filter();
        public Data data
        {
            get { return zvardata; }
            set { zvardata = value; }
        }
        public Filter filter
        {
            get { return zvarfilter; }
            set { zvarfilter = value; }
        }
    }




}
