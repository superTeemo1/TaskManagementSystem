using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.DTOs;

public interface ITaskForm
{
    [Required, StringLength(100, MinimumLength = 3)]
    string Title { get; set; }

    [StringLength(1000)]
    string? Description { get; set; }

    DateTime? DueDate { get; set; }

    [Required]
    Domain.Models.TaskStatus Status { get; set; }
}
