using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarea3_CondoriVilcapuma.Models;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Tarea3_CondoriVilcapuma.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            
        }
        
        // GET: Home
        public ActionResult Index(List<ClsCarga> listaCarga)
        {
            
            var path = Server.MapPath(@"~/Content/Files/");

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            
            List<ClsCarpeta> listCarpetas = new List<ClsCarpeta>();
            List<ClsArchivo> listArchivos;
            List<ClsCarpeta> listSubCarpetas;
            ClsArchivo archivo;
            ClsCarpeta subCarpeta;
            foreach (var item in directoryInfo.GetDirectories())
            {

                listSubCarpetas = new List<ClsCarpeta>();
                foreach (var item2 in item.GetDirectories())
                {
                    subCarpeta = new ClsCarpeta();
                    listArchivos = new List<ClsArchivo>();
                    
                    foreach (var item3 in item.GetFiles())
                    {
                        archivo = new ClsArchivo();
                        archivo.nombreFile = item3.Name;
                        archivo.link = item3.FullName;
                        listArchivos.Add(archivo);
                    }
                    subCarpeta.nombreCarpeta = item2.Name;
                    subCarpeta.link = item2.FullName;
                    listSubCarpetas.Add(subCarpeta);
                    
                }
                
                listCarpetas.Add(new ClsCarpeta()
                {
                    nombreCarpeta = item.Name,
                    link = item.FullName,
                    Carpetas = listSubCarpetas
                });


            }
            if(listaCarga!= null)
            {
                ViewBag.totalRegistros = listaCarga.Count();
                ViewBag.totalCursos = (from lista in listaCarga group lista by lista.asignatura).Count();
                ViewBag.totalDocentes = (from lista in listaCarga group lista by lista.docente).Count();
            }
            else
            {
                ViewBag.totalRegistros = 0;
                ViewBag.totalCursos = 0;
                ViewBag.totalDocentes = 0;
            }
            return View(listCarpetas);
        }
        

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string directorio= "~/Content/Principal/")
        {
            if(file==null || file.ContentLength == 0)
            {
                ViewBag.Error = "Porfavor seleccione un archivo excel";
                return View("Index");
            }
            else
            {
                if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                {

                    var path = Path.Combine(Server.MapPath(directorio), file.FileName);
                    var ubicacion = Server.MapPath(@"~/Content/Files/");
                    var data = new byte[file.ContentLength];
                    file.InputStream.Read(data, 0, file.ContentLength);

                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }
                    List<ClsCarga> listaCarga= importar(path, ubicacion);
                    return RedirectToAction("Index", listaCarga);   
                }
                else
                {
                    var data = new byte[file.ContentLength];
                    var path = Path.Combine(Server.MapPath(directorio), file.FileName);
                    file.InputStream.Read(data, 0, file.ContentLength);
                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }
                    ViewBag.Error = "Porfavor seleccione un archivo excel";
                    return RedirectToAction("Index");
                }
                
            }
            
        }
        public static List<ClsCarga> importar(string _path, string _ubicacion)
        {
            var path = _path;
            var ubicacion = _ubicacion;
            
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook= application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            
            
            var listaCarga = new List<ClsCarga>();
            for(int row= 3; row <range.Rows.Count; row++)
            {
                ClsCarga carga = new ClsCarga();
                if(((Excel.Range)range.Cells[row, 2]).Text != "")
                {
                    carga.codigo = ((Excel.Range)range.Cells[row, 2]).Text;
                    carga.asignatura = ((Excel.Range)range.Cells[row, 3]).Text;
                    carga.tipo = ((Excel.Range)range.Cells[row, 4]).Text;
                    carga.docente = ((Excel.Range)range.Cells[row, 5]).Text;
                    carga.ciclo = ((Excel.Range)range.Cells[row, 6]).Text;
                    carga.seccion = ((Excel.Range)range.Cells[row, 8]).Text;
                    carga.semestre = ((Excel.Range)range.Cells[row, 9]).Text;
                    listaCarga.Add(carga);
                }
                
            }
            workbook.Close(false, path, false);
            var cicloCurso = from lista in listaCarga group lista by lista.ciclo;
            DirectoryInfo directoryInfo = new DirectoryInfo(ubicacion);
            foreach (var ciclo in cicloCurso)
            {
                DirectoryInfo cicloDirectory = directoryInfo.CreateSubdirectory(ciclo.Key);
                foreach (var curso in ciclo)
                {
                    DirectoryInfo cursoDirectory = cicloDirectory.CreateSubdirectory(curso.asignatura);
                }
            }
            return listaCarga;
            

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