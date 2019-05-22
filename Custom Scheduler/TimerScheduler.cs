using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Scheduler
{
    public class TimerScheduler : TaskScheduler
    {
        private readonly ConcurrentQueue<Task> _queue = new ConcurrentQueue<Task>();
        private readonly Timer _tmr;
        public new static readonly TaskScheduler Default = new TimerScheduler();

        public TimerScheduler()
        {
            _tmr = new Timer(state =>
            {
                while (_queue.TryDequeue(out var task))
                {
                    if (!base.TryExecuteTask(task))
                        Console.WriteLine("Error");
                }
            }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        protected override IEnumerable<Task> GetScheduledTasks() => _queue;

        protected override void QueueTask(Task task)
        {
            _queue.Enqueue(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}
