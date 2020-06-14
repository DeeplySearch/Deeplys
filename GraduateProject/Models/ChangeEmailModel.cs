using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GraduateProject.Models
{
    public class ChangeEmailModel
    {
        [Required(ErrorMessage = "New Email required", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string NewPassword { get; set; }


        [Required]
        public string ResetCode { get; set; }
    }
}