using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakManagement.Domain.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required, StringLength(100)] 
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)] 
    public string? Description { get; set; }

    [DataType(DataType.Date)] 
    public DateTime? DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.New;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
