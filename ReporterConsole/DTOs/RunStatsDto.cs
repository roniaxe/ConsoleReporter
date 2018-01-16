using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.DTOs
{
    public class RunStatsDto
    {
        public int? BatchRunNum { get; set; }
        public string BatchName { get; set; }
        public string TaskName { get; set; }
        public string Chunkked { get; set; }
        public int Proccessed { get; set; }
    }
}
