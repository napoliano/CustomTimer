using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace CustomTimer
{
    public partial class Timer : Singleton<Timer>
    {
        private readonly ConcurrentBag<TimerTask> _taskBuffer = new();

        private readonly PriorityQueue<TimerTask, long> _priorityQueue = new();

        private readonly TaskQueueProcessor<TimerTask> _taskQueueProcessor = new(GlobalDefine.TimerThreadCount);

        private readonly Thread _checker;


        public Timer()
        {
            _checker = new Thread(DeliverTasksToQueue);
            _checker.Name = $"Timer Checker";
            _checker.Start();
        }

        public void AddTimerTask(ulong delayMilliseconds, Action<object?> action, object? arg = null)
        {
            _taskBuffer.Add(new TimerTask(delayMilliseconds, action, arg));
        }

        private void DeliverTasksToQueue()
        {
            while (true)
            {
                long nowTick = DateTime.Now.Ticks;

                while (_taskBuffer.TryTake(out var timerTask))
                {
                    long executeTick = timerTask.ExecutionTime.Ticks;
                    if (executeTick < nowTick)
                    {
                        _taskQueueProcessor.Enqueue(timerTask);
                    }
                    else
                    {
                        _priorityQueue.Enqueue(timerTask, executeTick);
                    }
                }

                while (true)
                {
                    if (false == _priorityQueue.TryPeek(out var timerTask, out long executeTick))
                        break;

                    if (executeTick > nowTick)
                        break;

                    _priorityQueue.Dequeue();
                    _taskQueueProcessor.Enqueue(timerTask);
                }

                Thread.Sleep(1);
            }
        }
    }
}
