using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CustomTimer
{
    public interface ITaskQueueProcessor
    {
        public void Execute();
    }

    public class TaskQueueProcessor<T> where T : ITaskQueueProcessor
    {
        private readonly ConcurrentQueue<T> _taskQueue = new();

        private readonly List<Thread> _workers = new();

        private readonly AutoResetEvent _autoResetEvent = new(false);

        private bool _stop;


        public TaskQueueProcessor(uint workerCount, string name = "")
        {
            if (0 == workerCount)
            {
                throw new ArgumentOutOfRangeException(nameof(workerCount), "Worker count cannot be negative.");
            }

            for (int i = 0; i < workerCount; ++i)
            {
                var worker = new Thread(ProcessTasks);
                worker.Name = $"{(string.IsNullOrEmpty(name) ? "" : name + " ")} TaskQueueProcessor Worker {i}";
                worker.Start();

                _workers.Add(worker);
            }
        }

        public void Enqueue(T task)
        {
            if (true == _stop)
                return;

            _taskQueue.Enqueue(task);
            _autoResetEvent.Set();
        }

        private void ProcessTasks()
        {
            while (false == _stop)
            {
                if (true == _taskQueue.TryDequeue(out var task))
                {
                    task.Execute();
                }
                else
                {
                    _autoResetEvent.WaitOne();
                }
            }
        }

        public void Release()
        {
            _stop = true;

            foreach (var worker in _workers)
            {
                _autoResetEvent.Set();
            }

            foreach (var worker in _workers)
            {
                worker.Join();
            }

            _taskQueue.Clear();
            _workers.Clear();
            _autoResetEvent.Close();
        }
    }
}
