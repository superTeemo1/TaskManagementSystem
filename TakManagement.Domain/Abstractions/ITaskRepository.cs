using TakManagement.Domain.Models;

namespace TakManagement.Domain.Abstractions;

public interface ITaskRepository
{
    TaskItem Create(TaskItem item);

    TaskItem? Get(int id);

    IEnumerable<TaskItem> GetAll(string? q = null, Models.TaskStatus? status = null);

    bool Update(TaskItem item);

    bool Delete(int id);
}
