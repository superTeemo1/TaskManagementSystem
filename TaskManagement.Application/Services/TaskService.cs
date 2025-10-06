using TakManagement.Domain.Abstractions;
using TakManagement.Domain.Models;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }

    public TaskItem Create(TaskCreateDto dto)
    {
        var item = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DueDate = dto.DueDate,
            Status = dto.Status
        };
        return _repo.Create(item);
    }

    public TaskItem? Get(int id) => _repo.Get(id);

    public IEnumerable<TaskItem> GetAll(string? q = null, TakManagement.Domain.Models.TaskStatus? status = null) => _repo.GetAll(q, status);

    public bool Update(int id, TaskUpdateDto dto)
    {
        var existing = _repo.Get(id);
        if (existing is null) return false;
        existing.Title = dto.Title.Trim();
        existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        existing.DueDate = dto.DueDate;
        existing.Status = dto.Status;
        return _repo.Update(existing);
    }

    public bool Delete(int id) => _repo.Delete(id);
}
