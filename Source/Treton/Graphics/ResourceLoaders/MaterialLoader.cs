﻿using System;
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
	class MaterialLoader : BaseLoader<Material, MaterialData.Material>
	{
		protected override async Task<MaterialData.Material> LoadImpl(ResourceId id, System.IO.Stream stream)
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
				return (MaterialData.Material)formatter.Deserialize(ms);
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
				for (var p = 0; p < passes.Length; p++)
				{
					var pass = layer.Passes[p];

					var passShaders = new Shader[pass.Shaders.Length];
					var samplers = new Material.Sampler[pass.Samplers.Length];

					for (var s = 0; s < passShaders.Length; s++)
					{
						passShaders[s] = shaders[pass.Shaders[s]];
					}

					for (var s = 0; s < samplers.Length; s++)
					{
						var sampler = pass.Samplers[i];
						// TODO: // Resolve textures if we are supposed to
						samplers[s] = new Material.Sampler(sampler.Name, sampler.Type, sampler.Source, null);
					}

					passes[i] = new Material.Pass(passShaders, samplers);
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
