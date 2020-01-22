using fursvp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fursvp.data
{
    public interface IRepository<T> where T : IEntity<T>
    {
        Task<IQueryable<T>> GetAll();
        Task<T> GetById(Guid guid);
        Task Insert(T entity);
        Task Update(T entity);
        Task Delete(Guid guid);
    }
}
