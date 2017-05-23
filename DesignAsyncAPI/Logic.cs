using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Logic
    {
        #region OldFashion

        public void OldFashion(
            Func<string> strategy, string title)
        {
            Console.WriteLine(title);
            byte[] buffer = Encoding.UTF8.GetBytes("API Design");
            string content = strategy();
            Console.WriteLine(content);
        }

        #endregion // OldFashion

        #region TapFashion

        public async Task TapFashion(
            Func<Task<string>> strategy, string title)
        {
            Console.WriteLine(title);
            byte[] buffer = Encoding.UTF8.GetBytes("API Design");
            string content = await strategy();
            Console.WriteLine(content);
        }

        #endregion // TapFashion

        #region VTapFashion

        public async ValueTask<object> VTapFashion(
            Func<ValueTask<string>> strategy, string title)
        {
            Console.WriteLine(title);
            byte[] buffer = Encoding.UTF8.GetBytes("API Design");
            string content = await strategy();
            Console.WriteLine(content);
            return null;
        }

        #endregion // VTapFashion

        #region LTapFashion

        public async LightTask<object> LTapFashion(
            Func<LightTask<string>> strategy, string title)
        {
            Console.WriteLine(title);
            byte[] buffer = Encoding.UTF8.GetBytes("API Design");
            string content = await strategy();
            Console.WriteLine(content);
            return null;
        }

        #endregion // LTapFashion
    }
}
