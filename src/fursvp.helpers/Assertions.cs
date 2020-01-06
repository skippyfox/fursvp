using System;
using System.Reflection;

namespace fursvp.helpers
{
    public class Assertions<TException> where TException : Exception
    {
        private ConstructorInfo _ctor { get; }

        public Assertions()
        {
            var type = typeof(TException);
            _ctor = type.GetConstructor(new[] { typeof(string) });
            if (_ctor == null)
            {
                throw new ArgumentException($"{type.Name} does not have a (string) constructor.");
            }
        }

        public void That(bool evaluation, string reason)
        {
            if (!evaluation)
            {
                throw (TException)_ctor.Invoke(new[] { reason });
            }
        }
    }
}
