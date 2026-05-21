using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Catalogo {
    [Table("CatalogoHumand")]
    public class Catalogo {
        [Key]
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
