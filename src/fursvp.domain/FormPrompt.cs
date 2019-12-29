using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fursvp.domain
{
    public abstract class FormPrompt
    {
        public string Prompt { get; set; }
        public ICollection<string> Options { get; set; } = new Collection<string>();
    }
}
