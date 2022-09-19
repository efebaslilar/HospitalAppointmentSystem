using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHRSLiteEntityLayer.ViewModels
{
    public class DoctorViewModel
    {
        [MinLength(11)]
        [StringLength(11, ErrorMessage = "TC Kimlik numarası 11 haneli olmalıdır!")]
        public string TCNumber { get; set; }
        public string UserId { get; set; }// Identity Model'in ID değeri burada Foreign Key olacaktır.

        [StringLength(50, MinimumLength = 2, ErrorMessage = "İsminiz en az 2 en çok 50 karakter olmalıdır!")]
        [Required(ErrorMessage = "İsim gereklidir")]
        public string Name { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyisminiz en az 2 en çok 50 karakter olmalıdır!")]
        [Required(ErrorMessage = "Soyisim gereklidir")]
        public string Surname { get; set; }
       

    }
}
