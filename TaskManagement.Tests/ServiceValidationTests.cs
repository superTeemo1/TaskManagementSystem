using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validation;
using TaskManagement.Domain.Models;
using TaskManagement.Infrastructure.Repositories;
using Xunit;

namespace TaskManagement.Tests
{
    public class ServiceValidationTests
    {
        private TaskService MakeService()
            => new TaskService(new InMemoryTaskRepository());

        [Fact]
        public void Create_Succeeds_With_MinimalValidPayload()
        {
            var svc = MakeService();
            var created = svc.Create(new TaskCreateDto
            {
                Title = "Task A",
                Status = TaskManagement.Domain.Models.TaskStatus.New
            });
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Task A", created.Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("ab")] // < 3 chars
        public void Create_Fails_When_Title_Invalid(string? title)
        {
            var svc = MakeService();
            var dto = new TaskCreateDto { Title = title ?? "", Status = TaskManagement.Domain.Models.TaskStatus.New };
            Assert.Throws<DomainValidationException>(() => svc.Create(dto));
        }

        [Fact]
        public void Create_Fails_When_Description_TooLong()
        {
            var svc = MakeService();
            var longDesc = new string('x', 1001);
            var dto = new TaskCreateDto { Title = "X", Description = longDesc, Status = TaskManagement.Domain.Models.TaskStatus.New };
            Assert.Throws<DomainValidationException>(() => svc.Create(dto));
        }

        [Fact]
        public void Create_Fails_When_DueDate_In_Past()
        {
            var svc = MakeService();
            var dto = new TaskCreateDto { Title = "X", Status = TaskManagement.Domain.Models.TaskStatus.New, DueDate = DateTime.UtcNow.Date.AddDays(-1) };
            Assert.Throws<DomainValidationException>(() => svc.Create(dto));
        }

        [Fact]
        public void Create_Fails_When_Duplicate_Title()
        {
            var svc = MakeService();
            svc.Create(new TaskCreateDto { Title = "Unique", Status = TaskManagement.Domain.Models.TaskStatus.New });
            Assert.Throws<DomainValidationException>(() =>
                svc.Create(new TaskCreateDto { Title = "unique", Status = TaskManagement.Domain.Models.TaskStatus.New }) // case-insensitive
            );
        }

        [Fact]
        public void Get_Throws_For_Invalid_Id()
        {
            var svc = MakeService();
            Assert.Throws<ValidationException>(() => svc.Get(0));
            Assert.Throws<ValidationException>(() => svc.Get(-5));
        }

        [Fact]
        public void Update_ReturnsFalse_When_NotFound()
        {
            var svc = new TaskService(new InMemoryTaskRepository());

            // Use a positive, non-existing id and a valid DTO so we reach the "not found" branch.
            var ok = svc.Update(999, new TaskUpdateDto
            {
                Title = "Valid Title", // >= 3 chars
                Status = TaskManagement.Domain.Models.TaskStatus.InProgress
            });

            Assert.False(ok);
        }

        [Fact]
        public void Update_Fails_For_Invalid_Id_Or_Title()
        {
            var svc = new TaskService(new InMemoryTaskRepository());

            // Invalid id -> should throw ValidationException
            Assert.Throws<ValidationException>(() =>
                svc.Update(0, new TaskUpdateDto { Title = "Valid Title", Status = TaskManagement.Domain.Models.TaskStatus.New })
            );

            // Create a valid task first (>= 3 chars)
            var created = svc.Create(new TaskCreateDto { Title = "Alpha", Status = TaskManagement.Domain.Models.TaskStatus.New });

            // Then attempt to update with an invalid title (< 3 chars) -> should throw
            Assert.Throws<DomainValidationException>(() =>
                svc.Update(created.Id, new TaskUpdateDto { Title = "ab", Status = TaskManagement.Domain.Models.TaskStatus.New })
            );
        }

        [Fact]
        public void Update_Fails_When_Duplicate_Title()
        {
            var svc = MakeService();
            var a = svc.Create(new TaskCreateDto { Title = "Alpha", Status = TaskManagement.Domain.Models.TaskStatus.New });
            var b = svc.Create(new TaskCreateDto { Title = "Bravo", Status = TaskManagement.Domain.Models.TaskStatus.New });

            // Try to rename Bravo -> Alpha
            Assert.Throws<DomainValidationException>(() =>
                svc.Update(b.Id, new TaskUpdateDto { Title = "alpha", Status = TaskManagement.Domain.Models.TaskStatus.New })
            );
        }

        [Fact]
        public void Delete_Throws_For_Invalid_Id()
        {
            var svc = MakeService();
            Assert.Throws<ValidationException>(() => svc.Delete(0));
        }

        [Fact]
        public void GetAll_Filters_By_Text_And_Status()
        {
            var svc = MakeService();
            var t1 = svc.Create(new TaskCreateDto { Title = "Buy milk", Description = "2% organic", Status = TaskManagement.Domain.Models.TaskStatus.New });
            var t2 = svc.Create(new TaskCreateDto { Title = "Buy bread", Status = TaskManagement.Domain.Models.TaskStatus.InProgress });
            var t3 = svc.Create(new TaskCreateDto { Title = "Call mom and dad", Status = TaskManagement.Domain.Models.TaskStatus.Done });

            var textFiltered = svc.GetAll("buy", null).ToList();
            Assert.Equal(2, textFiltered.Count);
            Assert.All(textFiltered, x => Assert.Contains("buy", x.Title, StringComparison.OrdinalIgnoreCase));

            var statusFiltered = svc.GetAll(null, TaskManagement.Domain.Models.TaskStatus.Done).ToList();
            Assert.NotEmpty(statusFiltered);
            Assert.All(statusFiltered, x => Assert.Equal(TaskManagement.Domain.Models.TaskStatus.Done, x.Status));
            Assert.Contains(statusFiltered, x => x.Id == t3.Id); // robustno — poredi po ID-u
        }
    }
}
