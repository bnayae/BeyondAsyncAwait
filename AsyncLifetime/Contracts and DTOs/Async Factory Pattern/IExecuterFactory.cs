using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public interface IExecuterFactory
    {
        Task<IExecuter> CreateAsync();
    }
}