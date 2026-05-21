using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Usuario {

    [Table("Usuario")]
    public class Usuario {
        [Key]
        public int idUsuario { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public int? usuarioCreacion { get; set; }
        public int? usuarioModificacion { get; set; }
        public int? usuarioEliminacion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaModificacion { get; set; }
        public DateTime? fechaEliminacion { get; set; }
        public int eliminado { get; set; }
    }


}
