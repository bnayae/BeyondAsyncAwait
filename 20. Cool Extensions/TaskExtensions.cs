using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Task Extensions
    /// </summary>
    public static class TaskExtensions
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(15);

        #region WithTimeout

        /// <summary>
        /// Creates task with timeout.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException">Operations was timeout after [{timeout.TotalMinutes:N3}]</exception>
        [DebuggerHidden]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public static async Task<T> WithTimeout<T>(
            this Task<T> task,
            TimeSpan timeout = default(TimeSpan))
        {
            //return task;
            await WithTimeout((Task)task, timeout).ConfigureAwait(false);

            return await task.ConfigureAwait(false); 
        }

        /// <summary>
        /// Creates task with timeout.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException">Operations was timeout after [{timeout.TotalMinutes:N3}]</exception>
        [DebuggerHidden]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public static async Task WithTimeout(
            this Task task,
            TimeSpan timeout = default(TimeSpan))
        {
            //return task;
            if (timeout == default(TimeSpan))
                timeout = DefaultTimeout;

            Task check = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);
            if (check != task)
            {
                throw new TimeoutException($"Operations was timeout after [{timeout.TotalMinutes:N3}] minutes");
            }
            else
                await task.ConfigureAwait(false); // throw if faulted
        }

        #endregion // WithTimeout

        #region IsTimeoutAsync

        /// <summary>
        /// Check for completion of task.
        /// Ideal for checking for expected execution duration without exception.
        /// Can be use to produce warning in case of potential deadlocks.
        /// </summary>
        /// <param name="initTask">The initialize task.</param>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        [DebuggerNonUserCode]
        public static async Task<bool> IsTimeoutAsync(
            this Task initTask, 
            TimeSpan duration)
        {
            Task delay = Task.Delay(duration);
            Task deadlockDetection = await Task.WhenAny(initTask, delay).ConfigureAwait(false);
            if (deadlockDetection == delay)
            {
                return false;
            }
            else
            {
                await initTask.ConfigureAwait(false);
                return true;
            }
        }

        #endregion // IsTimeoutAsync

        #region CancelSafe

        /// <summary>
        /// Cancels the safe.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static bool CancelSafe(
            this CancellationTokenSource instance)
        {
            return CancelSafe(instance, out Exception ex);
        }

        /// <summary>
        /// Cancels the safe.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="exc">The exception (on fault cancellation).</param>
        /// <returns></returns>
        public static bool CancelSafe(
            this CancellationTokenSource instance, out Exception exc)
        {
            exc = null;
            try
            {
                instance.Cancel();
            }
            #region Exception Handling

            catch (Exception ex)
            {
                exc = ex;
                return false;
            }

            #endregion // Exception Handling
            return true;
        }

        #endregion // WithCancellation

        #region RegisterWeak

        /// <summary>
        /// Weak registration for cancellation token.
        /// </summary>
        /// <param name="cancellation">The cancellation.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        //[DebuggerHidden]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public static CancellationTokenRegistration RegisterWeak(
            this CancellationToken cancellation,
            Action action)
        {
            var weak = new WeakReference<Action>(action);
            Action tmp = () =>
            {
                if (weak.TryGetTarget(out Action act))
                    act();
            };
            return cancellation.Register(tmp);
        }

        /// <summary>
        /// Weak registration for cancellation token.
        /// </summary>
        /// <param name="cancellation">The cancellation.</param>
        /// <param name="action">The action.</param>
        /// <param name="state">The state.</param>
        //[DebuggerHidden]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public static CancellationTokenRegistration RegisterWeak(
            this CancellationToken cancellation,
            Action<object> action,
            object state)
        {
            var weak = new WeakReference<Action<object>>(action);
            Action<object> tmp = (state_) =>
            {
                if (weak.TryGetTarget(out Action<object> act))
                    act(state_);
            };
            return cancellation.Register(tmp, state);
        }

        #endregion // RegisterWeak

        #region ThrowAll

        /// <summary>
        /// Throws all (when catching exception withing async / await
        /// and there is potential for multiple exception to be thrown.
        /// async / await will propagate single exception.
        /// in order to catch all the exception use this extension).
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Task ThrowAll(this Task t)
        {
            var result = t.ContinueWith(c =>
            {
                if (c.Exception == null)
                    return;
                throw new AggregateException(c.Exception.Flatten().InnerExceptions);
            });
            return result;
        }

        /// <summary>
        /// Throws all (when catching exception withing async / await
        /// and there is potential for multiple exception to be thrown.
        /// async / await will propagate single exception.
        /// in order to catch all the exception use this extension).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Task<T> ThrowAll<T>(this Task<T> t)
        {
            var result = t.ContinueWith(c =>
            {
                if (c.Exception == null)
                    return c.Result;
                throw new AggregateException(c.Exception.Flatten().InnerExceptions);
            });
            return result;
        }

        #endregion // ThrowAll
    }
}