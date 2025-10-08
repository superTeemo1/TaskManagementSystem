using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Abstractions;
using TaskManagement.Domain.Models;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Validation;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;

    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Creates a new task item from the provided DTO.
    /// </summary>
    /// <param name="dto">Task creation data transfer object</param>
    /// <returns>The created <see cref="TaskItem"/></returns>
    /// <exception cref="ValidationException">If input is invalid.</exception>
    public TaskItem Create(TaskCreateDto dto)
    {
        if (dto is null)
        {
            throw new ValidationException("Payload is required.");
        }

        var errors = new List<(string Field, string Message)>();
        var title = (dto.Title ?? string.Empty).Trim();

        // Field validations
        if (title.Length is < 3 or > 100)
        {
            errors.Add((nameof(dto.Title), "Title must be between 3 and 100 characters."));
        }

        if (dto.Description is { Length: > 1000 })
        {
            errors.Add((nameof(dto.Description), "Description can be up to 1000 characters."));
        }

        if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            errors.Add((nameof(dto.DueDate), "Due date cannot be in the past."));
        }

        if (!Enum.IsDefined(typeof(Domain.Models.TaskStatus), dto.Status))
        {
            errors.Add((nameof(dto.Status), "Invalid task status."));
        }

        // Business rule: unique title
        if (!string.IsNullOrWhiteSpace(title))
        {
            var duplicate = _repo.GetAll()
                .Any(t => string.Equals(t.Title, title, StringComparison.OrdinalIgnoreCase));
            if (duplicate)
                errors.Add((nameof(dto.Title), "A task with the same title already exists."));
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        var item = new TaskItem
        {
            Title = title,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DueDate = dto.DueDate,
            Status = dto.Status
        };

        return _repo.Create(item);
    }

    /// <summary>
    /// Updates an existing task item with new values from the provided DTO.
    /// </summary>
    /// <param name="id">Task identifier</param>
    /// <param name="dto">Task update data transfer object</param>
    /// <returns>True if the update was successful; otherwise, false</returns>
    /// <exception cref="ValidationException">If input is invalid.</exception>
    public bool Update(int id, TaskUpdateDto dto)
    {
        if (id <= 0)
        {
            throw new ValidationException("Id must be a positive integer.");
        }

        if (dto is null)
        {
            throw new ValidationException("Payload is required.");
        }

        var errors = new List<(string Field, string Message)>();
        var title = (dto.Title ?? string.Empty).Trim();

        if (title.Length is < 3 or > 100)
        {
            errors.Add((nameof(dto.Title), "Title must be between 3 and 100 characters."));
        }

        if (dto.Description is { Length: > 1000 })
        {
            errors.Add((nameof(dto.Description), "Description can be up to 1000 characters."));
        }

        if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            errors.Add((nameof(dto.DueDate), "Due date cannot be in the past."));
        }

        if (!Enum.IsDefined(typeof(Domain.Models.TaskStatus), dto.Status))
        {
            errors.Add((nameof(dto.Status), "Invalid task status."));
        }

        var existing = _repo.Get(id);
        if (existing is null)
        { 
            return false;
        }

        // Business rule: unique title (excluding current entity)
        if (!string.IsNullOrWhiteSpace(title))
        {
            var duplicate = _repo.GetAll()
                .Any(t => t.Id != id &&
                          string.Equals(t.Title, title, StringComparison.OrdinalIgnoreCase));
            if (duplicate)
                errors.Add((nameof(dto.Title), "A task with the same title already exists."));
        }

        if (errors.Count > 0)
        {
            throw new DomainValidationException(errors);
        }

        existing.Title = title;
        existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        existing.DueDate = dto.DueDate;
        existing.Status = dto.Status;

        return _repo.Update(existing);
    }

    /// <summary>
    /// Retrieves a task item by its unique identifier.
    /// </summary>
    /// <param name="id">Task identifier</param>
    /// <returns>The TaskItem if found; otherwise, null</returns>
    /// <exception cref="ValidationException">If id is invalid.</exception>
    public TaskItem? Get(int id)
    {
        if (id <= 0)
        {
            throw new ValidationException("Id must be a positive integer.");
        }

        return _repo.Get(id);
    }

    /// <summary>
    /// Returns all tasks, optionally filtered by text or status.
    /// </summary>
    /// <param name="q">Optional text query</param>
    /// <param name="status">Optional status filter</param>
    /// <returns>Filtered task list</returns>
    public IEnumerable<TaskItem> GetAll(string? q = null, TaskManagement.Domain.Models.TaskStatus? status = null)
        => _repo.GetAll(q, status);

    /// <summary>
    /// Deletes a task item by its unique identifier.
    /// </summary>
    /// <param name="id">Task identifier</param>
    /// <returns>True if the deletion was successful; otherwise, false</returns>
    /// <exception cref="ValidationException">If id is invalid.</exception>
    public bool Delete(int id)
    {
        if (id <= 0)
        {
            throw new ValidationException("Id must be a positive integer.");
        }

        return _repo.Delete(id);
    }
}
