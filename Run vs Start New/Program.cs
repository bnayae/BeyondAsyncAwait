using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Task _ = StartNewAsync();
            //Task _ = RunAsync();
            //Task _ = StartNewAsRunAsync();
            //Task _ = ParentChildAsync();
            //Task _ = ParentChildSolutionAsync();
            //Task _ = ParentChildRunAsync();
            //Task _ = ParentChildLikeRunAsync();

            Console.ReadKey();
        }

        #region StartNewAsync

        // what is the return value of this call?
        private static async Task StartNewAsync()
        {
            Console.Write("1 ");

            // Should be sequential
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                Console.Write("2 ");
            }); // what is the return value of this call?
            Console.Write("3 ");
        }

        #endregion // StartNewAsync

        #region RunAsync

        // what is the return value of this call?
        private static async Task RunAsync()
        {
            Console.Write("1 ");

            // Should be sequential
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.Write("2 ");
            });
            Console.Write("3 ");
        }

        #endregion // RunAsync

        #region StartNewAsRunAsync

        // what is the return value of this call?
        private static async Task StartNewAsRunAsync()
        {
            Console.Write("1 ");

            // Should be sequential
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                Console.Write("2 ");
            }).Unwrap();
            Console.Write("3 ");
        }

        #endregion // StartNewAsRunAsync

        #region ParentChildAsync

        private static async Task ParentChildAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                        Console.Write("!");
                    });
                    Thread.Sleep(1000);
                    Console.Write(".");
                });
            });
            Console.Write("X");
        }

        #endregion // ParentChildAsync

        #region ParentChildSolutionAsync

        private static async Task ParentChildSolutionAsync()
        {
            var options = TaskCreationOptions.AttachedToParent;

            await Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                        Console.Write("!");
                    }, options);
                    Thread.Sleep(1000);
                    Console.Write(".");
                }, options);
            });
            Console.Write("X");
        }

        #endregion // ParentChildSolutionAsync

        #region ParentChildRunAsync

        private static async Task ParentChildRunAsync()
        {
            var options = TaskCreationOptions.AttachedToParent;

            await Task.Run(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                        Console.Write("!");
                    }, options);
                }, options);
            });
            Console.Write("X");
        }

        #endregion // ParentChildRunAsync

        #region ParentChildLikeRunAsync

        private static async Task ParentChildLikeRunAsync()
        {
            var options = TaskCreationOptions.AttachedToParent;

            await Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                        Console.Write("!");
                    }, options);
                }, options);
            }, TaskCreationOptions.DenyChildAttach);
            Console.Write("X");
        }

        #endregion // ParentChildLikeRunAsync
   }
}