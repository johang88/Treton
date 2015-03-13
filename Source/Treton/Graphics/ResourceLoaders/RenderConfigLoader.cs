using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Treton.Core;
using Treton.Core.Resources;

namespace Treton.Graphics.ResourceLoaders
{
	class RenderConfigLoader : ILoader
	{
		private readonly MainThreadScheduler _scheduler;
		private readonly RenderSystem _renderSystem;
		private readonly TaskFactory _factory;

		public RenderConfigLoader(MainThreadScheduler scheduler, RenderSystem renderSystem)
		{
			if (scheduler == null)
				throw new ArgumentNullException("scheduler");
			if (renderSystem == null)
				throw new ArgumentNullException("renderSystem");

			_scheduler = scheduler;
			_renderSystem = renderSystem;
			_factory = new TaskFactory(scheduler);
		}

		private async Task<Renderer.RendererConfiguration> LoadImpl(ResourceId id, System.IO.Stream stream)
		{
			byte[] data = new byte[stream.Length];
			var offset = 0;
			while (offset < stream.Length)
			{
				offset += await stream.ReadAsync(data, offset, (int)stream.Length - offset);
			}

			using (var ms = new MemoryStream(data))
			{
				var formatter = new BinaryFormatter();
				return (Renderer.RendererConfiguration)formatter.Deserialize(ms);
			}
		}

		private Task UnloadImpl(Renderer.RendererConfiguration resource)
		{
			return Task.FromResult(0);
		}

		private void BringIn(Renderer.RendererConfiguration resource)
		{
			resource.Initialize(_renderSystem);
		}

		private void BringOut(Renderer.RendererConfiguration resource)
		{
			resource.Teardown();
		}

		public async Task<object> Load(ResourceId id, System.IO.Stream stream)
		{
			var resource = await LoadImpl(id, stream);
			await _factory.StartNew(() => BringIn(resource));

			return resource;
		}

		public async Task Unload(object resource)
		{
			await _factory.StartNew(() => BringOut((Renderer.RendererConfiguration)resource));
			await UnloadImpl((Renderer.RendererConfiguration)resource);
		}
	}
}
