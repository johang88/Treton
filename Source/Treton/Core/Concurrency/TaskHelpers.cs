using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core.Concurrency
{
	public static class TaskHelpers
	{
		private static TaskFactory _factory;

		internal static void Initialize(MainThreadScheduler mainThreadScheduler)
		{
			_factory = new TaskFactory(mainThreadScheduler);
		}

		public static Task RunOnMainThread(Action f)
		{
			return _factory.StartNew(f);
		}

		public static Task<T> RunOnMainThread<T>(Func<T> f)
		{
			return _factory.StartNew(f);
		}
	}
}
