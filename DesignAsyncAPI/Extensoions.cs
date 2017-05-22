using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks
{
    public static class Extensoions
    {
        public static ValueTask<T> ToValueTask<T>(this T value) =>
            new ValueTask<T>(value);
    }
}
