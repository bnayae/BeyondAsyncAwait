using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    /// <summary>
    /// Flows:
    /// 
    ///     SeqAsync -> _SeqAsync -> __LastAsync
    ///     
    ///                     ---> _ForkA_Async --
    ///     ForkJoinAsync -|                    |--> __LastAsync
    ///                     ---> _ForkB_Async --
    ///                     
    /// </summary>
    /// <remarks>
    /// When not using immutable one fork can see other fork data.
    /// Available on .NET 4.6 or .NET Core
    /// For older version use CallContext.LogicalGetData / CallContext.LogicalSetData
    /// </remarks>
    public class Program
    {
        //private static readonly IAsyncContext _context = AsyncImmutableContext.Instance;
        private static readonly IAsyncContext _context = AsyncContext.Instance;

        //private static Func<int, Task> _executer = SeqAsync;
        private static Func<int, Task> _executer = ForkJoinAsync;

        public static void Main()
        {
            _executer(10);
            //var tasks = from i in Enumerable.Range(0, 3)
            //            select _executer(i);

            //Task.WaitAll(tasks.ToArray()); // Don't get use to use Wait
            //Console.WriteLine("Done");
            Console.ReadKey();
        }

        #region SeqAsync

        static async Task SeqAsync(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            await _SeqAsync(i);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // SeqAsync

        #region ForkJoinAsync


        static async Task ForkJoinAsync(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            Task a = _Fork_A_Async(i);
            Task b = _Fork_B_Async(i);
            await Task.WhenAll(a, b);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // ForkJoinAsync

        #region _ForkA_Async

        static async Task _Fork_A_Async(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            await __LastAsync(i);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // _ForkA_Async

        #region _ForkB_Async

        static async Task _Fork_B_Async(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            await __LastAsync(i);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // _ForkB_Async

        #region _SeqAsync

        static async Task _SeqAsync(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            await __LastAsync(i);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // _SeqAsync

        #region __LastAsync

        static async Task __LastAsync(int i)
        {
            _context.Add($"Start {i}");
            await Task.Delay(100);
            _context.Add($"End {i}");
            Console.WriteLine(_context.GetContext(i));
        }

        #endregion // __LastAsync
    }
}
