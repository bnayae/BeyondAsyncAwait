using System;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task _ = DefaultAsync();
            //Task _ = FormatAsync();
            //Task _ = DefaultMultiAsync();
            Task _ = FormatMultiAsync();
            Console.ReadKey();
        }

        #region DefaultAsync

        private static async Task DefaultAsync()
        {
            try
            {
                await Step1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion // DefaultAsync

        #region FormatAsync

        private static async Task FormatAsync()
        {
            try
            {
                await Step1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }
        }

        #endregion // FormatAsync

        #region DefaultMultiAsync

        private static async Task DefaultMultiAsync()
        {
            try
            {
                await StepAAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion // DefaultMultiAsync

        #region FormatMultiAsync

        private static async Task FormatMultiAsync()
        {
            try
            {
                await StepAAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }
        }

        #endregion // FormatMultiAsync

        #region Step1Async

        private static async Task Step1Async()
        {
            await Task.Delay(1);
            await Step2Async();
        }

        #endregion // Step1Async

        #region Step2Async

        private static async Task Step2Async()
        {
            try
            {
                await Task.Delay(1);
                await Step3Async();
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("in between", ex);
            }
        }

        #endregion // Step2Async

        #region Step3Async

        private static async Task Step3Async()
        {
            await Task.Delay(1);
            await Step4Async();
        }

        #endregion // Step3Async

        #region Step4Async

        private static async Task Step4Async()
        {
            await Task.Delay(1);
            throw new FormatException("Illegal");
        }
 
        #endregion // Step4Async

        #region StepAAsync

        private static async Task StepAAsync()
        {
            await Task.Delay(1);
            await StepBAsync();
        }

        #endregion // StepAAsync

        #region StepBAsync

        private static async Task StepBAsync()
        {
            var t1 = Task.Run(() => throw new ArgumentException("Other Error"));
            var t2 = Step1Async();
            await Task.WhenAll(t1, t2).ThrowAll();
        }

        #endregion // StepBAsync
   }
}