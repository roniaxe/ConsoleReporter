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
        }

        public async Task<List<GroupedErrorsDTO>> GetErrorGroups()
        {
            var gbaByDateAndErrorType = from gba in _db.GBatchAudit
                                        where gba.EntryTime >= ReporterArgs.FromDate &&
                                              gba.EntryTime <= ReporterArgs.ToDate &&
                                              (gba.EntryType == 5 || gba.EntryType == 6)
                                        select gba;

            var gbaJoinTasksAndBatches = from gbajoin in gbaByDateAndErrorType
                                         join task1 in _db.TTask on gbajoin.TaskId equals task1.TaskId
                                         join batch1 in _db.TBatch on gbajoin.BatchId equals batch1.BatchId
                                         select new
                                         {
                                             Desc = Regex.Replace(gbajoin.Description, @"[\d-]", string.Empty),
                                             task1.TaskId,
                                             task1.TaskName,
                                             batch1.BatchId,
                                             batch1.BatchName
                                         };

            var gbaGroupedByErrorMessage = from errs in gbaJoinTasksAndBatches
                                           group errs by new
                                           {
                                               Message = errs.Desc,
                                               Task = errs.TaskName,
                                               TaskId = errs.TaskId,
                                               Batch = errs.BatchName,
                                               BatchId = errs.BatchId,
                                               Count = errs
                                           } into errGroup
                                           select new GroupedErrorsDTO
                                           {
                                               Message = errGroup.Key.Message,
                                               Batch = errGroup.Key.Batch,
                                               BatchId = errGroup.Key.BatchId,
                                               Task = errGroup.Key.Task,
                                               TaskId = errGroup.Key.TaskId,
                                               Count = errGroup.Count()
                                           };

            var res = await gbaGroupedByErrorMessage.ToListAsync();
            foreach (var groupedErrorsDto in res)
            {
                groupedErrorsDto.DefectNo = _defectContext.Defects.FirstOrDefault(def =>
                    def.Desc == groupedErrorsDto.Message &&
                    def.BatchId == groupedErrorsDto.BatchId &&
                    def.TaskId == groupedErrorsDto.TaskId)?.DefectNumber;
            }

            return res;
        }

        public async Task<List<TaskListDto>> GetTaskList()
        {

            var gbaJoinTasksAndBatches = from gbajoin in _db.GBatchAudit
                                         join task1 in _db.TTask on gbajoin.TaskId equals task1.TaskId
                                         join batch1 in _db.TBatch on gbajoin.BatchId equals batch1.BatchId
                                         where gbajoin.EntryTime >= ReporterArgs.FromDate &&
                                               gbajoin.EntryTime <= ReporterArgs.ToDate &&
                                               gbajoin.EntryType == 1
                                         select new
                                         {
                                             gbajoin.EntryTime,
                                             gbajoin.BatchRunNum,
                                             task1.TaskId,
                                             task1.TaskName,
                                             batch1.BatchId,
                                             batch1.BatchName
                                         };

            var gbaGroupedByErrorMessage = from gbaJoin in gbaJoinTasksAndBatches
                                           group gbaJoin by new
                                           {
                                               gbaJoin.BatchRunNum,
                                               Task = gbaJoin.TaskName,
                                               TaskId = gbaJoin.TaskId,
                                               Batch = gbaJoin.BatchName,
                                               BatchId = gbaJoin.BatchId,
                                           } into taskBatchGroup
                                           select new TaskListDto
                                           {
                                               BatchName = taskBatchGroup.Key.Batch,
                                               BatchId = taskBatchGroup.Key.BatchId,
                                               TaskName = taskBatchGroup.Key.Task,
                                               TaskId = taskBatchGroup.Key.TaskId,
                                               BatchRunNumber = taskBatchGroup.Key.BatchRunNum,
                                               StartTime = taskBatchGroup.Min(p => p.EntryTime)
                                           };

            return await gbaGroupedByErrorMessage.OrderBy(t => t.StartTime).ToListAsync();


        }

        public async Task<List<RunStatsDto>> GetBatchStatistics()
        {
            var q =
                from gba in _db.GBatchAudit
                join tTask in _db.TTask on gba.TaskId equals tTask.TaskId
                join tBatch in _db.TBatch on gba.BatchId equals tBatch.BatchId
                where gba.EntryTime > ReporterArgs.FromDate &&
                      gba.EntryTime < ReporterArgs.ToDate &&
                      gba.EntryType == 3 &&
                      (gba.Description.Contains("completed , reached") || gba.BatchId == 168)

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