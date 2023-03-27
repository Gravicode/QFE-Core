using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "hizb")]
    public class hizb {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "surahfrom")]
        public int surahfrom { get; set; }

        [Column(name: "ayahfrom")]
        public int ayahfrom { get; set; }

        [Column(name: "surahto")]
        public int surahto { get; set; }

        [Column(name: "ayahto")]
        public int ayahto { get; set; }
    }
}
