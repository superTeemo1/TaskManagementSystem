using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Domain.Models;
using TaskManagement.Infrastructure.Repositories;
using Xunit;

namespace TaskManagement.Tests
{
    public class RepositoryInvariantTests
    {
        private static InMemoryTaskRepository Repo() => new InMemoryTaskRepository();

        [Fact]
        public void Create_Assigns_Id_And_Timestamps()
        {
            var repo = Repo();

            var created = repo.Create(new TaskItem
            {
                Title = "First task",
                Status = Domain.Models.TaskStatus.New
            });

            Assert.True(created.Id > 0);
            Assert.True((DateTime.UtcNow - created.CreatedAtUtc).TotalSeconds < 5);
            Assert.Equal(created.CreatedAtUtc, created.UpdatedAtUtc);
        }

        [Fact]
        public void Get_Returns_Item_And_Null_For_Invalid_Id()
        {
            var repo = Repo();
            var created = repo.Create(new TaskItem { Title = "Take a rest", Status = Domain.Models.TaskStatus.New });

            Assert.NotNull(repo.Get(created.Id));
            Assert.Null(repo.Get(0));
            Assert.Null(repo.Get(-1));
            Assert.Null(repo.Get(created.Id + 999));
        }

        [Fact]
        public void Update_Modifies_Existing_And_Sets_UpdatedAt()
        {
            var repo = Repo();
            var created = repo.Create(new TaskItem { Title = "Old", Status = Domain.Models.TaskStatus.New });

            var before = created.UpdatedAtUtc;
            created.Title = "New";
            created.Status = Domain.Models.TaskStatus.InProgress;

            var ok = repo.Update(created);
            var fetched = repo.Get(created.Id);

            Assert.True(ok);
            Assert.Equal("New", fetched!.Title);
            Assert.Equal(Domain.Models.TaskStatus.InProgress, fetched.Status);
            Assert.True(fetched.UpdatedAtUtc >= before);
        }

        [Fact]
        public void Update_ReturnsFalse_When_NotFound()
        {
            var repo = Repo();
            var phantom = new TaskItem { Id = 999, Title = "Test title", Status = Domain.Models.TaskStatus.New };

            var ok = repo.Update(phantom);

            Assert.False(ok);
        }

        [Fact]
        public void Delete_ReturnsTrue_When_Existing_Else_False()
        {
            var repo = Repo();
            var a = repo.Create(new TaskItem { Title = "Alpha", Status = Domain.Models.TaskStatus.New });

            Assert.True(repo.Delete(a.Id));
            Assert.False(repo.Delete(a.Id)); // already deleted
            Assert.False(repo.Delete(0));    // invalid id
        }

        [Fact]
        public void GetAll_Supports_Text_And_Status_Filtering_And_Sorts_By_UpdatedAt_Desc()
        {
            var repo = Repo();
            var t1 = repo.Create(new TaskItem { Title = "Buy milk", Description = "2% organic", Status = Domain.Models.TaskStatus.New });
            Thread.Sleep(5); // ensure different timestamps
            var t2 = repo.Create(new TaskItem { Title = "Buy bread", Status = Domain.Models.TaskStatus.InProgress });
            Thread.Sleep(5);
            var t3 = repo.Create(new TaskItem { Title = "Call mom", Status = Domain.Models.TaskStatus.Done });

            // Text filter (case-insensitive)
            var textFiltered = repo.GetAll("buy", null).ToList();
            Assert.Equal(2, textFiltered.Count);
            Assert.All(textFiltered, x => Assert.Contains("buy", x.Title, StringComparison.OrdinalIgnoreCase));

            // Status filter
            var done = repo.GetAll(null, Domain.Models.TaskStatus.Done).ToList();
            Assert.Single(done);
            Assert.Equal(Domain.Models.TaskStatus.Done, done[0].Status);

            // Sort by UpdatedAt desc (touch t1 to make it most recent)
            t1.Title = "Buy milk (updated)";
            repo.Update(t1);
            var all = repo.GetAll().ToList();
            Assert.Equal(t1.Id, all.First().Id);
        }

        [Fact]
        public void Guard_Throws_On_Invalid_Entities()
        {
            var repo = Repo();

            // Empty title
            Assert.Throws<ArgumentException>(() =>
                repo.Create(new TaskItem { Title = "  ", Status = Domain.Models.TaskStatus.New }));

            // Title too short
            Assert.Throws<ArgumentException>(() =>
                repo.Create(new TaskItem { Title = "ab", Status = Domain.Models.TaskStatus.New }));

            // Description too long
            var longDesc = new string('x', 1001);
            Assert.Throws<ArgumentException>(() =>
                repo.Create(new TaskItem { Title = "Valid", Description = longDesc, Status = Domain.Models.TaskStatus.New }));

            // Due date in the past
            Assert.Throws<ArgumentException>(() =>
                repo.Create(new TaskItem { Title = "Valid", Status = Domain.Models.TaskStatus.New, DueDate = DateTime.UtcNow.AddDays(-1) }));

            // Update requires valid Id (> 0)
            Assert.Throws<ArgumentException>(() =>
                repo.Update(new TaskItem { Id = 0, Title = "Valid", Status = Domain.Models.TaskStatus.New }));
        }

        [Fact]
        public void Concurrent_Creates_Produce_Unique_Ids()
        {
            var repo = Repo();
            var ids = new int[200];

            Parallel.For(0, ids.Length, i =>
            {
                var created = repo.Create(new TaskItem { Title = $"T{i:000}", Status = Domain.Models.TaskStatus.New });
                ids[i] = created.Id;
            });

            Assert.Equal(ids.Length, ids.Distinct().Count());
        }
    }
}
