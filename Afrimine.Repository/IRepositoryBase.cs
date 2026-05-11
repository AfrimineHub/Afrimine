using System.Linq.Expressions;

namespace Afrimine.Repository
{
    public interface IRepositoryBase<T>
    {
        Task Create(T entity);
        void Delete(T entity);
        IQueryable<T> FindAll(bool trackChanges);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
        void Update(T entity);
    }
}
