using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class ExecuterFactory : IExecuterFactory
    {
        private ISetting _setting;

        public ExecuterFactory(ISetting setting)
        {
            _setting = setting;
        }

        public async Task<IExecuter> CreateAsync()
        {
            Config config = await _setting.GetAsync().ConfigureAwait(false);
            return new Executer(config);
        }
    }
}
