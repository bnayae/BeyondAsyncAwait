using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Completeble
    {
        public static Completeble<T> Create<T>(T value) => new Completeble<T>(value);
    }

    public class Completeble<T>
    {
        private readonly TaskCompletionSource<T> _taskSemantic;

        #region Ctor

        public Completeble(T value)
        {
            Value = value;
            _taskSemantic = new TaskCompletionSource<T>();
        }

        #endregion // Ctor

        public T Value { get; }

        public Task<T> Task => _taskSemantic.Task;

        public bool TryComplete(T data) => _taskSemantic.TrySetResult(data);
    }
}
