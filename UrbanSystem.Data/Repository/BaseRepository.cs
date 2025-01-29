using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UrbanSystem.Data.Repository.Contracts;

namespace UrbanSystem.Data.Repository
{
    public class BaseRepository<TType, TId> : IRepository<TType, TId> where TType : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TType> _dbSet;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TType>();
        }

        public void Add(TType item)
        {
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        public async Task AddAsync(TType item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public bool Delete(TId id)
        {
            TType entity = GetById(id);

            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> DeleteAsync(TId id)
        {
            TType entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Expression<Func<TType, bool>> predicate)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync();
            if (!entities.Any())
            {
                return false;
            }

            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();

            return true;
        }

        public IEnumerable<TType> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TType>> GetAllAsync(Expression<Func<TType, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return _dbSet.AsQueryable();
        }

        public TType GetById(TId id)
        {
            TType? entity = _dbSet.Find(id);

            return entity!;
        }

        public async Task<TType> GetByIdAsync(TId id)
        {
            TType? entity = await _dbSet.FindAsync(id);

            return entity!;
        }

        public bool Update(TType item)
        {
            try
            {
                _dbSet.Attach(item);
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            TType? entity;

            if (id is object[] compositeKey)
            {
                entity = await _dbSet.FindAsync(compositeKey);
            }
            else if (id is ValueTuple<object, object> tupleKey)
            {
                entity = await _dbSet.FindAsync(tupleKey.Item1, tupleKey.Item2);
            }
            else
            {
                entity = await _dbSet.FindAsync(id);
            }

            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdateAsync(TType item)
        {
            try
            {
                _dbSet.Attach(item);
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<int> CountAsync(Expression<Func<TType, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}