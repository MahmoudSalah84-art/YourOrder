using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity;
using yourOrder.Core.Specifications;

namespace yourOrder.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        #region Without Specifications
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        #endregion


        #region With Specifications
        Task<IEnumerable<T>> GetAllWithSpec(ISpecification<T> spec);
        Task<T> GetByIdWithSpec(ISpecification<T> spec);
        #endregion


        Task<int> GetCountAsync(ISpecification<T> spec);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);


    }
}
