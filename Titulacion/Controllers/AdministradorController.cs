using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Titulacion.Models;

namespace Titulacion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        private readonly TutoriasContext _context;

        public AdministradorController(TutoriasContext context)
        {
            _context = context;
        }

        // --- GESTIÓN DE ALUMNOS (sin cambios) ---
        [HttpGet]
        public IActionResult Alumnos()
        {
            var listaAlumnos = _context.Alumno.Include(a => a.IdUsuarioNavigation).ToList();
            return View(listaAlumnos);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarAlumno(int idAlumno)
        {
            // Buscamos el registro del alumno que se va a eliminar
            var alumnoAEliminar = await _context.Alumno.FindAsync(idAlumno);

            if (alumnoAEliminar != null)
            {
                // Buscamos el usuario asociado para también eliminarlo
                var usuarioAEliminar = await _context.Usuarios.FindAsync(alumnoAEliminar.IdUsuario);

                // Borramos inscripciones primero para evitar conflictos de dependencia
                var inscripciones = _context.Inscripcion.Where(i => i.IdAlumno == idAlumno);
                _context.Inscripcion.RemoveRange(inscripciones);

                _context.Alumno.Remove(alumnoAEliminar);
                if (usuarioAEliminar != null)
                {
                    _context.Usuarios.Remove(usuarioAEliminar);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Alumno eliminado con éxito.";
            }

            return RedirectToAction("Alumnos");
        }

        

        // --- GESTIÓN DE PROFESORES (CORREGIDO Y COMPLETO) ---
        [HttpGet]
        public IActionResult Profesores()
        {
            var listaProfesores = _context.Profesor
                                          .Include(p => p.IdUsuarioNavigation)
                                          .ToList();
            return View(listaProfesores);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProfesor(int idProfesor)
        {
            var profesorAEliminar = await _context.Profesor.FindAsync(idProfesor);
            if (profesorAEliminar != null)
            {
                var usuarioAEliminar = await _context.Usuarios.FindAsync(profesorAEliminar.IdUsuario);
                var inscripciones = _context.Inscripcion.Where(i => i.IdProfesor == idProfesor);
                _context.Inscripcion.RemoveRange(inscripciones);
                _context.Profesor.Remove(profesorAEliminar);
                if (usuarioAEliminar != null)
                {
                    _context.Usuarios.Remove(usuarioAEliminar);
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profesor eliminado con éxito.";
            }
            return RedirectToAction("Profesores");
        }

        [HttpPost]
        public JsonResult AgregarProfesor([FromBody] AgregarProfesorViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Todos los campos son obligatorios." });
            }
            try
            {
                if (_context.Usuarios.Any(u => u.User == modelo.User))
                {
                    return Json(new { success = false, message = "El nombre de usuario ya existe." });
                }

                var nuevoUsuario = new Usuarios
                {
                    User = modelo.User,
                    Pass = "25DA2ECE06D8767ECCAFF139D4BA7490DD1B9AEEB51F413DE23BCE797CC47E68",
                    Tipo = 1,
                    Visibilidad = true
                };
                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();

                var nuevoProfesor = new Profesor
                {
                    IdUsuario = nuevoUsuario.IdUsuario,
                    Nombre = modelo.Nombre,
                    ApellidoPat = modelo.ApellidoPat,
                    ApellidoMat = modelo.ApellidoMat,
                    Correo = modelo.Correo,
                    Grupo = modelo.Grupo,
                    HorasTotales = modelo.HorasTotales,
                    HorasTutoria = modelo.HorasTutoria
                };
                _context.Profesor.Add(nuevoProfesor);
                _context.SaveChanges();

                return Json(new { success = true, message = "Profesor agregado con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error en el servidor: " + ex.Message });
            }
        }

        // --- GESTIÓN DE INSCRIPCIONES (sin cambios) ---
        [HttpGet]
        public IActionResult Inscripciones()
        {
            var listaInscripciones = _context.Inscripcion
                                             .Include(i => i.IdAlumnoNavigation)
                                             .ThenInclude(a => a.IdUsuarioNavigation)
                                             .Include(i => i.IdProfesorNavigation)
                                             .ToList();
            return View(listaInscripciones);
        }
    }
}