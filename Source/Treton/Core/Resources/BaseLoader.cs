using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core.Resources
{
	public abstract class BaseLoader<TResource, TResourceData> : ILoader
	{
		private readonly MainThreadScheduler _scheduler;
		private readonly TaskFactory _factory;

		public BaseLoader(MainThreadScheduler scheduler)
		{
			if (scheduler == null)
				throw new ArgumentNullException("scheduler");

			_scheduler = scheduler;
			_factory = new TaskFactory(scheduler);
		}

		public async Task<object> Load(ResourceId id, System.IO.Stream stream)
		{
			var resourceData = await LoadImpl(id, stream);
			var resource = await _factory.StartNew(() => BringIn(resourceData));
			return resource;
		}

		public async Task Unload(object resource)
		{
			await _factory.StartNew(() => BringOut((TResource)resource));
			await UnloadImpl((TResource)resource);
		}

		protected abstract Task<TResourceData> LoadImpl(ResourceId id, System.IO.Stream stream);
		protected abstract Task UnloadImpl(TResource resource);

		protected abstract TResource BringIn(TResourceData resourceData);
		protected abstract void BringOut(TResource resource);
	}
}
