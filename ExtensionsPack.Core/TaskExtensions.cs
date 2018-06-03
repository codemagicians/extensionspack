using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensionsPack.Core
{
    public static class TaskExtensions
    {
        /// <summary>
        /// A timeout value that will be used in case when not specified otherwise
        /// By default it set to 15 seconds
        /// </summary>
        public static TimeSpan DefaultTimeOut { get; set; }

        /// <summary>
        /// A default sleep interval. By default it set to 100 milliseconds
        /// </summary>
        public static TimeSpan SleepInterval { get; set; }

        static TaskExtensions()
        {
            DefaultTimeOut = TimeSpan.FromSeconds(15);
            SleepInterval = TimeSpan.FromMilliseconds(100);
        }

        /// <summary>
        /// Checks that running task does not exceed the time limits otherwise throws TimeOutException
        /// </summary>
        /// <param name="task">Task to run</param>
        /// <param name="timeout">Timeout value after which the TimeoutException is thrown</param>
        /// <returns></returns>
        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan? timeout = null)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (await Task.WhenAny(task, Task.Delay(timeout ?? DefaultTimeOut, timeoutCancellationTokenSource.Token)).ConfigureAwait(false) != task)
                {
                    throw new TimeoutException("The operation has timed out");
                }

                timeoutCancellationTokenSource.Cancel();
                return await task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks that running task does not exceed the time limits otherwise throws TimeOutException
        /// </summary>
        /// <param name="task">Target Task</param>
        /// <param name="timeout">Timeout value after which the TimeoutException is thrown</param>
        /// <returns>Resulting Task</returns>
        public static async Task WithTimeout(this Task task, TimeSpan? timeout = null)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (await Task.WhenAny(task, Task.Delay(timeout ?? DefaultTimeOut, timeoutCancellationTokenSource.Token)).ConfigureAwait(false) != task)
                {
                    throw new TimeoutException("The operation has timed out.");
                }
                timeoutCancellationTokenSource.Cancel();
                await task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Unwraps a 'hot' task with timeout and returns result
        /// </summary>
        /// <param name="task">Task to unwrap</param>
        /// <param name="timeout">Timeout value</param>
        /// <returns>Result returned by Task</returns>
        public static TResult UnWrapWithTimeout<TResult>(this Task<TResult> task, TimeSpan? timeout = null)
        {
            return WithTimeout(task, timeout).Result;
        }

        /// <summary>
        /// Unwraps a 'hot' task with timeout
        /// </summary>
        /// <param name="task">Task to unwrap</param>
        /// <param name="timeout">Timeout value</param>
        public static void UnWrapWithTimeout(this Task task, TimeSpan? timeout = null)
        {
            WithTimeout(task, timeout).Wait();
        }

        /// <summary>
        /// Make thread sleep until the condition is met or max sleep time is exceeded
        /// </summary>
        /// <param name="maxSleepTime">Time after which a thread must be released</param>
        /// <param name="interval">Time interval between checks</param>
        /// <param name="predicateFunc">A predicate function that checks the condition</param>
        public static void SleepWhile(TimeSpan maxSleepTime, Func<bool> predicateFunc, TimeSpan? interval = null)
        {
            if (predicateFunc == null)
            {
                throw new ArgumentNullException(nameof(predicateFunc));
            }

            if (!interval.HasValue)
            {
                interval = SleepInterval;
            }
            var msInterval = interval.Value.Milliseconds;

            for (var i = 0; i <= maxSleepTime.TotalMilliseconds && predicateFunc.Invoke(); i += msInterval)
            {
                Thread.Sleep(msInterval);
            }
        }

        /// <summary>
        /// Make thread sleep until the condition is met or max sleep time is exceeded
        /// </summary>
        /// <param name="maxSleepTime">Time after which a thread must be released in ms</param>
        /// <param name="intervalMs">Time interval between checks in ms</param>
        /// <param name="predicateFunc">A predicate function that checks the condition</param>
        public static void Sleep(double maxSleepTime, Func<bool> predicateFunc, double? intervalMs = null)
        {
            if (predicateFunc == null)
            {
                throw new ArgumentNullException(nameof(predicateFunc));
            }
            TimeSpan? intervalTimeSpan = intervalMs.HasValue ? TimeSpan.FromMilliseconds(intervalMs.Value) : (TimeSpan?)null;
            SleepWhile(TimeSpan.FromMilliseconds(maxSleepTime), predicateFunc, intervalTimeSpan);
        }

        /// <summary>
        /// Asynchronously waits for a condition to be true in certain time frames
        /// </summary>
        /// <param name="predicateFunc">A predicate function with condition</param>
        /// <param name="timeout">A timeout after which an exception is thrown</param>
        /// <returns></returns>
        public static Task WaitFor(Expression<Func<bool>> predicateFunc, TimeSpan timeout, TimeSpan? interval = null, bool configureAwait = false)
        {
            Func<bool> predicateFuncDel = predicateFunc.Compile();
            interval = interval ?? TimeSpan.FromMilliseconds(1);
            return Task.Run(async () =>
            {
                while (!predicateFuncDel())
                {
                    await Task.Delay(interval.Value).ConfigureAwait(false);
                }
                SleepWhile(timeout, predicateFuncDel, interval);
                if (!predicateFuncDel())
                {
                    throw new TimeoutException();
                }
            });
        }

        /// <summary>
        /// Asynchronously waits for a condition to be true for specific object in certain time frames
        /// </summary>
        /// <param name="predicateFunc">A predicate function that checks object</param>
        /// <param name="getEntityExpression">A function that returns the target object</param>
        /// <param name="timeout">A timeout after which an exception is thrown</param>
        /// <param name="interval">An interval between checks</param>
        /// <returns></returns>
        public static Task WaitFor<T>(Func<T, bool> predicateFunc, Expression<Func<T>> getEntityExpression, TimeSpan timeout, TimeSpan? interval = null)
        {
            Func<T> getEntityDelegate = getEntityExpression.Compile();
            interval = interval ?? TimeSpan.FromMilliseconds(1);
            return Task.Run(async () =>
            {
                while (!predicateFunc(getEntityDelegate()))
                {
                    await Task.Delay(interval.Value).ConfigureAwait(false);
                }
            }).WithTimeout(timeout);
        }
    }
}
