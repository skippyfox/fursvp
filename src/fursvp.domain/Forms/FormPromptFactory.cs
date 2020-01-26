using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace fursvp.domain.Forms
{
    public class FormPromptFactory : IFormPromptFactory
    {
        private Dictionary<string, Type> _formPrompts { get; }

        public FormPromptFactory()
        {
            _formPrompts = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FormPrompt)))
                .ToDictionary(t => t.Name, t => t);
        }

        public FormPrompt GetFormPrompt(string discriminator, string prompt, ICollection<string> options)
        {
            var instance = (FormPrompt)Activator.CreateInstance(_formPrompts[discriminator]);
            instance.Prompt = prompt;
            instance.Options = options;
            return instance;
        }
    }
}
