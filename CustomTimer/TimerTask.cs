using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTimer
{
    internal readonly struct TimerTask : ITaskQueueProcessor
    {
        public DateTime ExecutionTime => _executionTime;
        private readonly DateTime _executionTime;

        private readonly Action<object?> _action;

        private readonly object? _arg;


        public TimerTask(ulong delayMilliseconds, Action<object?> action, object? arg = null)
        {
            _executionTime = DateTime.Now.AddMilliseconds(delayMilliseconds);
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _arg = arg;
        }

        public void Execute()
        {
            _action(_arg);
        }
    }
}
