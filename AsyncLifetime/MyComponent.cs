using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class MyComponent
    {
        private Config _config;
        private readonly Task Initilaized;

        public MyComponent(ISetting setting)
        {
            Initilaized = InitAsync(setting);
        }

        private async Task InitAsync(ISetting setting)
        {
            _config = await setting.GetAsync();
        }

        public async Task<int> GetData()
        {
            try
            {
                await Initilaized;
                return 42 * _config.Factor;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
