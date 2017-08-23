using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _016._1_Async_Init
{
    public interface ISetting
    {
        Task<Config> GetSetting();
    }
}
