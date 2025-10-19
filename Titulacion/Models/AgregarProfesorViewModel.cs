using System.ComponentModel.DataAnnotations;

namespace Titulacion.Models
{
    public class AgregarProfesorViewModel
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string ApellidoPat { get; set; }
        [Required]
        public string ApellidoMat { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
        [Required]
        public string Grupo { get; set; }
        [Required]
        public int HorasTotales { get; set; }
        [Required]
        public int HorasTutoria { get; set; }
    }
}