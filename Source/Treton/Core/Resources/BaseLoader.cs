using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treton.Core.Concurrency;

namespace Treton.Core.Resources
{
	public abstract class BaseLoader<TResource, TResourceData> : ILoader
	{
		public async Task<object> Load(ResourceId id, System.IO.Stream stream)
		{
			var resourceData = await LoadImpl(id, stream);
			var resource = await TaskHelpers.RunOnMainThread(() => BringIn(resourceData));
			return resource;
		}

		public async Task Unload(object resource)
		{
			await TaskHelpers.RunOnMainThread(() => BringOut((TResource)resource));
			await UnloadImpl((TResource)resource);
		}

		protected abstract Task<TResourceData> LoadImpl(ResourceId id, System.IO.Stream stream);
		protected abstract Task UnloadImpl(TResource resource);

		protected abstract TResource BringIn(TResourceData resourceData);
		protected abstract void BringOut(TResource resource);
	}
}
