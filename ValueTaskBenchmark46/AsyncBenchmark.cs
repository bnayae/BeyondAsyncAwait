using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    [ClrJob]
    //[CoreJob]
    //[MonoJob]
    [RankColumn, MinColumn, MaxColumn]
    //[SimpleJob(RunStrategy.Throughput, launchCount: 10, warmupCount: 0, targetCount: 100)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class AsyncBenchmark
    {
        //private const int OPERATIONS_PER_INVOKE = 1000;

        [BenchmarkCategory("FromResult")]
        [Benchmark(Description = "FromResult")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public Task<int> TaskFromResult() => Task.FromResult(1);

        [BenchmarkCategory("FromResult")]
        [Benchmark(Baseline = true, Description = "new ValueTask")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public ValueTask<int> ValueTaskFromResult() => new ValueTask<int>(1);

        [BenchmarkCategory("Async+FromResult")]
        [Benchmark(Description = "await Task")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async Task<int> TaskAsyncFromResult()
        {
            await Task.FromResult(1);
            await Task.Run(() => { });
            await Task.FromResult(1);
            return 1;
        }

        [BenchmarkCategory("Async+FromResult")]
        [Benchmark(Description = "await ValueTask")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async ValueTask<int> ValueTaskAsyncFromResult()
        {
            await new ValueTask<int>(1);
            await Task.Run(() => { });
            await new ValueTask<int>(1);
            return 1;
        }


        [BenchmarkCategory("Async+FromResult+ConfigureAwait")]
        [Benchmark(Description = "await Task.Configure")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async Task<int> TaskAsyncFromResultConfigureAwait()
        {
            await Task.FromResult(1).ConfigureAwait(false);
            await Task.Run(() => { }).ConfigureAwait(false);
            await Task.FromResult(1).ConfigureAwait(false);
            return 1;
        }

        [BenchmarkCategory("Async+FromResult+ConfigureAwait")]
        [Benchmark(Description = "await ValueTask.Configure")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async ValueTask<int> ValueTaskAsyncFromResultConfigureAwait()
        {
            await new ValueTask<int>(1).ConfigureAwait(false);
            await Task.Run(() => { }).ConfigureAwait(false);
            await new ValueTask<int>(1).ConfigureAwait(false);
            return 1;
        }
    }
}