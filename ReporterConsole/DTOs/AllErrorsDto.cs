using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.DTOs
{
    public class AllErrorsDto
    {
        public string Message { get; set; }
        public short? EntityType { get; set; }
        public string EntityId { get; set; }
        public string Batch { get; set; }
        public int BatchId { get; set; }
        public string Task { get; set; }
        public int TaskId { get; set; }
        public int? BatchRunNumber { get; set; }
    }
}
