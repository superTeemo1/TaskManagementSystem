using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public class TaskUpdateDto
{
    [Required, StringLength(100)] 
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)] 
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Required] 
    public TakManagement.Domain.Models.TaskStatus Status { get; set; }
}
