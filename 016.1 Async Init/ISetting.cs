using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public interface ISetting
    {
        Task<Config> GetSetting();
    }
}
