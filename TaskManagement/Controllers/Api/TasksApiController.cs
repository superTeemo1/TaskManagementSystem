using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;

namespace TaskManagement.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksApiController : ControllerBase
    {
        private readonly ITaskService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TasksApiController"/> class.
        /// </summary>
        /// <param name="service">The task service to use for task operations.</param>
        public TasksApiController(ITaskService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all tasks, optionally filtered by query and status.
        /// </summary>
        /// <param name="q">Optional text query to filter tasks.</param>
        /// <param name="status">Optional status filter.</param>
        /// <returns>A list of tasks matching the filter criteria.</returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? q, [FromQuery] TaskManagement.Domain.Models.TaskStatus? status)
        {
            var result = _service.GetAll(q, status);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific task by its ID.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The task if found; otherwise, NotFound.</returns>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var item = _service.Get(id);

            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="dto">The task creation data transfer object.</param>
        /// <returns>The created task with its location.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var created = _service.Create(dto);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="dto">The task update data transfer object.</param>
        /// <returns>NoContent if successful; otherwise, NotFound.</returns>
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid) 
            {
                return ValidationProblem(ModelState);
            }

            return _service.Update(id, dto) ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>NoContent if successful; otherwise, NotFound.</returns>
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }
    }
}
