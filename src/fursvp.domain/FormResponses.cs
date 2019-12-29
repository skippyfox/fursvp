using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fursvp.domain
{
    public class FormResponses
    {
        public string Prompt { get; set; }
        public ICollection<string> Responses { get; set; } = new Collection<string>();
    }
}
