using System;
using System.Collections.Generic;

namespace ReporterConsole.Models
{
    public partial class GBatchAudit
    {
        public int Pk { get; set; }
        public short EntryType { get; set; }
        public DateTime EntryTime { get; set; }
        public short? PrimaryKeyType { get; set; }
        public string PrimaryKey { get; set; }
        public short? SecondaryKeyType { get; set; }
        public string SecondaryKey { get; set; }
        public short? ThirdKeyType { get; set; }
        public string ThirdKey { get; set; }
        public int? Reference { get; set; }
        public int? BatchRunNum { get; set; }
        public int? BatchId { get; set; }
        public TBatch Batch { get; set; }
        public int? TaskId { get; set; }
        public TTask Task { get; set; }
        public int? ErrorNo { get; set; }
        public string Description { get; set; }
        public int? AssignedTeamNo { get; set; }
        public int? AssignedUserId { get; set; }
        public int? ChunkId { get; set; }
        public string ServerName { get; set; }
        public int? Pid { get; set; }
        public int Discriminator { get; set; }
    }
}
