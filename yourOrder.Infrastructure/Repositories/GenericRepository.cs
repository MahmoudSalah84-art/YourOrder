using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity;
using yourOrder.Core.Interfaces;
using yourOrder.Infrastructure.Data;

namespace yourOrder.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }


       

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id).AsTask();
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }




        public Task Add(T entity)
        {
            throw new NotImplementedException();
        }
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
        public Task Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
