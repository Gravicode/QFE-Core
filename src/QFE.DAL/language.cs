using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "language")]
    public class language {
        [Key]
        [Column(name: "langid")]
        public int langid { get; set; }

        [Column(name: "lang")]
        public string lang { get; set; }

        [Column(name: "dir")]
        public string dir { get; set; }
    }
}
