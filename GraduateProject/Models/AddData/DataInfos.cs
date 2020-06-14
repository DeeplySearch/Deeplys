using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GraduateProject.Models
{
    public class DataInfos
    {
        public int Id { get; set; }
        public Catygory catygory { get; set; }
        public int CatygoryId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Name required")]
        public string Name { get; set; }

        public string Description { get; set; } 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Url required")]
        public string URL { get; set; }

    }
}