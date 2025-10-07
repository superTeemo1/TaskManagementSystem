using System.Collections.ObjectModel;

namespace TaskManagement.Application.Validation
{
    /// <summary>
    /// Aggregates multiple field validation errors so the UI can show them all at once.
    /// </summary>
    public sealed class DomainValidationException : Exception
    {
        public IReadOnlyList<(string Field, string Message)> Errors { get; }

        public DomainValidationException(IEnumerable<(string Field, string Message)> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = new ReadOnlyCollection<(string Field, string Message)>(errors.ToList());
        }
    }
}