using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "surah")]
    public class surah {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "totalayah")]
        public int totalayah { get; set; }

        [Column(name: "name")]
        public string name { get; set; }

        [Column(name: "latin")]
        public string latin { get; set; }

        [Column(name: "place")]
        public string place { get; set; }
    }
}
