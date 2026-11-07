using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Afrimine.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class 
    {
        protected AppDbContext AppDbContext;
        protected RepositoryBase(AppDbContext appDbContext)
            => AppDbContext = appDbContext;

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
            AppDbContext.Set<T>()
            .AsNoTracking() :
            AppDbContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
            !trackChanges ?
            AppDbContext.Set<T>()
            .Where(expression)
            .AsNoTracking() :
            AppDbContext.Set<T>()
            .Where(expression);

        public async Task Create(T entity) => await AppDbContext.Set<T>().AddAsync(entity);
        public void Update (T entity) => AppDbContext.Set<T>().Update(entity);
        public void Delete(T entity) => AppDbContext.Set<T>().Remove(entity);
        
    }
}
