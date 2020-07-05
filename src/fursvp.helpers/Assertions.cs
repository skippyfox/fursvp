// <copyright file="Assertions.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Helpers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A helper class that can throws an exception in the event of an unexpected result.
    /// </summary>
    /// <typeparam name="TException">The type of Exception to throw when an assert fails.</typeparam>
    public class Assertions<TException>
        where TException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Assertions{TException}"/> class.
        /// </summary>
        public Assertions()
        {
            var type = typeof(TException);
            Ctor = type.GetConstructor(new[] { typeof(string) });
            if (Ctor == null)
            {
                throw new ArgumentException($"{type.Name} does not have a (string) constructor.");
            }
        }

        private ConstructorInfo Ctor { get; }

        /// <summary>
        /// Throws an exception when an evaluation result is false.
        /// </summary>
        /// <param name="evaluation">The result that is expected to have evaluated to true.</param>
        /// <param name="reason">The Exception message used when the result is not true and an Exception is thrown.</param>
        public void That(bool evaluation, string reason)
        {
            if (!evaluation)
            {
                throw (TException)Ctor.Invoke(new[] { reason });
            }
        }
    }
}
