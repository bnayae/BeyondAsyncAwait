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
    [ClrJob] //, RyuJitX64Job]//, LegacyJitX64Job]
    //[CoreJob]
    //[MonoJob]
    [MemoryDiagnoser]
    [RankColumn, MeanColumn, CategoriesColumn] //, MinColumn, MaxColumn]
    //[SimpleJob(RunStrategy.Throughput, launchCount: 5, warmupCount: 5, targetCount: 40, id: "Throughput")]
    //[SimpleJob(RunStrategy.Monitoring, launchCount: 5, warmupCount: 5, targetCount: 40, id: "Monitoring")]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class ValueOrNotBenchmark
    {
        //private const int OPERATIONS_PER_INVOKE = 1000;

        [BenchmarkCategory("FromResult")]
        [Benchmark(Description = "FromResult")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public Task<int> TaskFromResult() => Task.FromResult(1);

        [BenchmarkCategory("FromResult")]
        [Benchmark(Baseline = true, Description = "new ValueTask")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public ValueTask<int> ValueTaskFromResult() => new ValueTask<int>(1);

        [BenchmarkCategory("Awaitable-Sync")]
        [Benchmark(Description = "await Task")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async Task<int> SyncAwaitableTask()
        {
            await Task.CompletedTask;
            return 10;
        }

        [BenchmarkCategory("Awaitable-Sync")]
        [Benchmark(Description = "await ValueTask")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async ValueTask<int> SyncAwaitableValueTask()
        {
            await Task.CompletedTask;
            return 10;
        }

        [BenchmarkCategory("Awaitable-Async")]
        [Benchmark(Description = "await Task")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async Task<int> AsyncAwaitableTask()
        {
            await Task.Delay(1);
            return 10;
        }

        [BenchmarkCategory("Awaitable-Async")]
        [Benchmark(Description = "await ValueTask")] //, OperationsPerInvoke = OPERATIONS_PER_INVOKE)]
        public async ValueTask<int> AsyncAwaitableValueTask()
        {
            await Task.Delay(1);
            return 10;
        }
    }
}