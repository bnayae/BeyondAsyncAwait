// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="LightTask{TResult}"/>.</summary>
    /// <typeparam name="TResult">The type of the result produced.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public struct ConfiguredLightTaskAwaitable<TResult>
    {
        /// <summary>The wrapped <see cref="LightTask{TResult}"/>.</summary>
        private readonly LightTask<TResult> _value;
        /// <summary>true to attempt to marshal the continuation back to the original context captured; otherwise, false.</summary>
        private readonly bool _continueOnCapturedContext;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="LightTask{TResult}"/>.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal ConfiguredLightTaskAwaitable(LightTask<TResult> value, bool continueOnCapturedContext)
        {
            _value = value;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredLightTaskAwaitable{TResult}"/> instance.</summary>
        public ConfiguredLightTaskAwaiter GetAwaiter()
        {
            return new ConfiguredLightTaskAwaiter(_value, _continueOnCapturedContext);
        }

        /// <summary>Provides an awaiter for a <see cref="ConfiguredLightTaskAwaitable{TResult}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct ConfiguredLightTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>The value being awaited.</summary>
            private readonly LightTask<TResult> _value;
            /// <summary>The value to pass to ConfigureAwait.</summary>
            private readonly bool _continueOnCapturedContext;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            /// <param name="continueOnCapturedContext">The value to pass to ConfigureAwait.</param>
            internal ConfiguredLightTaskAwaiter(LightTask<TResult> value, bool continueOnCapturedContext)
            {
                _value = value;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>Gets whether the <see cref="ConfiguredLightTaskAwaitable{TResult}"/> has completed.</summary>
            public bool IsCompleted { get { return _value.IsCompleted; } }

            /// <summary>Gets the result of the LightTask.</summary>
            public TResult GetResult()
            {
                return _value._task == null ?
                    _value._result :
                    _value._task.GetAwaiter().GetResult();
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredLightTaskAwaitable{TResult}"/>.</summary>
            public void OnCompleted(Action continuation)
            {
                _value.AsTaskAsync().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            }

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredLightTaskAwaitable{TResult}"/>.</summary>
            public void UnsafeOnCompleted(Action continuation)
            {
                _value.AsTaskAsync().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            }
        }
    }
}