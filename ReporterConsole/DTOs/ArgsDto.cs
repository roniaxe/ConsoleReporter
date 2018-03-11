using System;


namespace ReporterConsole.DTOs
{
    public class ArgsDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Environment { get; set; } = "Production";
        public string DbName { get; set; }
        public string TextToEncrypt { get; set; }
    }
}
