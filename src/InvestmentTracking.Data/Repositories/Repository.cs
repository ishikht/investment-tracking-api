using System.Collections.Generic;
using System.Linq.Expressions;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracking.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;
    private readonly DbContext _dbContext;

    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task DeleteAsync(T entity)
    {
        await Task.Run(() => _dbSet.Remove(entity));
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.RemoveRange(entities));
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await Task.Run(() => _dbSet.Update(entity));
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.UpdateRange(entities));
    }

    public async Task SaveChangesAsync()
    {
       await _dbContext.SaveChangesAsync();
    }
}