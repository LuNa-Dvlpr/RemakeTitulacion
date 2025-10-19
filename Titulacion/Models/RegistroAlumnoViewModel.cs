using System.ComponentModel.DataAnnotations;

namespace Titulacion.Models
{
    public class RegistroAlumnoViewModel
    {
        [Required(ErrorMessage = "El número de boleta es obligatorio.")]
        [StringLength(10, ErrorMessage = "La boleta debe tener exactamente 10 caracteres.", MinimumLength = 10)]
        public string Boleta { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        public string ApellidoPat { get; set; }

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        public string ApellidoMat { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El grupo es obligatorio.")]
        public string Grupo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [DataType(DataType.Password)]
        [Compare("Pass", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPass { get; set; }
    }
}