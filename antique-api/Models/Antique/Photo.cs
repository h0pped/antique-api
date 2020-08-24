using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace antique_api.Models.Antique
{
    public class Photo
    {
        [Key]
        public int ID { get; set; }
        public string Path { get; set; }
    }
}
