using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Titulacion.Models
{
    public partial class Grupos
    {
        public Grupos()
        {
            RelacionGrupos = new HashSet<RelacionGrupos>();
        }

        public int IdGrupo { get; set; }
        public string Grupo { get; set; }

        public virtual ICollection<RelacionGrupos> RelacionGrupos { get; set; }
    }
}
