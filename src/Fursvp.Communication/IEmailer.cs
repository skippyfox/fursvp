// <copyright file="IEmailer.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Communication
{
    using System.Threading.Tasks;

    public interface IEmailer
    {
        void Send(Email email);
        Task SendAsync(Email email);
    }
}
