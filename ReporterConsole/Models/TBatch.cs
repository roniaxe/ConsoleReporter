using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReporterConsole.Models
{
    public partial class TBatch
    {
        [Key]
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public short Status { get; set; }
        public short? OutputFrequency { get; set; }
        public short CommitFlag { get; set; }
        public short? DetailsGroup { get; set; }
        public short? MaxDbErrors { get; set; }
        public short? OlActivationCode { get; set; }
        public short? AutoActivationCode { get; set; }
        public int EntityKey { get; set; }
        public int Uniqid { get; set; }
        public int Discriminator { get; set; }
    }
}
