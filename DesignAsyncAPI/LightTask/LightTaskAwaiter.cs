// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaiter for a <see cref="LightTask{TResult}"/>.</summary>
    public struct LightTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        /// <summary>The value being awaited.</summary>
        private readonly LightTask<TResult> _value;

        /// <summary>Initializes the awaiter.</summary>
        /// <param name="value">The value to be awaited.</param>
        internal LightTaskAwaiter(LightTask<TResult> value) { _value = value; }

        /// <summary>Gets whether the <see cref="LightTask{TResult}"/> has completed.</summary>
        public bool IsCompleted { get { return _value.IsCompleted; } }

        /// <summary>Gets the result of the LightTask.</summary>
        public TResult GetResult()
        {
            return _value._task == null ?
                _value._result :
                _value._task.GetAwaiter().GetResult();
        }

        /// <summary>Schedules the continuation action for this LightTask.</summary>
        public void OnCompleted(Action continuation)
        {
            _value.AsTaskAsync().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().OnCompleted(continuation);
        }

        /// <summary>Schedules the continuation action for this LightTask.</summary>
        public void UnsafeOnCompleted(Action continuation)
        {
            _value.AsTaskAsync().ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }
}