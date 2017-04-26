using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public static class AsyncExtensions
    {
        #region Constants

        //private static readonly Regex ASYNC_REGEX = new Regex(@"\.(.*)\.<(.*)>d__"); // .{group 0 - any}.<{group 1 = any}>d__
        // "^\s*at = start with 'at ' optional preceding whitespace 
        // (.*\)) = group any until ')'
        private static readonly Regex SYNC_REGEX = new Regex(@"^\s*at (.*\))");

        private static readonly Regex EXCLUDE = new Regex(@"[\W|_]"); // not char, digit  
        private static readonly Regex EXCLUDE_FROM_START = new Regex(@"^[\W|_]"); // not start with char, digit  
        private static readonly Regex ASYNC_REGEX1 = new Regex(@"^\s*at\s+(?<namespace>[\w|\.]+)\<(?<method>\w+)\>d__[0-9]+.MoveNext\(\)\sin"); // at {namespace}<{method}>d__##.MoveNext() in
        private static readonly Regex ASYNC_REGEX2 = new Regex(@"^\s*at\s+(?<namespace>[\w|\.]+)\<\>c\.\<\<(?<method>\w+)\>b__[0-9|_]+\>d\.MoveNext\(\)\s+in"); // at {namespace}<>c.<<{method}>b__##_##>d.MoveNext() in
        private static readonly Regex ASYNC_REGEX3 = new Regex(@"^\s*at\s+(?<namespace>[\w|\.]+)\<\>c__DisplayClass[0-9|_]+\.\<\<(?<method>\w+)\>b__[0-9]+\>d\.MoveNext\(\)"); // at {namespace}<>c__DisplayClass##_##.<<{method}>b__##>d.MoveNext()
        private static readonly Regex ASYNC_REGEX4 = new Regex(@"^\s*at\s+(?<namespace>[\w|\.]+)\<\>c\.\<(?<method>\w+)\>b__[0-9|_]+\(\)"); // at {namespace}<>c.<{method}>b__##_##() in
        private static readonly Regex ASYNC_REGEX5 = new Regex(@"^\s*at\s+System\.Threading\.Tasks\.Task.*\.InnerInvoke\(\)"); // at System.Threading.Tasks.Task`1.InnerInvoke()
        private static readonly string[] IGNORE_START_WITH =
            {
                "at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess",
                "at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification",
                "at System.Runtime.CompilerServices.TaskAwaiter.GetResult()",
                "at System.AppDomain.",
                "at System.Threading.ThreadHelper.ThreadStart_",
                "at System.Threading.ExecutionContext.Run",
                "--- End of stack trace from previous location where exception was thrown ---", // all async method do this
                "at System.Threading.Tasks.Task.Execute()",
                "at System.Threading.Tasks.ContinuationTaskFromTask.InnerInvoke()",
                "at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()",
                "at System.Runtime.CompilerServices.TaskAwaiter",
            };

        #endregion // Constants

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

        #region Format

        /// <summary>
        /// Simplify the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="stackTracer">The stack tracer.</param>
        /// <returns></returns>
        public static string Format(
                this Exception exception, bool includeRawInfo = true)
        {
            if (exception == null)
                return string.Empty;
            try
            {
                var keep = new List<string>();

                HandleExceptionFlow(exception, keep);

                var builder = new StringBuilder();

                var aggregate = exception as AggregateException;
                if (aggregate != null)
                {
                    builder.AppendLine("Root Causes:");
                    foreach (var ex in aggregate.Flatten().InnerExceptions)
                    {
                        var root = ex?.GetBaseException();
                        var rootMessage = root?.Message;
                        builder.AppendLine($"  [{root?.GetType()?.Name}]: Reason = {rootMessage}");
                    }
                }
                else
                    builder.AppendLine("Stack and message info");

                for (int i = keep.Count - 1; i >= 0; i--)
                {
                    builder.Append(keep[i]);
                }

                if (includeRawInfo)
                {
                    builder.AppendLine("\r\n======================= Raw info ===========================");
                    builder.AppendLine(exception.ToString());
                }
                return builder.ToString();
            }
            catch
            {
                return exception.ToString();
            }


            #region HandleExceptionFlow (local function)

            /// <summary>
            /// Handles the exception flow.
            /// </summary>
            /// <param name="exc">The exception.</param>
            /// <param name="keep">The keep.</param>
            void HandleExceptionFlow(
                Exception exc,
                List<string> keep)
            {
                if (exc is AggregateException aggregate)
                {
                    var exceptions = aggregate.Flatten().InnerExceptions;
                    if (exceptions.Count != 1)
                    {
                        int count = exceptions.Count;
                        foreach (var ex in exceptions)
                        {
                            HandleExceptionFlow(ex, keep);
                            var root = ex?.GetBaseException();
                            keep.Add($"\r\nRoot Cause #{count--}\r\n");
                        }
                        return;
                    }
                    exc = exceptions[0];
                }

                while (exc != null)
                {
                    List<string> keepLocal = new List<string>();
                    using (var r = new StringReader(exc.StackTrace))
                    {
                        keepLocal.Add($"  ## [{exc.GetType().Name}]: Reason = {exc.Message} \r\n");
                        while (true)
                        {
                            string line = r.ReadLine();
                            if (line == null)
                                break;
                            line = line.Trim();

                            if (IGNORE_START_WITH.Any(ignore => line.StartsWith(ignore)))
                                continue;

                            if (line.StartsWith("at System.Threading.ThreadHelper.ThreadStart()"))
                            {
                                keepLocal.Add("\tStart Thread:");
                                continue;
                            }
                            var m = ASYNC_REGEX1.Match(line);
                            if (m?.Success ?? false)
                            {
                                string data = $"\t{m.Groups?["namespace"]?.Value ?? "Missing"}{m.Groups?["method"]?.Value ?? "Missing"}(?) <-\r\n";
                                keepLocal.Add(data);
                                continue;
                            }
                            m = ASYNC_REGEX2.Match(line);
                            if (m?.Success ?? false)
                            {
                                string data = $"\tanonymous: {m.Groups?["namespace"]?.Value ?? "Missing"}{m.Groups?["method"]?.Value ?? "Missing"}(?) <-\r\n";
                                keepLocal.Add(data);
                                continue;
                            }
                            m = ASYNC_REGEX3.Match(line);
                            if (m?.Success ?? false)
                            {
                                string data = $"\t{m.Groups?["namespace"]?.Value ?? "Missing"}{m.Groups?["method"]?.Value ?? "Missing"}(?) <-\r\n";
                                keepLocal.Add(data);
                                continue;
                            }
                            m = ASYNC_REGEX4.Match(line);
                            if (m?.Success ?? false)
                            {
                                string data = $"\tanonymous: {m.Groups?["namespace"]?.Value ?? "Missing"}{m.Groups?["method"]?.Value ?? "Missing"}(?) <-\r\n";
                                keepLocal.Add(data);
                                continue;
                            }
                            m = ASYNC_REGEX5.Match(line);
                            if (m?.Success ?? false)
                            {
                                keepLocal.Add("\tStart Task: ");
                                continue;
                            }
                            m = SYNC_REGEX.Match(line);
                            if (m?.Success ?? false)
                            {
                                string data = m.Groups?[1]?.Value ?? "Missing";
                                keepLocal.Add($"\t{data} <-\r\n");
                                continue;
                            }
                            keepLocal.Add($"\t{line}\r\n");
                        }
                    }
                    keepLocal.Reverse();
                    keep.AddRange(keepLocal);
                    exc = exc.InnerException;
                    if (exc != null)
                        keep.Add("  -------------^-------------^-------------\r\n");
                }
            }

            #endregion // HandleExceptionFlow (local function)
        }

        #endregion // Format
    }
}
