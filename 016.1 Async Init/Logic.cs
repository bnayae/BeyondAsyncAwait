using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Logic: LogicBase
    {
        private string _name;

        public Logic(ISetting setting): base(setting)
        {
        }

        protected override Task OnInitAsync(ISetting setting)
        {
            _name = nameof(Bnaya);
            Trace.Write(_name.GetType().ToString());
            return base.OnInitAsync(setting);
        }

        public async Task<int> ExecAsync()
        {
            await Initialize.ConfigureAwait(false);
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            return 1 * _config.Factor;
        }
    }
}
