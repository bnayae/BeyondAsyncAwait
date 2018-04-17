using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public interface ISetting
    {
        Task<Config> GetAsync();
    }
}