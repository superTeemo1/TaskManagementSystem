using System.Collections.Concurrent;
using TaskManagement.Domain.Abstractions;
using TaskManagement.Domain.Models;

namespace TaskManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Thread-safe in-memory repository using ConcurrentDictionary.
    /// Holds repository-level invariants (argument guards) while business rules live in the service layer.
    /// </summary>
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<int, TaskItem> _store = new();
        private int _id = 0;

        /// <summary>
        /// Creates a new TaskItem, assigns a unique ID, and stores it in memory.
        /// </summary>
        /// <param name="item">The TaskItem to create</param>
        /// <returns>The created TaskItem with assigned ID and timestamps</returns>
        public TaskItem Create(TaskItem item)
        {
            // Validate repository invariants
            Guard(item);

            // Thread-safe unique id assignment
            var id = Interlocked.Increment(ref _id);
            item.Id = id;

            var now = DateTime.UtcNow;
            item.CreatedAtUtc = now;
            item.UpdatedAtUtc = now;

            _store[id] = item;
            return item;
        }

        /// <summary>
        /// Retrieves a TaskItem by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the TaskItem</param>
        /// <returns>The TaskItem if found; otherwise, null</returns>
        public TaskItem? Get(int id)
        {
            // Treat invalid ids as not found (do not throw)
            if (id <= 0) return null;
            return _store.TryGetValue(id, out var item) ? item : null;
        }

        /// <summary>
        /// Returns all TaskItems, optionally filtered by a text query and/or status.
        /// </summary>
        /// <param name="q">Optional text query to filter by title or description</param>
        /// <param name="status">Optional status filter</param>
        /// <returns>Filtered and ordered list of TaskItems</returns>
        public IEnumerable<TaskItem> GetAll(string? q = null, Domain.Models.TaskStatus? status = null)
        {
            IEnumerable<TaskItem> values = _store.Values;

            // Case-insensitive text filtering on Title/Description
            if (!string.IsNullOrWhiteSpace(q))
            {
                var needle = q.Trim().ToLowerInvariant();
                values = values.Where(t =>
                    (t.Title?.ToLowerInvariant().Contains(needle) ?? false) ||
                    (t.Description?.ToLowerInvariant().Contains(needle) ?? false));
            }

            if (status.HasValue)
            {
                values = values.Where(t => t.Status == status.Value);
            }

            // Newest first
            return values.OrderByDescending(t => t.UpdatedAtUtc);
        }

        /// <summary>
        /// Updates an existing TaskItem in memory.
        /// </summary>
        /// <param name="item">The TaskItem with updated values</param>
        /// <returns>True if the update was successful; otherwise, false</returns>
        public bool Update(TaskItem item)
        {
            // Validate invariants and require a valid Id on update
            Guard(item, requireId: true);

            if (!_store.ContainsKey(item.Id))
            {
                return false;
            }

            item.UpdatedAtUtc = DateTime.UtcNow;
            _store[item.Id] = item;

            return true;
        }

        /// <summary>
        /// Deletes a TaskItem by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the TaskItem to delete</param>
        /// <returns>True if the deletion was successful; otherwise, false</returns>
        public bool Delete(int id)
        {
            if (id <= 0) return false;
            return _store.TryRemove(id, out _);
        }

        /// <summary>
        /// Validates repository-level invariants and throws ArgumentException on violation.
        /// This protects the store from obviously invalid entities regardless of service logic.
        /// </summary>
        private static void Guard(TaskItem? item, bool requireId = false)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null.");
            }

            // Id must be provided (and positive) for updates
            if (requireId && item.Id <= 0)
            {
                throw new ArgumentException("Valid Id is required for update.", nameof(item.Id));
            }

            // Title: required and length constraints
            var title = item.Title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required.", nameof(item.Title));
            }

            if (title.Length < 3 || title.Length > 100)
            {
                throw new ArgumentException("Title must be between 3 and 100 characters.", nameof(item.Title));
            }

            // Description: optional, but capped
            if (item.Description is not null && item.Description.Length > 1000)
            {
                throw new ArgumentException("Description can be up to 1000 characters.", nameof(item.Description));
            }

            // Status must be a valid enum value
            if (!Enum.IsDefined(typeof(Domain.Models.TaskStatus), item.Status))
            {
                throw new ArgumentException("Invalid task status.", nameof(item.Status));
            }

            // Due date cannot be in the past (date-only comparison)
            if (item.DueDate.HasValue && item.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Due date cannot be in the past.", nameof(item.DueDate));
            }

            // Normalize persisted fields
            item.Title = title;
            if (string.IsNullOrWhiteSpace(item.Description))
            {
                item.Description = null;
            }
        }
    }
}