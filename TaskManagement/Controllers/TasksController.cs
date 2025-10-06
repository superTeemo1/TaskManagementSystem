using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;

namespace TaskManagement.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _service;
        public TasksController(ITaskService service) => _service = service;

        // LIST + filteri
        public IActionResult Index(string? q, TakManagement.Domain.Models.TaskStatus? status)
        {
            ViewData["q"] = q;
            ViewData["status"] = status;

            var items = _service.GetAll(q,  status);

            return View(items);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var item = _service.Get(id);
            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create() => View(new TaskCreateDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var created = _service.Create(dto);
            TempData["Flash"] = "Task created.";

            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        // EDIT
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _service.Get(id);

            if (item is null)
            {
                return NotFound();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaskUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var ok = _service.Update(id, dto);
            if (!ok)
            {
                return NotFound();
            }

            TempData["Flash"] = "Task updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ok = _service.Delete(id);

            TempData["Flash"] = ok ? "Task deleted." : "Task not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}
