using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fursvp.domain.Forms
{
    public abstract class FormPrompt
    {
        public FormPrompt(string behavior)
        {
            Behavior = behavior;
        }

        /// <summary>
        /// Discriminator for implementations
        /// </summary>
        public string Behavior { get; }
        public string Prompt { get; set; }
        public ICollection<string> Options { get; set; } = new Collection<string>();
    }
}
