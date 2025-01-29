using System.Linq.Expressions;

namespace UrbanSystem.Data.Repository.Contracts
{
    public interface IRepository<TType, TId>
    {
        TType GetById(TId id);

        Task<TType> GetByIdAsync(TId id);

        IEnumerable<TType> GetAll();

        Task<IEnumerable<TType>> GetAllAsync();

        Task<IEnumerable<TType>> GetAllAsync(Expression<Func<TType, bool>> predicate);

        IQueryable<TType> GetAllAttached();

        void Add(TType item);

        Task AddAsync(TType item);

        bool Delete(TId id);

        Task<bool> DeleteAsync(Expression<Func<TType, bool>> predicate);
        Task<bool> DeleteAsync(object id);

        Task<bool> DeleteAsync(TId id);

        bool Update(TType type);

        Task<bool> UpdateAsync(TType type);
    }
}