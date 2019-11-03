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
            //Task t = SequentialForkAsync();
            Task t = NonSequentialForkAsync();

            Console.ReadKey();
        }

        #region SequentialForkAsync

        private static async Task SequentialForkAsync()
        {
            // the output of transform block is sequential 
            // (dictate by the order of the original input)
            var block = new TransformBlock<int, string>(i => UnitOfWorkAsync(i), 
                                            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = ITERATIONS });

            // the action block will get the transformation in the original order
            var ab = new ActionBlock<string>(m => Console.WriteLine($"# Action: {m}"));
            block.LinkTo(ab, new DataflowLinkOptions { PropagateCompletion = true });

            for (int i = 0; i < ITERATIONS; i++)
            {
                block.Post(i);
            }

            block.Complete();
            await ab.Completion.ConfigureAwait(false);
            Console.WriteLine("Done");
        }

        #endregion // SequentialForkAsync

        #region NonSequentialForkAsync

        private static async Task NonSequentialForkAsync()
        {
            var block = CreateMultiTransform<int, string>(UnitOfWorkAsync,
                            new ExecutionDataflowBlockOptions
                                    { MaxDegreeOfParallelism = ITERATIONS });

            var ab = new ActionBlock<string>(m => Console.WriteLine($"# Action: {m}"));
            block.LinkTo(ab, new DataflowLinkOptions { PropagateCompletion = true });
            for (int i = 0; i < ITERATIONS; i++)
            {
                block.Post(i);
            }

            block.Complete();
            await ab.Completion.ConfigureAwait(false);
            Console.WriteLine("Done");
        }

        #endregion // NonSequentialForkAsync

        #region UnitOfWork

        private static async Task<string> UnitOfWorkAsync(int i)
        {
            int duration = ((i % 5) + 1) * 1000; // transformation duration
            await Task.Delay(duration).ConfigureAwait(false);
            Console.WriteLine($"\t\t# Transform: {i} Transformed after {duration} ms,");
            return $"{i} = {duration} ms";
        }

        #endregion // UnitOfWork

        #region CreateMultiTransform

        /// <summary>
        /// Create non-sequential transform behavior by composition of blocks.
        /// </summary>
        /// <typeparam name="Tin">The type of the in.</typeparam>
        /// <typeparam name="Tout">The type of the out.</typeparam>
        /// <param name="act">The act.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private static IPropagatorBlock<Tin, Tout> CreateMultiTransform<Tin, Tout>(
                                Func<Tin, Task<Tout>> act,
                                ExecutionDataflowBlockOptions options)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            var input = new BufferBlock<Tin>();     // get the input
            var output = new BufferBlock<Tout>();   // keep the output

            int degreeOfParallelism = options.MaxDegreeOfParallelism;
            options.MaxDegreeOfParallelism = 1; // reset to single task
            options.BoundedCapacity = 1; // otherwise the first one will take it all

            // instead of single block with multi-task
            // having multi-blocks with single task
            // each block map the input to the output
            var blocks = new Task[degreeOfParallelism]; // completion of all blocks
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                var block = new TransformBlock<Tin, Tout>(act, options);
                blocks[i] = block.Completion;
                input.LinkTo(block, linkOptions);
                block.LinkTo(output);
            }

            Task _ = Task.WhenAll(blocks).ContinueWith(m => output.Complete());
            return DataflowBlock.Encapsulate(input, output); // wrap the functionality
        }

        #endregion // CreateMultiTransform
    }
}
