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
    public class BatchAuditRepo : IBatchAuditRepository
    {
        private readonly AlisUatContext _db;
        private readonly DefectContext _defectContext;

        public BatchAuditRepo(AlisUatContext db, DefectContext defectContext)
        {
            _db = db;
            _defectContext = defectContext;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<List<GroupedErrorsDTO>> GetErrorGroups()
        {
            var gbaByDateAndErrorType = from gba in _db.GBatchAudit
                where gba.EntryTime >= ReporterArgs.FromDate &&
                      gba.EntryTime <= ReporterArgs.ToDate &&
                      (gba.EntryType == 5 || gba.EntryType == 6)
                group gba by new
                {
                    Message = Regex.Replace(gba.Description, @"[\d-]", string.Empty),
                    Batch = gba.Batch.BatchName,
                    gba.BatchId,
                    Task = gba.Task.TaskName,
                    gba.TaskId
                } into grouping
                select new GroupedErrorsDTO
                {
                    Message = grouping.Key.Message,
                    Batch = grouping.Key.Batch,
                    BatchId = grouping.Key.BatchId,
                    Task = grouping.Key.Task,
                    TaskId = grouping.Key.TaskId,
                    Count = grouping.Count()
                };

            foreach (var groupedErrorsDto in gbaByDateAndErrorType)
            {
                var match = _defectContext.Defects.FirstOrDefault(def =>
                    def.Desc == groupedErrorsDto.Message &&
                    def.BatchId == groupedErrorsDto.BatchId &&
                    def.TaskId == groupedErrorsDto.TaskId);
                groupedErrorsDto.DefectNo = match?.DefectNumber;
            }
            var res = await gbaByDateAndErrorType.ToListAsync();
            return res;
        }

        public async Task<List<TaskListDto>> GetTaskList()
        {

            var gbaResult = from gba in _db.GBatchAudit
                                         where gba.EntryTime >= ReporterArgs.FromDate &&
                                               gba.EntryTime <= ReporterArgs.ToDate &&
                                               gba.EntryType == 1 &&
                                               gba.TaskId != 0
                                         orderby gba.EntryTime
                                         select new
                                         {
                                             gba.EntryTime,
                                             gba.BatchRunNum,
                                             gba.TaskId,
                                             gba.Task.TaskName,
                                             gba.BatchId,
                                             gba.Batch.BatchName
                                         };

            var gbaGroupedByErrorMessage = from gbaGroup in gbaResult
                                           group gbaGroup by new
                                           {
                                               gbaGroup.BatchRunNum,
                                               Task = gbaGroup.TaskName,
                                               TaskId = gbaGroup.TaskId,
                                               Batch = gbaGroup.BatchName,
                                               BatchId = gbaGroup.BatchId,
                                           } into taskBatchGroup

                                           select new TaskListDto
                                           {
                                               BatchName = taskBatchGroup.Key.Batch,
                                               BatchId = taskBatchGroup.Key.BatchId,
                                               TaskName = taskBatchGroup.Key.Task,
                                               TaskId = taskBatchGroup.Key.TaskId,
                                               BatchRunNumber = taskBatchGroup.Key.BatchRunNum,
                                               StartTime = taskBatchGroup.Min(grp => grp.EntryTime)
                                           };

            return await gbaGroupedByErrorMessage.OrderBy(t => t.StartTime).ToListAsync();


        }

        public async Task<List<RunStatsDto>> GetBatchStatistics()
        {
            var q =
                from gba in _db.GBatchAudit
                where gba.EntryTime > ReporterArgs.FromDate &&
                      gba.EntryTime < ReporterArgs.ToDate &&
                      gba.EntryType == 3 &&
                      (gba.Description.Contains("completed , reached") || gba.BatchId == 168)

                group gba by new { gba.Task.TaskName, gba.Batch.BatchName, gba.BatchRunNum }
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
                    where
                        gba.EntryTime > ReporterArgs.FromDate &&
                        gba.EntryTime < ReporterArgs.ToDate &&
                        (gba.EntryType == 6 || gba.EntryType == 5)
                    //orderby new { gba.Description }
                    select new AllErrorsDto
                    {
                        Message = gba.Description,
                        EntityType = gba.PrimaryKeyType,
                        EntityId = gba.PrimaryKey,
                        Batch = gba.Batch.BatchName,
                        BatchId = gba.BatchId,
                        Task = gba.Task.TaskName,
                        TaskId = gba.TaskId,
                        BatchRunNumber = gba.BatchRunNum
                    };
            return await q.OrderBy(o => o.Message).ToListAsync();

        }
    }
}