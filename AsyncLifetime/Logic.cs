using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Logic
    {
        private Config _config;
        private readonly Task Initilaized;

        public Logic(ISetting setting)
        {
            Initilaized = InitAsync(setting)
                                .WithTimeout();
        }

        private async Task InitAsync(ISetting setting)
        {
            _config = await setting.GetAsync().ConfigureAwait(false);
        }

        public async Task<int> GetData()
        {
            try
            {
                await Initilaized.ConfigureAwait(false);
                return 42 * _config.Factor;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
