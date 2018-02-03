using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
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

            List<Task> tasks = new List<Task>{errors,taskList,runStats,allErrors};
            while (tasks.Any())
            {
                var completedTask = await Task.WhenAny(tasks);
                DataTable dataTable = null;
                if (completedTask == errors)
                {
                    dataTable = EnumerableToDataTable.ToDataTable(await errors);
                    dataTable.TableName = "Error Groups";
                }

                if (completedTask == taskList)
                {
                    dataTable = EnumerableToDataTable.ToDataTable(await taskList);
                    dataTable.TableName = "Batch Run List";
                }

                if (completedTask == runStats)
                {
                    dataTable = EnumerableToDataTable.ToDataTable(await runStats);
                    dataTable.TableName = "Run Statistics";
                }

                if (completedTask == allErrors)
                {
                    dataTable = EnumerableToDataTable.ToDataTable(await allErrors);
                    dataTable.TableName = "All Errors";
                }
                result.Add(dataTable);
                tasks.Remove(completedTask);
            }

            //var groupedErrorsDataTable = EnumerableToDataTable.ToDataTable(await errors);
            //groupedErrorsDataTable.TableName = "Error Groups";
            //result.Add(groupedErrorsDataTable);

            //var taskListDataTable = EnumerableToDataTable.ToDataTable(await taskList);
            //taskListDataTable.TableName = "Batch Run List";
            //result.Add(taskListDataTable);

            //var runStatsDataTable = EnumerableToDataTable.ToDataTable(await runStats);
            //taskListDataTable.TableName = "Run Statistics";
            //result.Add(runStatsDataTable);

            //var allErrorsDataTable = EnumerableToDataTable.ToDataTable(await allErrors);
            //taskListDataTable.TableName = "All Errors";
            //result.Add(allErrorsDataTable);

            return result;
        }
    }
}
