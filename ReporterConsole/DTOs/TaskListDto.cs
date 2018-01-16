using System;

namespace ReporterConsole.DTOs
{
    public class TaskListDto
    {
        public string BatchName { get; set; }
        public int BatchId { get; set; }
        public string TaskName { get; set; }
        public int TaskId { get; set; }
        public int? BatchRunNumber { get; set; }
        public DateTime StartTime { get; set; }
    }
}
