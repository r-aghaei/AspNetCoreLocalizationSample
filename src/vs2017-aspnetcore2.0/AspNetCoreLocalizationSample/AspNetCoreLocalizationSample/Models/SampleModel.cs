using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreLocalizationSample.Models
{
    public class SampleModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [StringLength(3, MinimumLength = 0, ErrorMessage = "Length of {0} field should be between {2} and {1}.")]
        public string Name { get; set; }
    }
}