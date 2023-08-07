using System.ComponentModel.DataAnnotations;

namespace TodoApp
{
    // TodoItem.cs
    public class TodoItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
        public int Priority { get; set; }

        [EnumDataType(typeof(TaskStatus), ErrorMessage = "Invalid status.")]
        public TaskStatus Status { get; set; }

       
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

}
