using Movil.Models;
using System;

namespace Movil
{
    [Serializable]
    public class BodyRequest
    {
        public Data data { get; set; } = new Data();

        public string filter { get; set; }
    }


}
