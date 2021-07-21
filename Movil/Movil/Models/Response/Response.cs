using System;

namespace Movil
{
    [Serializable]
    public class Response<T>
    {
        public bool zSuccess { get; set; }
        public string zMessage { get; set; }
        public T zResponse
        {
            get; set;
        }
    }
}
