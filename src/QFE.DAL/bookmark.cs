using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "bookmark")]
    public class bookmark {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "title")]
        public string title { get; set; }

        [Column(name: "surah")]
        public int surah { get; set; }

        [Column(name: "juz")]
        public int juz { get; set; }

        [Column(name: "ayah")]
        public int ayah { get; set; }

        
    }
}
