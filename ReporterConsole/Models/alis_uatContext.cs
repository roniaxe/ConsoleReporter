using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration;
using ReporterConsole.Utils;

namespace ReporterConsole.Models
{
    public partial class alis_uatContext : DbContext
    {
        public static IConfigurationRoot Configuration { get; set; }
        public virtual DbSet<GBatchAudit> GBatchAudit { get; set; }
        public virtual DbSet<TBatch> TBatch { get; set; }
        public virtual DbSet<TTask> TTask { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {           
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppConfig.Configuration["ConnectionStrings:Production"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GBatchAudit>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("g_batch_audit");

                entity.HasIndex(e => e.BatchId)
                    .HasName("g_batch_audit_batch_id_idx");

                entity.HasIndex(e => e.BatchRunNum)
                    .HasName("g_batch_audit_run_num_idx");

                entity.HasIndex(e => e.Description)
                    .HasName("g_Batch_audit_desc_idx");

                entity.HasIndex(e => e.EntryTime)
                    .HasName("gba_idx")
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => e.TaskId)
                    .HasName("g_batch_audit_val_id_idx");

                entity.HasIndex(e => new { e.EntryTime, e.EntryType })
                    .HasName("gba_entry_type_time_idx");

                entity.HasIndex(e => new { e.EntryTime, e.TaskId })
                    .HasName("entry_time_task_id_batch_id");

                entity.HasIndex(e => new { e.EntryType, e.BatchRunNum })
                    .HasName("g_batch_audit_idx");

                entity.Property(e => e.Pk).HasColumnName("pk");

                entity.Property(e => e.AssignedTeamNo).HasColumnName("assigned_team_no");

                entity.Property(e => e.AssignedUserId).HasColumnName("assigned_user_id");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.BatchRunNum).HasColumnName("batch_run_num");

                entity.Property(e => e.ChunkId).HasColumnName("chunk_id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Discriminator)
                    .HasColumnName("discriminator")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EntryTime)
                    .HasColumnName("entry_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EntryType).HasColumnName("entry_type");

                entity.Property(e => e.ErrorNo).HasColumnName("error_no");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.PrimaryKey)
                    .HasColumnName("primary_key")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PrimaryKeyType).HasColumnName("primary_key_type");

                entity.Property(e => e.Reference).HasColumnName("reference");

                entity.Property(e => e.SecondaryKey)
                    .HasColumnName("secondary_key")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SecondaryKeyType).HasColumnName("secondary_key_type");

                entity.Property(e => e.ServerName)
                    .HasColumnName("server_name")
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.ThirdKey)
                    .HasColumnName("third_key")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ThirdKeyType).HasColumnName("third_key_type");
            });

            modelBuilder.Entity<TBatch>(entity =>
            {
                entity.HasKey(e => e.Uniqid)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("t_batch");

                entity.HasIndex(e => e.BatchId)
                    .HasName("t_batch_pk")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Uniqid).HasColumnName("uniqid");

                entity.Property(e => e.AutoActivationCode).HasColumnName("auto_activation_code");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.BatchName)
                    .IsRequired()
                    .HasColumnName("batch_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CommitFlag).HasColumnName("commit_flag");

                entity.Property(e => e.DetailsGroup).HasColumnName("details_group");

                entity.Property(e => e.Discriminator)
                    .HasColumnName("discriminator")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EntityKey)
                    .HasColumnName("entity_key")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MaxDbErrors).HasColumnName("max_db_errors");

                entity.Property(e => e.OlActivationCode).HasColumnName("ol_activation_code");

                entity.Property(e => e.OutputFrequency).HasColumnName("output_frequency");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<TTask>(entity =>
            {
                entity.HasKey(e => e.Uniqid)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("t_task");

                entity.HasIndex(e => e.TaskId)
                    .HasName("task_id_idx")
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Uniqid).HasColumnName("uniqid");

                entity.Property(e => e.ActivityCode)
                    .HasColumnName("activity_code")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ChunkFlag).HasColumnName("chunk_flag");

                entity.Property(e => e.ChunkParam).HasColumnName("chunk_param");

                entity.Property(e => e.ChunkType).HasColumnName("chunk_type");

                entity.Property(e => e.CommentText)
                    .HasColumnName("comment_text")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CommitFrequency).HasColumnName("commit_frequency");

                entity.Property(e => e.CounterInc)
                    .HasColumnName("counter_inc")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Discriminator)
                    .HasColumnName("discriminator")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EntityKey)
                    .HasColumnName("entity_key")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EntryPointName)
                    .IsRequired()
                    .HasColumnName("entry_point_name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.FailureAction)
                    .HasColumnName("failure_action")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.InterfaceType).HasColumnName("interface_type");

                entity.Property(e => e.MaxItemsBeforeMemoryClaen)
                    .HasColumnName("max_items_before_memory_claen")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumRetries)
                    .HasColumnName("num_retries")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OpeningRegDate)
                    .HasColumnName("opening_reg_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OpeningUserId).HasColumnName("opening_user_id");

                entity.Property(e => e.PostTask).HasColumnName("post_task");

                entity.Property(e => e.PreTask).HasColumnName("pre_task");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasColumnName("task_name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TaskRunName)
                    .IsRequired()
                    .HasColumnName("task_run_name")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TimeoutSeconds)
                    .HasColumnName("timeout_seconds")
                    .HasDefaultValueSql("((0))");
            });
        }
    }
}
