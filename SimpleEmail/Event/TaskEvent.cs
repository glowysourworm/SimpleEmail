using SimpleWpf.IocFramework.EventAggregation;

namespace SimpleEmail.Event
{
    public enum TaskEventType
    {
        Started = 0,
        Completed = 1
    }
    public enum TaskType
    {
        Initialization = 0,
        PrimaryTask = 1,
        BackgroundTask = 2
    }

    public class TaskEventData
    {
        /// <summary>
        /// Type of task:  Initialization, Primary (blocking), or Background
        /// </summary>
        public TaskType Type { get; set; }

        /// <summary>
        /// Signals that the task has either started or completed
        /// </summary>
        public TaskEventType Status { get; set; }
    }
    public class TaskEvent : IocEvent<TaskEventData>
    {

    }
}
