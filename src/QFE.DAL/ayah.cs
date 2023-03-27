using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "ayah")]
    public class ayah
    {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "arabic")]
        public string arabic { get; set; }
    }
}
