using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReporterConsole.Data;
using ReporterConsole.DTOs;
using static ReporterConsole.Program;

namespace ReporterConsole.DAOs
{
    public class BatchAuditRepo : IRepository
    {
        private readonly AlisUatContext _db;

        public BatchAuditRepo(AlisUatContext db)
        {
            _db = db;
        }

        public async Task<IList<GroupedErrorsDTO>> GetErrorGroups()
        {

            var query = await
            (from gba in _db.GBatchAudit
             where gba.EntryTime >= ReporterArgs.FromDate &&
                   gba.EntryTime <= ReporterArgs.ToDate &&
                   (gba.EntryType == 6 || gba.EntryType == 5)
             join task in _db.TTask on gba.TaskId equals task.TaskId
             join batch in _db.TBatch on gba.BatchId equals batch.BatchId
             select new
             {
                 BatchAudit = gba,
                 Task = task,
                 Batch = batch
             }).ToListAsync();

            query.ForEach(q =>
                q.BatchAudit.Description = Regex.Replace(q.BatchAudit.Description, @"[\d-]", string.Empty));

            var groupMessageQuery =
                from errorList in query
                group errorList by new
                {
                    Message = errorList.BatchAudit.Description,
                    Task = errorList.Task.TaskName,
                    errorList.Task.TaskId,
                    Batch = errorList.Batch.BatchName,
                    errorList.Batch.BatchId
                }
                into errGroup
                select new GroupedErrorsDTO
                {
                    Message = errGroup.Key.Message,
                    Batch = errGroup.Key.Batch,
                    BatchId = errGroup.Key.BatchId,
                    Task = errGroup.Key.Task,
                    TaskId = errGroup.Key.TaskId,
                    Count = errGroup.Count()
                };

            return groupMessageQuery.ToList();
        }

        public async Task<List<TaskListDto>> GetTaskList()
        {

                var q =
                from gba in _db.GBatchAudit
                join tTask in _db.TTask on gba.TaskId equals tTask.TaskId
                join tBatch in _db.TBatch on gba.BatchId equals tBatch.BatchId
                where gba.EntryTime > ReporterArgs.FromDate && gba.EntryTime < ReporterArgs.ToDate
                group new { gba, tTask, tBatch } by new
                {
                    gba.BatchRunNum,
                    tTask.TaskId,
                    tBatch.BatchId,
                    tTask.TaskName,
                    tBatch.BatchName
                }
                into g
                select new TaskListDto
                {
                    BatchName = g.Key.BatchName,
                    BatchId = g.Key.BatchId,
                    TaskName = g.Key.TaskName,
                    TaskId = g.Key.TaskId,
                    BatchRunNumber = g.Key.BatchRunNum,
                    StartTime = g.Min(p => p.gba.EntryTime) 
                };

            return await q.ToListAsync();


        }

        public async Task<List<RunStatsDto>> GetBatchStatistics()
        {

            var q = from gba in _db.GBatchAudit
                    where gba.EntryTime > ReporterArgs.FromDate &&
                          gba.EntryTime < ReporterArgs.ToDate &&
                          (gba.Description.Contains("completed , reached") || gba.BatchId == 168 && gba.EntryType == 3)
                    join tTask in _db.TTask on gba.TaskId equals tTask.TaskId
                    join tBatch in _db.TBatch on gba.BatchId equals tBatch.BatchId
                    group gba by new { tTask.TaskName, tBatch.BatchName, gba.BatchRunNum }
                into grouped
                    select new RunStatsDto
                    {
                        BatchRunNum = grouped.Key.BatchRunNum,
                        BatchName = grouped.Key.BatchName,
                        TaskName = grouped.Key.TaskName,
                        Chunkked = grouped.Sum(g => g.ChunkId) > 1 ? "Yes" : "No",
                        Proccessed = grouped.Count()
                    };
            return await q.ToListAsync();

        }

        public async Task<List<AllErrorsDto>> GetAllErrors()
        {

            var q = from gba in _db.GBatchAudit
                    join tt in _db.TTask on gba.TaskId equals tt.TaskId
                    join tb in _db.TBatch on gba.BatchId equals tb.BatchId
                    where
                        gba.EntryTime > ReporterArgs.FromDate &&
                        gba.EntryTime < ReporterArgs.ToDate &&
                        (gba.EntryType == 6 || gba.EntryType == 5)
                    orderby new { gba.Description }
                    select new AllErrorsDto
                    {
                        Message = gba.Description,
                        EntityType = gba.PrimaryKeyType,
                        EntityId = gba.PrimaryKey,
                        Batch = tb.BatchName,
                        BatchId = tb.BatchId,
                        Task = tt.TaskName,
                        TaskId = tt.TaskId,
                        BatchRunNumber = gba.BatchRunNum
                    };
            return await q.ToListAsync();

        }
    }
}