using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Treton.Core.Concurrency
{
	public class MainThreadScheduler : System.Threading.Tasks.TaskScheduler
	{
		private readonly ConcurrentQueue<Task> _pending = new ConcurrentQueue<Task>();
		private readonly Thread _mainThread;

		public MainThreadScheduler(Thread mainThread)
		{
			if (mainThread == null)
				throw new ArgumentNullException("mainThread");

			_mainThread = mainThread;
		}

		public bool HasScheduledTasks { get { return _pending.Count > 0; } }

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return _pending;
		}

		protected override void QueueTask(Task task)
		{
			_pending.Enqueue(task);
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (Thread.CurrentThread == _mainThread)
			{
				return TryExecuteTask(task);
			}
			else
			{
				return false;
			}
		}

		public void Tick(Stopwatch timer, long maxTicks)
		{
			var start = timer.ElapsedTicks;
			while ((timer.ElapsedTicks - start) < maxTicks && !_pending.IsEmpty)
			{
				Task task;
				if (_pending.TryDequeue(out task))
				{
					TryExecuteTask(task);
				}
			}
		}
	}
}
