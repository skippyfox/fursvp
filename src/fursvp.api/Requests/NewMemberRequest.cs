using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fursvp.api.Requests
{
    public class NewMemberRequest
    {

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
