using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fursvp.api.Requests
{
    public class UpdateMemberRequest
    {
        [Required]
        public bool IsAttending { get; set; }
        [Required]
        public bool IsOrganizer { get; set; }
    }
}
