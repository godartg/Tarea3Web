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
        public ActionResult Index(int? @totalRegistros, int? @totalCursos, int? totalDocentes)
        {

            List<ClsArchivo> listFiles = new List<ClsArchivo>();
            var path = Server.MapPath(@"~/Content/Principal/");
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

            ViewBag.totalRegistros = totalRegistros;
            ViewBag.totalCursos = totalCursos;
            ViewBag.totalDocentes = totalDocentes;

            //listaArchivo = dir.EnumerateFiles().Select(f => f.Name);
            return View(listFiles);
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
                    var ubicacion1 = Server.MapPath(@"~/Content/Ejercicio1/");
                    var ubicacion2 = Server.MapPath(@"~/Content/Ejercicio2/");
                    var data = new byte[file.ContentLength];
                    file.InputStream.Read(data, 0, file.ContentLength);

                    using (var sw = new FileStream(path, FileMode.Create))
                    {
                        sw.Write(data, 0, data.Length);
                    }
                    List<ClsCarga> listaCarga= importar(path, ubicacion1, ubicacion2);

                    int totalRegistros = listaCarga.Count();
                    int totalCursos = (from lista in listaCarga group lista by lista.asignatura).Count();
                    int totalDocentes = (from lista in listaCarga group lista by lista.docente).Count();
             

                    
                    return RedirectToAction("Index", "Home", new { @totalRegistros = totalRegistros, @totalCursos = totalCursos , @totalDocentes = totalDocentes });   
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
        public static List<ClsCarga> importar(string _path, string _ubicacion1, string _ubicacion2)
        {
            var path = _path;
            var ubicacion1 = _ubicacion1;
            var ubicacion2 = _ubicacion2;

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
                    carga.seccion = ((Excel.Range)range.Cells[row, 7]).Text;
                    carga.semestre = ((Excel.Range)range.Cells[row, 8]).Text;
                    listaCarga.Add(carga);
                }
                
            }
            workbook.Close(false, path, false);
            var cicloCurso = from lista in listaCarga group lista by lista.ciclo;
            var docenteCurso = from lista in listaCarga group lista by lista.docente;
            DirectoryInfo directoryInfo = new DirectoryInfo(ubicacion1);
            foreach (var ciclo in cicloCurso)
            {
                DirectoryInfo cicloDirectory = directoryInfo.CreateSubdirectory(ciclo.Key);
                foreach (var curso in ciclo)
                {
                    DirectoryInfo cursoDirectory = cicloDirectory.CreateSubdirectory(curso.codigo +" "+curso.asignatura+" " + curso.seccion);
                }
            }
            DirectoryInfo directoryInfo2 = new DirectoryInfo(ubicacion2);
            foreach (var docente in docenteCurso)
            {
                DirectoryInfo docenteDirectory;
                if (docente.Key.Equals(""))
                {
                    docenteDirectory = directoryInfo2.CreateSubdirectory("Anonimo");
                }
                else
                {
                    docenteDirectory = directoryInfo2.CreateSubdirectory(docente.Key);

                }
                foreach (var curso in docente)
                {
                    DirectoryInfo cursoDirectory2 = docenteDirectory.CreateSubdirectory(curso.codigo + " " + curso.asignatura + " " + curso.seccion);
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
        public ActionResult Editar(string totalLink, HttpPostedFileBase file)
        {
            FileInfo archivo = new FileInfo(@totalLink);
            archivo.Delete();
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(@"~/Content/Files/"), fileName);
                file.SaveAs(path);

            }

            return RedirectToAction("Index");
        }

    }
}