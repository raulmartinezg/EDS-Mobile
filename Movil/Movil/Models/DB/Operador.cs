using SQLite;
using System;

namespace Movil
{
    public class Operador
    {
        [PrimaryKey]
        public int FiOperador { get; set; }
        public int FiEstatus { get; set; }
        public string FnOperador { get; set; }

        [MaxLength(100)]
        [NotNull]
        public string FcNombre { get; set; }

        [MaxLength(20)]
        [NotNull]
        public string FcPassword { get; set; }

        [MaxLength(300)]
        public string FcExhorto { get; set; }

        [NotNull]
        public DateTime FdIngreso { get; set; }
    }
}
