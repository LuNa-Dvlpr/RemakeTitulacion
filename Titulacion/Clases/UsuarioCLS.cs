using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Titulacion.Models;

namespace Titulacion.Clases
{
    public class UsuarioCLS
    {
        private static bool tutoria;
        private static string nombre;

        public bool Tutoria { get => tutoria; set => tutoria = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        private readonly General generic = new General();

        public int Validar(Usuarios userReci, TutoriasContext db)
        {
            string pass = General.cifrarDatos(userReci.Pass);
            var user = db.Usuarios.FirstOrDefault(x => x.User == userReci.User && x.Pass == pass);

            if (user == null || !user.Visibilidad)
            {
                return -1;
            }

            if (Convert.ToInt32(user.Tipo) == 2)
            {
                var alumn = db.Alumno.FirstOrDefault(x => x.IdUsuario == user.IdUsuario);
                if (alumn != null)
                {
                    Nombre = $"{alumn.Nombre} {alumn.ApellidoPat} {alumn.ApellidoMat}";
                    Tutoria = alumn.Tutoria;
                }
            }
            return Convert.ToInt32(user.Tipo);
        }

        // El método ahora recibe la boleta del alumno y la conexión a la BD
        public List<Profesor> listaProfesores(string boletaUsuario, TutoriasContext db)
        {
            var getUser = db.Usuarios.FirstOrDefault(x => x.User == boletaUsuario);
            if (getUser == null) return new List<Profesor>();

            var getGrupo = db.Alumno.FirstOrDefault(x => x.IdUsuario == getUser.IdUsuario)?.Grupo;
            if (getGrupo == null) return new List<Profesor>();

            return db.Profesor.Where(prof => prof.Grupo == getGrupo).ToList();
        }

        public bool RegistrarTutor(string boleta, string nomProfe, TutoriasContext db)
        {
            try
            {
                var us = db.Usuarios.FirstOrDefault(x => x.User == boleta);
                if (us == null) return false;

                var alm = db.Alumno.FirstOrDefault(x => x.IdUsuario == us.IdUsuario);
                if (alm == null) return false;

                string[] aux = nomProfe.Split(' ');
                var prof = db.Profesor.FirstOrDefault(x => x.Nombre == aux[0]);
                if (prof == null) return false;

                prof.HorasTutoria--;
                alm.Tutoria = true;

                var inscrip = new Inscripcion
                {
                    IdProfesor = prof.IdProfesor,
                    IdAlumno = alm.IdAlumno,
                    Fecha = DateTime.Now.Date,
                    Folio = General.Folio(alm)
                };

                db.Inscripcion.Add(inscrip);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Alumno> listaAlumnos(string idUsuarioProfesor, TutoriasContext db)
        {
            // Primero, encontramos al profesor usando su 'User' ID
            var profesor = db.Profesor
                             .Include(p => p.IdUsuarioNavigation) // Incluimos la tabla Usuarios para poder buscar por el 'User'
                             .FirstOrDefault(p => p.IdUsuarioNavigation.User == idUsuarioProfesor);

            // Si no encontramos al profesor, devolvemos una lista vacía para evitar errores
            if (profesor == null)
            {
                return new List<Alumno>();
            }

            // Ahora, buscamos todas las inscripciones de ese profesor y devolvemos la lista de alumnos asociados
            return db.Inscripcion
                     .Where(i => i.IdProfesor == profesor.IdProfesor)
                     .Include(i => i.IdAlumnoNavigation) // Incluimos los datos del Alumno
                     .ThenInclude(a => a.IdUsuarioNavigation) // Incluimos los datos del Usuario del Alumno
                     .Select(i => i.IdAlumnoNavigation) // Seleccionamos solo los objetos Alumno
                     .ToList();
        }

        // Asumiendo que ComprobanteCLS también necesita el contexto
        // Si no es así, puedes quitar "TutoriasContext db" de aquí y del controlador
        public System.IO.FileStream GenerarComprobante(TutoriasContext db)
        {
            // Aquí iría tu lógica para generar el PDF usando el 'db' context
            // Por ahora, este es un placeholder para que compile
            throw new NotImplementedException();
        }
    }
}