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

        public T Value { get; }

        public Task<T> Task => _taskSemantic.Task;

        public bool TryComplete() => _taskSemantic.TrySetResult(Value);
        public bool TryCompleteAsFaulted(Exception error) => _taskSemantic.TrySetException(error);

        public Completeble<T> ProceedWith(T nextValue) =>
                        new Completeble<T>(nextValue, _taskSemantic);
    }
}
