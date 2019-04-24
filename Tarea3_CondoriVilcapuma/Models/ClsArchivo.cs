using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace Tarea3_CondoriVilcapuma.Models
{
    public class ClsArchivo
    {
        public string nombreFile { get; set; }
        public string link { get; set; }
        public string tipo { get; set; }
        public double tamano { get; set; }

    }
}