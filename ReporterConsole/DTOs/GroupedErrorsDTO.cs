using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.DTOs
{
    public class GroupedErrorsDTO
    {
        public string Message { get; set; }
        public string Batch { get; set; }
        public int BatchId { get; set; }
        public string Task { get; set; }
        public int TaskId { get; set; }
        public int Count { get; set; }
        public string DefectNo { get; set; }
    }
}
