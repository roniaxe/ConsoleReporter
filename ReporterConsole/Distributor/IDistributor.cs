using System.Threading.Tasks;

namespace ReporterConsole.Distributor
{
	public interface IDistributor
	{
	    string Attachment { get; set; }
        void Execute();
    }
}
