using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain.Forms
{
    public interface IFormPromptFactory
    {
        FormPrompt GetFormPrompt(string discriminator, string prompt, ICollection<string> options);
    }
}
