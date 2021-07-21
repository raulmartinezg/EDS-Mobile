using System;

namespace Movil
{
    [Serializable]
    public class UserLoginResponse
    {
        public string token { get; set; }

        public DateTime expires { get; set; }

        public int status { get; set; }
        public bool success { get; set; }

        public User data { get; set; }

        public string message { get; set; }

    }
    [Serializable]
    public class User
    {
        /// <summary>
        /// claveOperador
        /// </summary>
        public int cus { get; set; }

        public string success { get; set; }

        public int estatus { get; set; }

        public int nop { get; set; }

        public string pwd { get; set; }
        public string nom { get; set; }
        public string message { get; set; }
        public string exhorto { get; set; }


    }

}
