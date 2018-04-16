using System;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task _ = DefaultAsync(10);
            Task _ = FormatAsync(10);
            //Task _ = FormatAsync(10, ErrorFormattingOption.FormatDuplication);
            //Task _ = DefaultMultiAsync();
            //Task _ = FormatMultiAsync();
            Console.ReadKey();
        }

        #region DefaultAsync

        private static async Task DefaultAsync(int i)
        {
            try
            {
                await Step1Async(i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion // DefaultAsync

        #region FormatAsync

        private static async Task FormatAsync(int j, ErrorFormattingOption options = ErrorFormattingOption.Default)
        {
            try
            {
                //await Task.Run(() => throw new ArgumentException("Other Error"));
                await Step1Async(j);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Format(options));
                Console.WriteLine(ex.FormatWithLineNumber());
            }
        }

        #endregion // FormatAsync

        #region FormatNonSequentialAsync

        private static async Task FormatNonSequentialAsync()
        {
            try
            {
                await NonSequentialRootAsync(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format(ErrorFormattingOption.FormatDuplication));
            }
        }

        #endregion // FormatNonSequentialAsync

        #region Step1Async

        private static async Task Step1Async(int i)
        {
            await Task.Delay(1);
            var s = new string('*', i % 6);
            await Step2Async(s);
        }

        #endregion // Step1Async

        #region Step2Async

        private static async Task Step2Async(string s)
        {
            try
            {
                await Task.Delay(1);
                await Step3Async(s);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("in between", ex);
            }
        }

        #endregion // Step2Async

        #region Step3Async

        private static async Task Step3Async(string s1)
        {
            await Task.Delay(1);
            await Step4Async(s1);
        }

        #endregion // Step3Async

        #region Step4Async

        private static async Task Step4Async(string s2)
        {
            await Task.Delay(1);
            throw new FormatException($"Illegal {s2}");
        }

        #endregion // Step4Async

        #region NonSequentialRootAsync

        private static async Task NonSequentialRootAsync(int j)
        {
            await Task.Delay(1);
            await NonSequentialSplitAsync(DateTime.Now.AddDays(j)).ThrowAll();
        }

        #endregion // NonSequentialRootAsync

        #region NonSequentialSplitAsync

        private static Task NonSequentialSplitAsync(DateTime dt)
        {
            var t1 = Task.Run(() => throw new ArgumentException("Other Error"));
            var t2 = Step1Async(dt.Second);
            return Task.WhenAll(t1, t2).ThrowAll();
        }

        #endregion // NonSequentialSplitAsync
    }
}