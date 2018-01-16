using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ReporterConsole.DAOs;
using ReporterConsole.DTOs;

namespace ReporterConsole.Utils
{
    public class QueryManager
    {
        public static async Task<List<DataTable>> GetQueriesResultList(FromToDateDto dateDto)
        {
            var result = new List<DataTable>();
            //Get Grouped Errors
            var errors = BatchAuditRepo.GetErrorGroups(dateDto);
            //Get Task List
            var taskList = BatchAuditRepo.GetTaskList(dateDto);
            //Get Run Statistics
            var runStats = BatchAuditRepo.GetBatchStatistics(dateDto);
            //Get All Errors
            var allErrors = BatchAuditRepo.GetAllErrors(dateDto);


            var groupedErrorsDataTable = EnumerableToDataTable.ToDataTable(await errors);
            groupedErrorsDataTable.TableName = "Error Groups";
            result.Add(groupedErrorsDataTable);

            var taskListDataTable = EnumerableToDataTable.ToDataTable(await taskList);
            taskListDataTable.TableName = "Batch Run List";
            result.Add(taskListDataTable);

            var runStatsDataTable = EnumerableToDataTable.ToDataTable(await runStats);
            taskListDataTable.TableName = "Run Statistics";
            result.Add(runStatsDataTable);

            var allErrorsDataTable = EnumerableToDataTable.ToDataTable(await allErrors);
            taskListDataTable.TableName = "All Errors";
            result.Add(allErrorsDataTable);

            return result;
        }
    }
}
