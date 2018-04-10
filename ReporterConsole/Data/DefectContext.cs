using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ReporterConsole.Data
{
    public class DefectContext : DbContext
    {
        public DefectContext(DbContextOptions<DefectContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Defect>()
                .HasKey(k => new {k.Desc, k.BatchId, k.TaskId});
        }

        public DbSet<Defect> Defects { get; set; }
    }

    public class Defect
    {
        [Key]
        public string Desc { get; set; }
        [Key]
        public int BatchId { get; set; }
        [Key]
        public int TaskId { get; set; }
        public string DefectNumber { get; set; }
    }
}
