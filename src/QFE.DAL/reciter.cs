using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFE.DAL
{
    [Table(name: "reciter")]
    public class reciter {
        [Key]
        [Column(name: "idx")]
        public int idx { get; set; }
        [Column(name: "name")]
        public string name { get; set; }
       
        [Column(name: "mediaurl")]
        public string mediaurl { get; set; }
    }
}
