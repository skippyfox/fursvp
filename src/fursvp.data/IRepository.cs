using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fursvp.data
{
    public interface IRepository<T> where T : IEntity<T>
    {
        IQueryable<T> GetAll();
        T GetById(Guid guid);
        void Insert(T entity);
        void Update(T entity);
        void Delete(Guid guid);
    }
}
