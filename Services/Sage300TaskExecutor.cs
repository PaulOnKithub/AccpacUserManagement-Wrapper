using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AccpacUserManagement_Wrapper.Services
{
    /// <summary>
    /// Singleton executor service that manages a dedicated STA thread for executing Sage 300 tasks.
    /// All Sage 300 API calls must run on this single thread to avoid COM interop threading issues.
    /// </summary>
    public sealed class Sage300TaskExecutor : IDisposable
    {
        private static readonly Lazy<Sage300TaskExecutor> _instance =
            new Lazy<Sage300TaskExecutor>(() => new Sage300TaskExecutor());

        private Thread _executorThread;
        private BlockingCollection<WorkItem> _taskQueue;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private bool _isRunning = false;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Gets the singleton instance of the Sage300TaskExecutor.
        /// </summary>
        public static Sage300TaskExecutor Instance => _instance.Value;

        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// </summary>
        private Sage300TaskExecutor()
        {
            Debug.WriteLine("Sage300TaskExecutor instance created");
        }

        /// <summary>
        /// Starts the executor thread. This should be called once during application startup.
        /// </summary>
        public void Start()
        {
            lock (_lockObject)
            {
                if (_isRunning)
                {
                    Debug.WriteLine("Sage300TaskExecutor already running");
                    return;
                }

                _taskQueue = new BlockingCollection<WorkItem>();
                _cancellationTokenSource = new CancellationTokenSource();

                // Create and start the STA thread
                _executorThread = new Thread(ExecutorThreadLoop)
                {
                    Name = "Sage300ExecutorThread",
                    IsBackground = false // Make it a foreground thread to ensure proper shutdown
                };

                // CRITICAL: Set apartment state to STA for COM interop
                _executorThread.SetApartmentState(ApartmentState.STA);
                _executorThread.Start();

                _isRunning = true;
                Debug.WriteLine($"Sage300TaskExecutor started on thread ID: {_executorThread.ManagedThreadId}");
            }
        }

        /// <summary>
        /// Stops the executor thread gracefully. This should be called during application shutdown.
        /// </summary>
        public void Stop()
        {
            lock (_lockObject)
            {
                if (!_isRunning)
                {
                    Debug.WriteLine("Sage300TaskExecutor not running");
                    return;
                }

                Debug.WriteLine("Stopping Sage300TaskExecutor...");

                // Signal cancellation and complete the queue
                _cancellationTokenSource?.Cancel();
                _taskQueue?.CompleteAdding();

                // Wait for the thread to finish (with timeout)
                if (_executorThread != null && _executorThread.IsAlive)
                {
                    bool joined = _executorThread.Join(TimeSpan.FromSeconds(10));
                    if (!joined)
                    {
                        Debug.WriteLine("Warning: Sage300TaskExecutor thread did not stop gracefully within timeout");
                    }
                }

                // Clean up resources
                _taskQueue?.Dispose();
                _cancellationTokenSource?.Dispose();

                _isRunning = false;
                Debug.WriteLine("Sage300TaskExecutor stopped");
            }
        }

        /// <summary>
        /// Executes a function asynchronously on the dedicated Sage 300 thread.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="work">The function to execute on the Sage 300 thread.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task<T> ExecuteAsync<T>(Func<T> work)
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Sage300TaskExecutor is not running. Call Start() first.");
            }

            if (work == null)
            {
                throw new ArgumentNullException(nameof(work));
            }

            var workItem = new WorkItem<T>(work);

            try
            {
                _taskQueue.Add(workItem);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Sage300TaskExecutor is shutting down and cannot accept new tasks.");
            }

            return workItem.TaskCompletionSource.Task;
        }

        /// <summary>
        /// Executes an action asynchronously on the dedicated Sage 300 thread.
        /// </summary>
        /// <param name="work">The action to execute on the Sage 300 thread.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ExecuteAsync(Action work)
        {
            if (work == null)
            {
                throw new ArgumentNullException(nameof(work));
            }

            return ExecuteAsync(() =>
            {
                work();
                return true; // Return a dummy value
            });
        }

        /// <summary>
        /// The main loop that runs on the dedicated STA thread.
        /// </summary>
        private void ExecutorThreadLoop()
        {
            Debug.WriteLine($"Sage300 executor thread started. Thread ID: {Thread.CurrentThread.ManagedThreadId}, ApartmentState: {Thread.CurrentThread.GetApartmentState()}");

            try
            {
                foreach (var workItem in _taskQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    try
                    {
                        Debug.WriteLine($"Executing work item on thread ID: {Thread.CurrentThread.ManagedThreadId}");
                        workItem.Execute();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error executing work item: {ex.Message}");
                        // Exception is handled within workItem.Execute()
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Sage300 executor thread cancelled");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fatal error in Sage300 executor thread: {ex.Message}");
            }

            Debug.WriteLine("Sage300 executor thread exiting");
        }

        /// <summary>
        /// Gets whether the executor is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Disposes the executor and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Stop();
            }

            _disposed = true;
        }

        /// <summary>
        /// Base class for work items.
        /// </summary>
        private abstract class WorkItem
        {
            public abstract void Execute();
        }

        /// <summary>
        /// Represents a work item to be executed on the Sage 300 thread.
        /// </summary>
        private class WorkItem<T> : WorkItem
        {
            private readonly Func<T> _work;
            public TaskCompletionSource<T> TaskCompletionSource { get; }

            public WorkItem(Func<T> work)
            {
                _work = work ?? throw new ArgumentNullException(nameof(work));
                TaskCompletionSource = new TaskCompletionSource<T>();
            }

            public override void Execute()
            {
                try
                {
                    T result = _work();
                    TaskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Work item execution failed: {ex.Message}");
                    TaskCompletionSource.SetException(ex);
                }
            }
        }
    }
}