using System;
using System.Collections.Generic;

namespace ReporterConsole.Models
{
    public partial class TTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime OpeningRegDate { get; set; }
        public int OpeningUserId { get; set; }
        public short InterfaceType { get; set; }
        public string EntryPointName { get; set; }
        public string TaskRunName { get; set; }
        public short CommitFrequency { get; set; }
        public string CommentText { get; set; }
        public int EntityKey { get; set; }
        public int? PostTask { get; set; }
        public int? PreTask { get; set; }
        public int? ChunkParam { get; set; }
        public int? ChunkFlag { get; set; }
        public int? ChunkType { get; set; }
        public int FailureAction { get; set; }
        public int TimeoutSeconds { get; set; }
        public int NumRetries { get; set; }
        public int CounterInc { get; set; }
        public short? MaxItemsBeforeMemoryClaen { get; set; }
        public int Uniqid { get; set; }
        public int Discriminator { get; set; }
        public int ActivityCode { get; set; }
    }
}
