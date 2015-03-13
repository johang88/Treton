using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks.Schedulers;

namespace Treton.Core.Resources
{
	public interface IResourceManager
	{
		Task<object> Load(IPackage package, ResourceId id);
		void Unload(ResourceId id);

		object Get(ResourceId id);
		TResource Get<TResource>(ResourceId resource);

		Task GarbageCollect();
	}

	public class ResourceManager : IResourceManager
	{
		private readonly IResourceLoaders _loaders;
		private readonly Dictionary<ResourceId, Reference> _resources = new Dictionary<ResourceId, Reference>();
		private SemaphoreSlim _synch = new SemaphoreSlim(1);

		public ResourceManager(IResourceLoaders loaders)
		{
			if (loaders == null)
				throw new ArgumentNullException("loaders");

			_loaders = loaders;
		}

		public async Task<object> Load(IPackage package, ResourceId id)
		{
			await _synch.WaitAsync();

			Reference reference = null;
			if (_resources.ContainsKey(id))
			{
				reference = _resources[id];
			}
			else
			{
				var loader = _loaders.Get(id.Type);

				using (var stream = package.Open(id))
				{
					var resource = await loader.Load(id, stream);
					reference = new Reference(resource);
					_resources.Add(id, reference);
				}
			}

			reference.AddReference();
			
			_synch.Release();

			return reference.Resource;
		}

		public void Unload(ResourceId id)
		{
			if (_resources.ContainsKey(id))
			{
				_resources[id].RemoveReference();
			}
		}

		public async Task GarbageCollect()
		{
			await _synch.WaitAsync();

			var garbage = _resources.Where(kv => !kv.Value.IsReferenced).ToList();
			foreach (var kv in garbage)
			{
				var id = kv.Key;
				var reference = kv.Value;

				var loader = _loaders.Get(id.Type);
				await loader.Unload(reference.Resource);

				_resources.Remove(id);
			}

			_synch.Release();
		}

		public object Get(ResourceId id)
		{
			return _resources[id].Resource;
		}

		public TResource Get<TResource>(ResourceId resource)
		{
			return (TResource)Get(resource);
		}

		private class Reference
		{
			public readonly object Resource;
			private int _count = 0;

			public Reference(object resource)
			{
				if (resource == null)
					throw new ArgumentNullException("resource");

				Resource = resource;
			}

			public void AddReference()
			{
				_count++;
			}

			public void RemoveReference()
			{
				_count--;
			}

			public bool IsReferenced
			{
				get { return _count > 0; }
			}
		}
	}
}
