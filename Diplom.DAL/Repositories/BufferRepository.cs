using Diplom.DAL.Interfaces;
using Buffer = Diplom.Domain.Entity.Buffer;

namespace Diplom.DAL.Repositories;

public class BufferRepository : IBaseRepository<Buffer>
{
    private readonly ApplicationDbContext _db;
    
    public BufferRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task Create(Buffer entity)
    {
        await _db.Buffers.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public IQueryable<Buffer> GetAll()
    {
        return _db.Buffers;
    }

    public async Task Delete(Buffer entity)
    {
        _db.Buffers.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<Buffer> Update(Buffer entity)
    {
        _db.Buffers.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }
}