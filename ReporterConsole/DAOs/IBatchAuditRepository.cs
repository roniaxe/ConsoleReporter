using System.Collections.Generic;
using System.Threading.Tasks;
using ReporterConsole.DTOs;

namespace ReporterConsole.DAOs
{
    public interface IBatchAuditRepository : IRepository
    {
        Task<List<AllErrorsDto>> GetAllErrors();

        Task<List<RunStatsDto>> GetBatchStatistics();

        Task<List<TaskListDto>> GetTaskList();

        Task<List<GroupedErrorsDTO>> GetErrorGroups();
    }
}
