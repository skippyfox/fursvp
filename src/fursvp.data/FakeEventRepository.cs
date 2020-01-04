using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.data
{
    public class FakeEventRepository : IRepository<Event>
    {
        private List<Event> _events { get; } = new List<Event>();

        public void Delete(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Event> GetAll()
        {
            return _events.AsQueryable();
        }

        public Event GetById(Guid guid)
        {
            return _events.FirstOrDefault(e => e.Id == guid);
        }

        public void Insert(Event entity)
        {
            _events.Add(entity);
        }

        public void Update(Event entity)
        {
            _events.RemoveAll(e => e.Id == entity.Id);
            Insert(entity);
        }
    }
}
