﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio1.DBContext;
using System.Net;
using System.IO;
using Laboratorio1.Models;
using System.Timers;
using Laboratorio1.LOG;

namespace Laboratorio1.Controllers
{
    public class JugadorController : Controller
    { 
        public DefaultConnection db = DefaultConnection.getInstance;

        // GET: Jugador
        public ActionResult Index()
        {
            
            return View(db.Jugadores.ToList());
        }

        // GET: Jugador/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Jugador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jugador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include ="Nombre,Apellido,Club,Posicion,Salario")]Models.Jugador jugador)
        {
            LogFile timing = new LogFile();
            timing.StartTimer();
             
            if (ModelState.IsValid)
            {
                jugador.JugadorID = ++db.IdActual;
                db.Jugadores.Add(jugador);

                timing.Logcreate("Crear");
                return RedirectToAction("Index");
            }
            
            return View(jugador);
        }

        // GET: Jugador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Jugador jugador = db.Jugadores.Find(x => x.JugadorID == id);
            if (jugador == null)
            {
                
                return HttpNotFound();
            }

            return View(jugador);
        }

        // POST: Jugador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JugadorID, Apellido, Nombre, Club, Posicion, Salario")]Models.Jugador jugador)
        {
            LogFile timing = new LogFile();
            timing.StartTimer();
            if (ModelState.IsValid)
            {
                Models.Jugador modifiedJugador = db.Jugadores.Find(x => x.JugadorID == jugador.JugadorID);

                if (modifiedJugador == null)
                {
                    timing.Logcreate("Editar");
                    return HttpNotFound();
                }

                modifiedJugador.Club = jugador.Club;
                modifiedJugador.Salario = jugador.Salario;
                timing.Logcreate("Editar");
                return View(modifiedJugador);
            }
            timing.Logcreate("Editar");
            return RedirectToAction("Index");
        }
        
        // GET: Jugador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Jugador jugador = db.Jugadores.Find(x => x.JugadorID == id);

            if (jugador == null)
            {
                return HttpNotFound();
            }
            return View(jugador);
        }
        

        // POST: Jugador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LogFile timing = new LogFile();
            timing.StartTimer();
            db.Jugadores.Remove(db.Jugadores.Find(x => x.JugadorID == id));
            timing.Logcreate("Eliminar");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase File)
            
        {
            LogFile timing = new LogFile();
           
            string filePath = string.Empty;
            if (File != null)
            {
                string path = Server.MapPath("~/UploadedFiles/");
                filePath = path + Path.GetFileName(File.FileName);
                string extension = Path.GetExtension(File.FileName);
                File.SaveAs(filePath);
                ViewBag.Message = "Archivo Cargado";

                string csvDatos = System.IO.File.ReadAllText(filePath);
                string[] csvDatos_ = csvDatos.Split('\n');
                
                foreach (string fila in csvDatos_)
                {

                    if (!string.IsNullOrEmpty(fila)&& fila !=csvDatos_[0])
                    {
                        db.Jugadores.Add(new Models.Jugador
                        {
                            JugadorID = ++db.IdActual,
                            Club = fila.Split(',')[0],
                            Apellido = fila.Split(',')[1],
                            Nombre = fila.Split(',')[2],
                            Posicion = fila.Split(',')[3],
                            Salario = (int)Convert.ToDouble(fila.Split(',')[4])
                        });
                    }
                }


                timing.Logcreate("Cargar Archivo");
                return RedirectToAction("Index");
            }

            timing.Logcreate("Cargar Archivo");
            return View();
           
        }
        [HttpGet]
        public ActionResult DeleteByfile()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteByfile(HttpPostedFileBase File)
       {
            LogFile timing = new LogFile();
            timing.StartTimer();
            var listdelete = new List<Jugador>();
            var filepath = string.Empty;

            if (File != null)
            {
                string path = Server.MapPath("~/Deleting/");
                filepath = path + Path.GetFileName(File.FileName);
                string extension = Path.GetExtension(File.FileName);
                File.SaveAs(filepath);
                ViewBag.Message = "Archivo Cargado";
                string csvDatos = System.IO.File.ReadAllText(filepath);
                string[] csvDatos_ = csvDatos.Split('\n');

                foreach (string fila in csvDatos_)
                {

                    if (!string.IsNullOrEmpty(fila) && fila != csvDatos_[0])
                    {
                        listdelete.Add(new Models.Jugador
                        {

                            Club = fila.Split(',')[0],
                            Apellido = fila.Split(',')[1],
                            Nombre = fila.Split(',')[2],
                            Posicion = fila.Split(',')[3],
                            Salario = (int)Convert.ToDouble(fila.Split(',')[4])
                        });
                    }
                }

                foreach (Jugador futbolista in listdelete)
                {
                    for(int i = 0; i< db.Jugadores.Count; i++)
                    {
                        Jugador toDelete = db.Jugadores[i];
                        if(toDelete.Nombre == futbolista.Nombre && toDelete.Apellido == futbolista.Apellido && toDelete.Club == futbolista.Club)
                        {
                            db.Jugadores.Remove(toDelete);
                        }

                    }


                }
               
            }
            timing.Logcreate("Eliminar por Archivo");
            return View();

        }
       
    }
}
