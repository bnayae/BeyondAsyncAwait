using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        private const int ITERATIONS = 15;
        static void Main(string[] args)
        {
            Task t = SequentialForkAsync();
            //Task t = NonSequentialForkAsync();

            Console.ReadKey();
        }

        private static async Task SequentialForkAsync()
        {
            var block = new TransformBlock<int, string>(async i =>
            {
                int duration = i % 5 * 1000;
                await Task.Delay(duration).ConfigureAwait(false);
                Console.WriteLine($"\t{i} Transformed after {duration} ms,");
                return $"{i} = {duration} ms";
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = ITERATIONS });

            var ab = new ActionBlock<string>(m => Console.WriteLine(m));
            block.LinkTo(ab, new DataflowLinkOptions { PropagateCompletion = true });
            for (int i = 0; i < ITERATIONS; i++)
            {
                block.Post(i);
            }

            block.Complete();
            await ab.Completion.ConfigureAwait(false);
            Console.WriteLine("Done");
        }

        private static async Task NonSequentialForkAsync()
        {
            var block = CreateMultiTransform<int, string>(async i =>
            {
                int duration = i % 5 * 1000;
                await Task.Delay(duration).ConfigureAwait(false);
                Console.WriteLine($"\t{i} Transformed after {duration} ms,");
                return $"{i} = {duration} ms";
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = ITERATIONS });

            var ab = new ActionBlock<string>(m => Console.WriteLine(m));
            block.LinkTo(ab, new DataflowLinkOptions { PropagateCompletion = true });
            for (int i = 0; i < ITERATIONS; i++)
            {
                block.Post(i);
            }

            block.Complete();
            await ab.Completion.ConfigureAwait(false);
            Console.WriteLine("Done");
        }

        private static IPropagatorBlock<Tin, Tout> CreateMultiTransform<Tin, Tout>(
                                Func<Tin, Task<Tout>> act,
                                ExecutionDataflowBlockOptions options)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var input = new BufferBlock<Tin>();
            var output = new BufferBlock<Tout>();
            int degreeOfParallelism = options.MaxDegreeOfParallelism;
            options.MaxDegreeOfParallelism = 1;
            options.BoundedCapacity = 1; // otherwise the first one will take it all
            var blockes = new Task[degreeOfParallelism];
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                var block = new TransformBlock<Tin, Tout>(act, options);
                blockes[i] = block.Completion;
                input.LinkTo(block, linkOptions);
                block.LinkTo(output);
            }

            Task _ = Task.WhenAll(blockes).ContinueWith(m => output.Complete());
            return DataflowBlock.Encapsulate(input, output);
        }
    }
}
