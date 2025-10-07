using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public class TaskCreateDto : ITaskForm
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Required] 
    public TaskManagement.Domain.Models.TaskStatus Status { get; set; } = TaskManagement.Domain.Models.TaskStatus.New;
}
