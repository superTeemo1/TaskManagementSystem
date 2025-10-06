using System.Collections.Concurrent;
using TakManagement.Domain.Abstractions;
using TakManagement.Domain.Models;

namespace TaskManagement.Infrastructure.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<int, TaskItem> _store = new();
    private int _id = 0;

    public TaskItem Create(TaskItem item)
    {
        var id = System.Threading.Interlocked.Increment(ref _id);
        item.Id = id;
        item.CreatedAtUtc = DateTime.UtcNow;
        item.UpdatedAtUtc = item.CreatedAtUtc;
        _store[id] = item;
        return item;
    }

    public TaskItem? Get(int id) => _store.TryGetValue(id, out var item) ? item : null;

    public IEnumerable<TaskItem> GetAll(string? q = null, TakManagement.Domain.Models.TaskStatus? status = null)
    {
        var values = _store.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLowerInvariant();

            values = values.Where(t =>
                (t.Title?.ToLowerInvariant().Contains(q) ?? false) ||
                (t.Description?.ToLowerInvariant().Contains(q) ?? false));
        }

        if (status.HasValue)
        {
            values = values.Where(t => t.Status == status.Value);
        }

        return values.OrderByDescending(t => t.UpdatedAtUtc);
    }

    public bool Update(TaskItem item)
    {
        if (!_store.ContainsKey(item.Id))
        {
            return false;
        }

        item.UpdatedAtUtc = DateTime.UtcNow;
        _store[item.Id] = item;

        return true;
    }

    public bool Delete(int id) => _store.TryRemove(id, out _);
}