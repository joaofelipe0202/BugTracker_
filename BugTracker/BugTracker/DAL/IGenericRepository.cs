using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.DAL
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetEntity(int id);
        T GetEntity(string id);
        T GetEntity(Func<T, bool> condition);
        IEnumerable<T> GetCollection(Func<T, bool> condition);
    }
}
