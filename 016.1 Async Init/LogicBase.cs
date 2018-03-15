using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    public class LogicBase
    {
        protected Config _config;
        protected readonly Task Initialize;


        public LogicBase(ISetting setting)
        {
            Initialize = Init(setting);
        }

        private async Task Init(ISetting setting)
        {
            _config = await setting.GetSetting();
            await OnInitAsync(setting);
        }

        protected virtual Task OnInitAsync(ISetting setting) => Task.CompletedTask;

    }
}
