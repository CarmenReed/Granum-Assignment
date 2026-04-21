using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class InteractionRepository
{
    private readonly AppDbContext _db;

    public InteractionRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(InteractionLog log, CancellationToken ct = default)
    {
        _db.Interactions.Add(log);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<InteractionLog> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Interactions.AsNoTracking().OrderByDescending(x => x.Timestamp);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
