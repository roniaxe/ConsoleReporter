using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ReporterConsole.DTOs;

namespace ReporterConsole.DAOs
{
    public interface IRepository
    {
        Task<List<AllErrorsDto>> GetAllErrors();

        Task<List<RunStatsDto>> GetBatchStatistics();

        Task<List<TaskListDto>> GetTaskList();

        Task<IList<GroupedErrorsDTO>> GetErrorGroups();
    }
}
