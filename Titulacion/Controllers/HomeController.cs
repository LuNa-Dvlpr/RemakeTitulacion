using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Titulacion.Clases;
using Titulacion.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Titulacion.Controllers
{
    public class HomeController : Controller
    {
        private readonly TutoriasContext _context;
        public HomeController(TutoriasContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {   //cifrar contraseñas de forma temporal, quitar comentario linea de abajo
           // string contraseñaCifrada = Titulacion.Clases.General.cifrarDatos("profe123");
            ViewBag.Bool = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Usuarios userReci)
        {
            UsuarioCLS user = new UsuarioCLS();

            switch (user.Validar(userReci, _context))
            {
                case 0:
                    var claimsAdmin = new List<Claim>
                    {
                        new Claim("Usuario", userReci.User),
                        new Claim("Contraseña", userReci.Pass),
                        
                    };

                    claimsAdmin.Add(new Claim(ClaimTypes.Role, "Administrador"));

                    var claimsIdentityAdmin = new ClaimsIdentity(claimsAdmin, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentityAdmin));

                    return RedirectToAction("Alumnos", "Administrador");                    
                case 1:
                    var claimsProfe = new List<Claim>
                    {
                        new Claim("Usuario", userReci.User),
                        new Claim("Contraseña", userReci.Pass),
                        new Claim(ClaimTypes.Role, "Profesor")
                    };
                    var claimsIdentityProfesor = new ClaimsIdentity(claimsProfe, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentityProfesor));
                    return RedirectToAction("InicioProfesor", "Sesiones");
                case 2:

                    var claimsAlumno = new List<Claim>
                    {
                        new Claim("Usuario", userReci.User),
                        new Claim("Contraseña", userReci.Pass),
                        new Claim(ClaimTypes.Role, "Alumno")
                    };

                    var claimsIdentityAlumno = new ClaimsIdentity(claimsAlumno, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentityAlumno));

                    return RedirectToAction("InicioAlumno", "Sesiones");
                default:
                    ViewBag.Bool = true;
                    ViewBag.Error = "Tu usuario y/o contraseña son incorrectos";
                    return View();
            }
        }

        // Este método muestra el formulario de registro (GET)
        [HttpGet]
        public IActionResult Registro()
        {
            // Simplemente creamos un modelo vacío y lo pasamos a la vista
            var modelo = new RegistroAlumnoViewModel();
            return View(modelo);
        }

        // Este método procesará los datos del formulario (POST)
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroAlumnoViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                // 1. Creamos el nuevo objeto "Usuarios"
                var nuevoUsuario = new Usuarios
                {
                    User = modelo.Boleta,
                    Pass = General.cifrarDatos(modelo.Pass), // OJO: En un proyecto real, esto debe encriptarse.
                    Tipo = 2, // Tipo 2 para Alumno, según tu lógica de login
                    Visibilidad = true
                };
                _context.Usuarios.Add(nuevoUsuario);
                // Guardamos para que la base de datos le asigne un IdUsuario
                await _context.SaveChangesAsync();

                // 2. Creamos el nuevo objeto "Alumno"
                var nuevoAlumno = new Alumno
                {
                    IdUsuario = nuevoUsuario.IdUsuario, // Usamos el ID del usuario que acabamos de crear
                    Nombre = modelo.Nombre,
                    ApellidoPat = modelo.ApellidoPat,
                    ApellidoMat = modelo.ApellidoMat,
                    Correo = modelo.Correo,
                    Grupo = modelo.Grupo,
                    Tutoria = false // Un nuevo alumno siempre empieza sin tutoría
                };
                _context.Alumno.Add(nuevoAlumno);
                await _context.SaveChangesAsync(); // Guardamos el alumno en la base de datos


                // Enviamos un mensaje de éxito a la pantalla de Login
                TempData["RegistroExitoso"] = "¡Tu cuenta ha sido creada! Ahora puedes iniciar sesión.";
                return RedirectToAction("Login", "Home");
            }

            // Si los datos no son válidos, se vuelve a mostrar el formulario con los errores.
            return View(modelo);
        }

        public async Task<IActionResult> Logout() {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login","Home");
        }

        public IActionResult Denegado() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
