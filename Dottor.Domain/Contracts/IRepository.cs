using Dottor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottor.Domain.Contracts
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get();
        T Get(int id);
        void Post(T t);
        void Put(T t);
        void Delete(T t);
    }
}
