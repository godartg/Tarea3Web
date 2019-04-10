using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarea3_CondoriVilcapuma.Models;
using System.IO;

namespace Tarea3_CondoriVilcapuma.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            
        }
        
        // GET: Home
        public ActionResult Index()
        {
            List<ClsArchivo> listFiles = new List<ClsArchivo>();
            var path = Server.MapPath(@"~/Content/Files/");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);


            foreach (var item in directoryInfo.GetFiles())
            {
                listFiles.Add(new ClsArchivo()
                {

                    nombreFile = item.Name,
                    link = item.FullName,
                    tipo = item.Extension,
                    tamano = item.Length
                    

                });

            }
            //listaArchivo = dir.EnumerateFiles().Select(f => f.Name);
            return View(listFiles);
        }
        

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var path = Path.Combine(Server.MapPath("~/Content/Files/"), file.FileName);

            var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);

            using (var sw = new FileStream(path, FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Eliminar(string file)
        {
            FileInfo archivo = new FileInfo(@file);
            archivo.Delete();
            return RedirectToAction("Index");
        }
        public ActionResult Editar(string file)
        {
            FileInfo archivo = new FileInfo(@file);

            return RedirectToAction("Index");
        }

    }
}