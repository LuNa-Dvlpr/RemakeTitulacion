using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Titulacion.Models;

namespace Titulacion.Controllers
{
    [Authorize] // Protegemos todo el controlador para que solo usuarios logueados puedan entrar
    public class ConfigController : Controller
    {
        private readonly TutoriasContext _context;

        public ConfigController(TutoriasContext context)
        {
            _context = context;
        }

        // --- ACCIONES PARA EL ROL DE ALUMNO ---

        [HttpGet]
        [Authorize(Roles = "Alumno")]
        public IActionResult Alumno()
        {
            string boletaUsuario = User.FindFirstValue("Usuario");
            if (boletaUsuario == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var alumno = _context.Alumno
                                 .Include(a => a.IdUsuarioNavigation) // Incluimos la tabla Usuarios para la consulta
                                 .FirstOrDefault(a => a.IdUsuarioNavigation.User == boletaUsuario);

            if (alumno == null)
            {
                return NotFound("No se encontraron los datos del alumno.");
            }

            var modelo = new AlumnoConfigViewModel
            {
                NombreCompleto = $"{alumno.Nombre} {alumno.ApellidoPat} {alumno.ApellidoMat}",
                Correo = alumno.Correo,
                Grupo = alumno.Grupo
            };

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = "Alumno")]
        public IActionResult CambiarPassword(string nuevaPassword, string confirmarPassword)
        {
            if (string.IsNullOrEmpty(nuevaPassword) || nuevaPassword != confirmarPassword)
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden o están vacías.";
                return RedirectToAction("Alumno");
            }

            string boletaUsuario = User.FindFirstValue("Usuario");
            var usuario = _context.Usuarios.FirstOrDefault(u => u.User == boletaUsuario);

            if (usuario != null)
            {
                usuario.Pass = Titulacion.Clases.General.cifrarDatos(nuevaPassword);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "¡Contraseña actualizada con éxito!";
            }

            return RedirectToAction("Alumno");
        }

        // --- ACCIONES PARA EL ROL DE PROFESOR ---

        [HttpGet]
        [Authorize(Roles = "Profesor")]
        public IActionResult Profesor()
        {
            string idUsuarioProfesor = User.FindFirstValue("Usuario");
            if (idUsuarioProfesor == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var profesor = _context.Profesor
                                   .Include(p => p.IdUsuarioNavigation)
                                   .FirstOrDefault(p => p.IdUsuarioNavigation.User == idUsuarioProfesor);

            if (profesor == null)
            {
                return NotFound("Datos del profesor no encontrados.");
            }

            var modelo = new ProfesorConfigViewModel
            {
                NombreCompleto = $"{profesor.Nombre} {profesor.ApellidoPat} {profesor.ApellidoMat}",
                Correo = profesor.Correo,
                Grupo = profesor.Grupo,
                HorasTutoria = profesor.HorasTutoria,
                HorasTotales = profesor.HorasTotales
            };

            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = "Profesor")]
        public IActionResult CambiarPasswordProfesor(string nuevaPassword, string confirmarPassword)
        {
            if (string.IsNullOrEmpty(nuevaPassword) || nuevaPassword != confirmarPassword)
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden o están vacías.";
                return RedirectToAction("Profesor");
            }

            string idUsuarioProfesor = User.FindFirstValue("Usuario");
            var usuario = _context.Usuarios.FirstOrDefault(u => u.User == idUsuarioProfesor);

            if (usuario != null)
            {
                usuario.Pass = Titulacion.Clases.General.cifrarDatos(nuevaPassword);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "¡Contraseña actualizada con éxito!";
            }

            return RedirectToAction("Profesor");
        }

        [HttpPost]
        [Authorize(Roles = "Profesor")]
        public IActionResult CambiarCorreoProfesor(string nuevoCorreo)
        {
            if (string.IsNullOrEmpty(nuevoCorreo))
            {
                TempData["ErrorMessage"] = "El correo no puede estar vacío.";
                return RedirectToAction("Profesor");
            }

            string idUsuarioProfesor = User.FindFirstValue("Usuario");
            var profesor = _context.Profesor
                                   .FirstOrDefault(p => p.IdUsuarioNavigation.User == idUsuarioProfesor);

            if (profesor != null)
            {
                profesor.Correo = nuevoCorreo;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "¡Correo actualizado con éxito!";
            }

            return RedirectToAction("Profesor");
        }
    }
}