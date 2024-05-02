using Diplom.DAL.Interfaces;
using Diplom.Domain.Entity;

namespace Diplom.DAL.Repositories;

public class AccessPermissionRepository : IBaseRepository<AccessPermission>
{
    private readonly ApplicationDbContext _db;
    
    public AccessPermissionRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task Create(AccessPermission entity)
    {
        await _db.AccessPermissions.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<AccessPermission> GetAll()
    {
        return _db.AccessPermissions;
    }

    public async Task Delete(AccessPermission entity)
    {
        _db.AccessPermissions.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<AccessPermission> Update(AccessPermission entity)
    {
        _db.AccessPermissions.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }
}