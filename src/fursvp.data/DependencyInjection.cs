// <copyright file="DependencyInjection.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using AutoMapper;
    using Fursvp.Data;
    using Fursvp.Data.Firestore;
    using Fursvp.Data.RepositoryDecorators;
    using Fursvp.Domain;
    using Fursvp.Domain.Authorization.WriteAuthorization;
    using Fursvp.Domain.Validation;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Provides static DependencyInjection extension methods for installation with .net core service collection.
    /// </summary>
    public static class DependencyInjectionInstaller
    {
        /// <summary>
        /// Registers repository services for use with FireStore repository.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddFursvpDataWithFirestore(this IServiceCollection services)
        {
            services.AddSingleton<IDictionaryMapper<Event>, EventMapper>();
            ConfigureRepositoryServices<Event, FirestoreRepository<Event>>(services);
        }

        /// <summary>
        /// Registers repository services for use with fake repository for development.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddFursvpDataWithFakeRepository(this IServiceCollection services)
        {
            ConfigureRepositoryServices<Event, FakeRepository<Event>>(services);
        }

        private static void ConfigureRepositoryServices<T, TRepository>(IServiceCollection services)
            where T : IEntity<T>
            where TRepository : class, IRepository<T>
        {
            // Register our decorator pipelines
            // Explicitly register the base T repository
            services.AddSingleton<TRepository>();

            // Explicitly register our caching repository with our TRepository (FirestoreRepository or FakeRepository) at the end of the pipeline.
            services.AddSingleton(s =>
            {
                var decorated = s.GetRequiredService<TRepository>();
                var memoryCache = s.GetRequiredService<IMemoryCache>();
                var mapper = s.GetRequiredService<IMapper>();
                return new CachingRepository<T>(decorated, memoryCache, mapper);
            });

            // Explicitly register our repository with authorization with CachingRepository next in the pipeline
            services.AddSingleton(s =>
            {
                var decorated = s.GetRequiredService<CachingRepository<T>>();
                var repositoryRead = s.GetRequiredService<IRepositoryRead<T>>();
                var authorize = s.GetRequiredService<IWriteAuthorize<T>>();
                return new RepositoryWithAuthorization<T>(decorated, repositoryRead, authorize);
            });

            // Explicitly register our repository with validation, with RepositoryWithAuthorization next in the pipeline
            services.AddSingleton(s =>
            {
                var decorated = s.GetRequiredService<RepositoryWithAuthorization<T>>();
                var repositoryRead = s.GetRequiredService<IRepositoryRead<T>>();
                var validator = s.GetRequiredService<IValidate<T>>();
                return new RepositoryWithValidation<T>(decorated, repositoryRead, validator);
            });

            // RepositoryWithValidation is the beginning of the write pipeline
            services.AddSingleton<IRepositoryWrite<T>>(s => s.GetRequiredService<RepositoryWithValidation<T>>());

            // And CachingRepository is the beginning of the read pipeline
            services.AddSingleton<IRepositoryRead<T>>(s => s.GetRequiredService<CachingRepository<T>>());
        }
    }
}
