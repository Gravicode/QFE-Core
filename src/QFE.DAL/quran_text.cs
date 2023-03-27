using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "quran_text")]
    public class quran_text {
        [Key]
        [Column(name: "index")]
        public int index { get; set; }
        [Column(name: "sura")]
        public int sura { get; set; }
        [Column(name: "aya")]
        public int aya { get; set; }
        [Column(name: "text")]
        public string text { get; set; }
    }
}
