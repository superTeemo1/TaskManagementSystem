using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validation;
using TaskManagement.Domain.Models;

namespace TaskManagement.Web.Controllers
{
    /// <summary>
    /// Controller for managing tasks in the web application.
    /// </summary>
    public class TasksController : Controller
    {
        private readonly ITaskService _service;
        private readonly ILogger<TasksController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TasksController"/> class.
        /// </summary>
        /// <param name="service">The task service to use for task operations.</param>
        /// <param name="logger">Logger instance for diagnostics and error reporting.</param>
        public TasksController(ITaskService service, ILogger<TasksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of tasks, optionally filtered by query and status.
        /// </summary>
        public IActionResult Index(string? q, Domain.Models.TaskStatus? status)
        {
            ViewData["q"] = q;
            ViewData["status"] = status;
            var items = _service.GetAll(q, status);
            return View(items);
        }

        /// <summary>
        /// Displays the details of a specific task.
        /// </summary>
        public IActionResult Details(int id)
        {
            var item = _service.Get(id);
            if (item is null)
            {
                TempData["FlashWarn"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        /// <summary>
        /// Displays the create task form.
        /// </summary>
        [HttpGet]
        public IActionResult Create() => View(new TaskCreateDto());

        /// <summary>
        /// Handles the creation of a new task.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var created = _service.Create(dto);
                TempData["Flash"] = "Task created successfully.";
                _logger.LogInformation("Task {Id} created successfully", created.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DomainValidationException vex)
            {
                // Map every field error to ModelState -> messages appear next to inputs
                foreach (var (field, message) in vex.Errors)
                {
                    // field is like nameof(dto.DueDate) etc.
                    ModelState.AddModelError(field, message);
                    _logger.LogWarning("Validation error creating task: {Field} - {Message}", field, message);
                }
                return View(dto);
            }
            catch (ValidationException ex)
            {
                // Fallback for non-field validations (guards)
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError(ex, "Validation error creating task");
                return View(dto);
            }
        }

        /// <summary>
        /// Displays the edit form for a specific task.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _service.Get(id);
            if (item is null)
            {
                TempData["FlashWarn"] = "Task not found.";
                _logger.LogWarning("Attempted to edit non-existing task {Id}", id);
                return RedirectToAction(nameof(Index));
            }

            var dto = new TaskUpdateDto
            {
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                Status = item.Status
            };

            return View(dto);
        }

        /// <summary>
        /// Handles the update of a specific task.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaskUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = _service.Update(id, dto);
                if (!updated)
                {
                    TempData["FlashWarn"] = "Task not found.";
                    _logger.LogWarning("Attempted to update non-existing task {Id}", id);
                    return RedirectToAction(nameof(Index));
                }

                TempData["Flash"] = "Task updated successfully.";
                _logger.LogInformation("Task {Id} updated successfully", id);

                return RedirectToAction(nameof(Index));
            }
            catch (DomainValidationException vex)
            {
                foreach (var (field, message) in vex.Errors)
                {
                    ModelState.AddModelError(field, message);
                    _logger.LogWarning("Validation error updating task {Id}: {Field} - {Message}", id, field, message);
                }
                return View(dto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError(ex, "Validation error updating task {Id}", id);
                return View(dto);
            }
        }

        /// <summary>
        /// Handles the deletion of a specific task.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _service.Delete(id);
                TempData[deleted ? "Flash" : "FlashWarn"] = deleted
                    ? "Task deleted successfully."
                    : "Task not found.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {Id}", id);
                TempData["FlashError"] = "Unexpected error while deleting task.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}