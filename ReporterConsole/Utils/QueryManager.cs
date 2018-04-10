using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReporterConsole.DAOs;

namespace ReporterConsole.Utils
{
    public class QueryManager
    {
        public static async Task<List<DataTable>> GetQueriesResultList()
        {
            var result = new List<DataTable>();

            var repo = Program.SProvider.GetService<IBatchAuditRepository>();

            //Get Grouped Errors
            var errors = repo.GetErrorGroups();
            //Get Task List
            var taskList = repo.GetTaskList();
            //Get Run Statistics
            var runStats = repo.GetBatchStatistics();
            //Get All Errors
            var allErrors = repo.GetAllErrors();

			List<Task> tasks = new List<Task> { errors, taskList, runStats, allErrors };
			while (tasks.Any())
			{
                try
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
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
			}

			return result;
        }
    }
}
