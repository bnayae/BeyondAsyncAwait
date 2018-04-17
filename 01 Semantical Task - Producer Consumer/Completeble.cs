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

        private Completeble(T value, TaskCompletionSource<T> taskSemantic)
        {
            Value = value;
            _taskSemantic = taskSemantic;
        }

        #endregion // Ctor

        public T Value { get; }

        public Task<T> Task => _taskSemantic.Task;

        public bool TryComplete() => _taskSemantic.TrySetResult(Value);

        public Completeble<T> ProceedWith(T nextValue) => // change the value keep the task semantic ref
                        new Completeble<T>(nextValue, _taskSemantic);
    }
}
