using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "transliteration")]
    public class transliteration {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }

        [Column(name: "surahidx")]
        public int surahidx { get; set; }

        [Column(name: "ayahidx")]
        public int ayahidx { get; set; }

        [Column(name: "langid")]
        public int langid { get; set; }

        [Column(name: "content")]
        public string content { get; set; }
    }
}
