using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class PoolScheduler : TaskScheduler
    {
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private Thread[] _threads;

        public PoolScheduler(uint poolSize)
        {
            _threads = new Thread[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var t = new Thread(Work);
                t.Name = $"Pool Scheduler {i}";
                t.Start();
            }
        }

        private void Work()
        {
            foreach (var task in _tasks.GetConsumingEnumerable())
            {
                if(!base.TryExecuteTask(task))
                    Console.WriteLine("Error");
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks() => _tasks;

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}
