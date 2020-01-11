using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fursvp.api.Requests
{
    public class UpdateEventRequest
    {
        [Required]
        public string Location { get; set; }
        [Required]
        public string Name { get; set; }

        public string OtherDetails { get; set; }
        [Required]
        public bool RsvpOpen { get; set; }
        public DateTime RsvpClosesAt { get; set; }
        [Required]
        public DateTime StartsAt { get; set; }
        [Required]
        public DateTime EndsAt { get; set; }
    }
}
