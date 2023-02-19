using System.Collections.Generic;
using System.Linq.Expressions;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InvestmentTracking.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;
    private readonly DbContext _dbContext;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(DbContext dbContext, ILogger<Repository<T>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
        _logger = logger;
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        _logger.LogDebug("Added entity {@Entity} to DbSet", entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        _logger.LogDebug("Added entities {@Entities} to DbSet", entities);
    }

    public async Task DeleteAsync(T entity)
    {
        await Task.Run(() => _dbSet.Remove(entity));
        _logger.LogDebug("Deleted entity {@Entity} from DbSet", entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.RemoveRange(entities));
        _logger.LogDebug("Deleted entities {@Entities} from DbSet", entities);
    }

    public async Task<T?> GetAsync(Guid id)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
        if (entity != null)
        {
            _logger.LogDebug("Retrieved entity {@Entity} from DbSet by Id {Id}", entity, id);
        }
        else
        {
            _logger.LogDebug("No entity with Id {Id} found in DbSet", id);
        }

        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        _logger.LogDebug("Retrieved all entities {@Entities} from DbSet", entities);
        return entities;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync();
        _logger.LogDebug("Retrieved entities {@Entities} from DbSet with predicate {@Predicate}", entities, predicate);
        return entities;
    }

    public async Task UpdateAsync(T entity)
    {
        await Task.Run(() => _dbSet.Update(entity));
        _logger.LogDebug("Updated entity {@Entity} in DbSet", entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _dbSet.UpdateRange(entities));
        _logger.LogDebug("Updated entities {@Entities} in DbSet", entities);
    }

    public async Task SaveChangesAsync()
    {
       await _dbContext.SaveChangesAsync();
       _logger.LogDebug("Saved changes to database");
    }
}