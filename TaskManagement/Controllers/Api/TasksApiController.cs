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
        public TasksApiController(ITaskService service) => _service = service;

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? q, [FromQuery] TakManagement.Domain.Models.TaskStatus? status)
            => Ok(_service.GetAll(q, status));

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
            => _service.Get(id) is { } item ? Ok(item) : NotFound();

        [HttpPost]
        public IActionResult Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            return _service.Update(id, dto) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
            => _service.Delete(id) ? NoContent() : NotFound();
    }
}
