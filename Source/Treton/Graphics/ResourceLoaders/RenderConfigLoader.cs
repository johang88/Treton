using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Treton.Core;
using Treton.Core.Resources;
using Treton.Core.Concurrency;

namespace Treton.Graphics.ResourceLoaders
{
	class RenderConfigLoader : ILoader
	{
		private readonly Core.Resources.IResourceManager _resourceManager;
		private readonly RenderSystem _renderSystem;

		public RenderConfigLoader(Core.Resources.IResourceManager resourceManager, RenderSystem renderSystem)
		{
			if (resourceManager == null)
				throw new ArgumentNullException("resourceManager");
			if (renderSystem == null)
				throw new ArgumentNullException("renderSystem");

			_resourceManager = resourceManager;
			_renderSystem = renderSystem;
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
			resource.Initialize(_resourceManager, _renderSystem);
		}

		private void BringOut(Renderer.RendererConfiguration resource)
		{
			resource.Teardown();
		}

		public async Task<object> Load(ResourceId id, System.IO.Stream stream)
		{
			var resource = await LoadImpl(id, stream);
			await TaskHelpers.RunOnMainThread(() => BringIn(resource));

			return resource;
		}

		public async Task Unload(object resource)
		{
			await TaskHelpers.RunOnMainThread(() => BringOut((Renderer.RendererConfiguration)resource));
			await UnloadImpl((Renderer.RendererConfiguration)resource);
		}
	}
}
