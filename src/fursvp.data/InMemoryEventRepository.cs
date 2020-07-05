// <copyright file="InMemoryEventRepository.cs" company="skippyfox">
// Copyright (c) skippyfox. All rights reserved.
// Licensed under the MIT license. See the license.md file in the project root for full license information.
// </copyright>

namespace Fursvp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Fursvp.Domain;
    using Fursvp.Domain.Forms;

    /// <summary>
    /// An in-memory repository store for <see cref="Event"/>.
    /// </summary>
    public class InMemoryEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventRepository"/> class for debugging and testing.
        /// </summary>
        /// <param name="mapper">The instance of <see cref="IMapper" /> for making deep copies.</param>
        public InMemoryEventRepository(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        private IMapper Mapper { get; }

        private List<Event> Events { get; } = new List<Event>();

        /// <summary>
        /// Permanently removes an existing document representing <see ref="Event" /> from memory.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{Event}"/>.</returns>
        public Task Delete(Guid guid)
        {
            this.Events.RemoveAll(e => e.Id == guid);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets a result set from memory containing all documents for the entity type.
        /// </summary>
        /// <returns>An <see cref="IQueryable{Event}"/> against which further filtering can be applied on the result set.</returns>
        public Task<IQueryable<Event>> GetAll()
        {
            return Task.FromResult(this.Events.Select(this.DeepCopy).AsQueryable());
        }

        /// <summary>
        /// Gets a single instance of T from memory matching the given Id.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <returns>An asynchronous <see cref="Task{Event}"/> containing the entity if found, otherwise null.</returns>
        public Task<Event> GetById(Guid guid)
        {
            return Task.FromResult(this.DeepCopy(this.Events.FirstOrDefault(e => e.Id == guid)));
        }

        /// <summary>
        /// Persists a new document representing <see cref="Event" /> to memory.
        /// </summary>
        /// <param name="entity">The entity to persist to memory.</param>
        /// <returns>An asynchronous <see cref="Task{Event}"/>.</returns>
        public Task Insert(Event entity)
        {
            this.Events.Add(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Overwrites an existing document representing <see ref="Event" /> in memory.
        /// </summary>
        /// <param name="entity">The entity to persist to memory.</param>
        /// <returns>An asynchronous <see cref="Task{Event}"/>.</returns>
        public Task Update(Event entity)
        {
            this.Events.RemoveAll(e => e.Id == entity.Id);
            this.Insert(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches for a newer version of <see ref="Event" /> in memory if it exists and returns it if found.
        /// </summary>
        /// <param name="guid">The globally unique identifier for the entity.</param>
        /// <param name="version">The presumed most recent version of the entity document.</param>
        /// <returns>The newer version of the entity if it exists. Otherwise, null.</returns>
        public async Task<Event> GetNewerVersionIfExists(Guid guid, int version)
        {
            var @eventInMemory = await this.GetById(guid);
            if (@eventInMemory?.Version > version)
            {
                return @eventInMemory;
            }

            return null;
        }

        private Event DeepCopy(Event @event) => @event != null ? this.Mapper.Map<Event, Event>(@event) : null;
    }
}
