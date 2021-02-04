// <copyright file="FirestoreOptions.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data.Firestore
{
    /// <summary>
    /// A collection of configuration options for use with the <see cref="FirestoreRepository{T}"/>.
    /// </summary>
    public class FirestoreOptions
    {
        /// <summary>
        /// Specifies the portion of env file to import
        /// </summary>
        public const string SectionName = "Firestore";

        /// <summary>
        /// Gets or sets the Google Cloud ProjectId for Firebase
        /// </summary>
        public string ProjectId { get; set; }
    }
}




