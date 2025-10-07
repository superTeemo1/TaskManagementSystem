using TaskManagement.Domain.Models;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Services;

public interface ITaskService
{
    TaskItem Create(TaskCreateDto dto);

    TaskItem? Get(int id);

    IEnumerable<TaskItem> GetAll(string? q = null, TaskManagement.Domain.Models.TaskStatus? status = null);

    bool Update(int id, TaskUpdateDto dto);

    bool Delete(int id);
}
