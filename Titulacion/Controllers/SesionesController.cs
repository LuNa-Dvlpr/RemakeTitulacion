using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Titulacion.Clases;
using Titulacion.Models;

namespace Titulacion.Controllers
{
    [Authorize]
    public class SesionesController : Controller
    {
        private readonly TutoriasContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // El constructor recibe los servicios que el controlador necesita
        public SesionesController(TutoriasContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Authorize(Roles = "Alumno")]
        public IActionResult InicioAlumno()
        {
            string boletaUsuario = User.FindFirstValue("Usuario");
            var usuarioCLS = new UsuarioCLS();

            // Obtiene la lista de profesores disponibles para el grupo del alumno
            List<Profesor> listaProfesor = usuarioCLS.listaProfesores(boletaUsuario, _context);

            // Obtiene los detalles del alumno para los mensajes de bienvenida
            var alumnoInfo = _context.Alumno.FirstOrDefault(a => a.IdUsuarioNavigation.User == boletaUsuario);

            ViewBag.Tutoria = alumnoInfo?.Tutoria ?? false;
            ViewBag.Nombre = (alumnoInfo != null) ? $"{alumnoInfo.Nombre} {alumnoInfo.ApellidoPat} {alumnoInfo.ApellidoMat}" : "Alumno";
            ViewBag.Boleta = boletaUsuario;

            return View(listaProfesor);
        }

        [HttpPost]
        [Authorize(Roles = "Alumno")]
        public IActionResult InicioAlumno(string IdUsuario, string nomProfe)
        {
            var usuarioCLS = new UsuarioCLS();
            ViewBag.Tutoria = usuarioCLS.RegistrarTutor(IdUsuario, nomProfe, _context);

            // Vuelve a cargar la información necesaria para la vista
            List<Profesor> listaProfesor = usuarioCLS.listaProfesores(IdUsuario, _context);
            var alumnoInfo = _context.Alumno.FirstOrDefault(a => a.IdUsuarioNavigation.User == IdUsuario);
            ViewBag.Nombre = (alumnoInfo != null) ? $"{alumnoInfo.Nombre} {alumnoInfo.ApellidoPat} {alumnoInfo.ApellidoMat}" : "Alumno";
            ViewBag.Boleta = IdUsuario;

            return View(listaProfesor);
        }

        [HttpPost]
        public JsonResult SolicitarTutoria([FromBody] SolicitudTutoriaViewModel data)
        {
            try
            {
                // Buscamos al alumno por su boleta
                var alumno = _context.Alumno
                                     .Include(a => a.IdUsuarioNavigation) // Incluimos la tabla Usuarios para la consulta
                                     .FirstOrDefault(a => a.IdUsuarioNavigation.User == data.Boleta);
                if (alumno == null)
                {
                    return Json(new { success = false, message = "Alumno no encontrado." });
                }

                // Buscamos al profesor por su nombre completo
                var profesor = _context.Profesor.FirstOrDefault(p => (p.Nombre + " " + p.ApellidoPat + " " + p.ApellidoMat) == data.Profesor);
                if (profesor == null)
                {
                    return Json(new { success = false, message = "Profesor no encontrado." });
                }

                // Verificamos si ya existe una inscripción
                var yaInscrito = _context.Inscripcion.Any(i => i.IdAlumno == alumno.IdAlumno);
                if (yaInscrito)
                {
                    return Json(new { success = false, message = "Este alumno ya tiene una tutoría inscrita." });
                }

                // Creamos la nueva inscripción
                var nuevaInscripcion = new Inscripcion
                {
                    IdAlumno = alumno.IdAlumno,
                    IdProfesor = profesor.IdProfesor,
                    Fecha = DateTime.Now.Date,
                    Folio = Titulacion.Clases.General.Folio(alumno) // Usamos tu método para generar el folio
                };

                _context.Inscripcion.Add(nuevaInscripcion);

                // Actualizamos al alumno para marcar que ya tiene tutoría
                alumno.Tutoria = true;

                _context.SaveChanges();

                return Json(new { success = true, message = "¡Tutoría inscrita con éxito!" });
            }
            catch (Exception ex)
            {
                // Devolvemos el mensaje de error real para facilitar la depuración
                return Json(new { success = false, message = "Ocurrió un error en el servidor: " + ex.Message });
            }
        }

        [Authorize(Roles = "Alumno")]
        public FileResult Comprobante()
        {
            string boletaUsuario = User.FindFirstValue("Usuario");
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            ComprobanteCLS compro = new ComprobanteCLS();
            FileStream documento = compro.GenerarComprobante(_context, boletaUsuario, wwwRootPath);

            return File(documento, "application/pdf", $"Comprobante_{boletaUsuario}.pdf");
        }

        [HttpGet]
        [Authorize(Roles = "Profesor")]
        public IActionResult InicioProfesor()
        {
            string idUsuarioProfesor = User.FindFirstValue("Usuario");
            var usuarioCLS = new UsuarioCLS();

            // Obtiene la lista de alumnos inscritos con este profesor
            List<Alumno> alumnos = usuarioCLS.listaAlumnos(idUsuarioProfesor, _context);

            return View(alumnos);
        }
    }
}