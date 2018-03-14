using System.Threading.Tasks;

namespace ReporterConsole.Distributor
{
	public interface IDistributor
    {
        Task ExecuteAsync();
    }
}
