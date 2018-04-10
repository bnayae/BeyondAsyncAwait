using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            #region ISetting setting = ... (mock)

            ISetting setting = A.Fake<ISetting>(); // MOCK
            A.CallTo(() => setting.GetAsync()) // the fake will return the config with 1 second delay
                .Returns(Task.Run(async () =>
                            {
                                await Task.Delay(10_000).ConfigureAwait(false);
                                return new Config();
                            }));

            #endregion // ISetting setting = ... (mock)            

            var logic = new Logic(setting);
            int a = await logic.GetData(); // asynchronous
            int b = await logic.GetData(); // synchronous
            Console.ReadKey();
        }

    }
}
