using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treton.Core;
using Treton.Core.Resources;

namespace Treton.Graphics.ResourceLoaders
{
	class MaterialLoader : BaseLoader<Material, MaterialData.Material>
	{
		public MaterialLoader(MainThreadScheduler scheduler)
			: base(scheduler)
		{

		}

		protected override async Task<MaterialData.Material> LoadImpl(ResourceId id, System.IO.Stream stream)
		{
			byte[] data = new byte[stream.Length];
			var offset = 0;
			while (offset < stream.Length)
			{
				offset += await stream.ReadAsync(data, offset, (int)stream.Length - offset);
			}

			using (var ms = new MemoryStream(data))
			using (BsonReader bson = new BsonReader(ms))
			{
				var serializer = new JsonSerializer();
				return serializer.Deserialize<MaterialData.Material>(bson);
			}
		}

		protected override Task UnloadImpl(Material resource)
		{
			// Not much to do here
			return Task.FromResult(0);
		}

		protected override Material BringIn(MaterialData.Material resourceData)
		{
			var shaders = new Shader[resourceData.Shaders.Length];
			for (var i = 0; i < shaders.Length; i++)
			{
				shaders[i] = new Shader(resourceData.Shaders[i].Type, resourceData.Shaders[i].Source);
			}

			var layers = new Material.Layer[resourceData.Layers.Length];
			for (var i = 0; i < layers.Length; i++)
			{
				var layer = resourceData.Layers[i];

				var passes = new Material.Pass[layer.Passes.Length];
				for (var p  = 0; p < passes.Length; p++)
				{
					var pass = layer.Passes[p];
					
					var passShaders = new Shader[pass.Shaders.Length];
					for(var s = 0; s < passShaders.Length; s++)
					{
						passShaders[s] = shaders[pass.Shaders[s]];
					}

					passes[i] = new Material.Pass(passShaders);
				}

				layers[i] = new Material.Layer(layer.Name, passes);
			}

			return new Material(shaders, layers);
		}

		protected override void BringOut(Material resource)
		{
			resource.Dispose();
		}
	}
}
