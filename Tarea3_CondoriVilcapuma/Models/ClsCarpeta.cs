using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarea3_CondoriVilcapuma.Models
{
    public class ClsCarpeta
    {
        public string nombreCarpeta { get; set; }
        public string link { get; set; }
        public virtual ICollection<ClsArchivo> Archivos { get; set; }
        public virtual ICollection<ClsCarpeta> Carpetas { get; set; }
    }
}