﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace Treton.ContentPipeline.Compilers.RenderConfig
{
	public class RenderConfigCompiler : ICompiler
	{
		public RenderConfigCompiler()
		{
			TypeName = Core.Hash.HashString("renderconfig");
		}

		public async Task<byte[]> Compile(ICompilationContext context, System.IO.Stream stream)
		{
			RenderConfigData sourceRenderConfig;
			using (var reader = new StreamReader(stream))
			{
				var source = await reader.ReadToEndAsync();
				sourceRenderConfig = JsonConvert.DeserializeObject<RenderConfigData>(source);
			}

			var renderConfig = new Graphics.Renderer.RendererConfiguration();
			
			renderConfig.GlobalRenderTargetDefinitions = sourceRenderConfig.RenderTargets.Select(rt =>
				new Graphics.Renderer.RendererConfiguration.RenderTargetDefinition
				{
					Name = Core.Hash.HashString(rt.Name),
					Format = rt.Format
				}
			).ToArray();

			renderConfig.LayerConfigurations = sourceRenderConfig.LayerConfigurations.Select(lc =>
				new Graphics.Renderer.LayerConfiguration
				{
					Name = Core.Hash.HashString(lc.Key),
					Layers = lc.Value.Select(l =>
						new Graphics.Renderer.Layer
						{
							Name = Core.Hash.HashString(l.Name),
							RenderTargetNames = l.RenderTargets.Select(Core.Hash.HashString).ToArray(),
							ResourceGeneratorName = string.IsNullOrWhiteSpace(l.ResourceGenerator) ? (uint?)null : (uint?)Core.Hash.HashString(l.ResourceGenerator)
						}
					).ToArray()
				}
			).ToArray();

			renderConfig.ResourceGenerators = sourceRenderConfig.ResourceGenerators.Select(rc =>
				new Graphics.Renderer.ResourceGenerator
				{
					Name =Core.Hash.HashString(rc.Key),
					ModifierDescriptions = rc.Value.Select(m =>
						new Graphics.Renderer.ResourceGenerator.ModifierDefinition
						{
							Type = Core.Hash.HashString(m.Type),
							Material = context.Queue(m.Material)
						}
					).ToArray()
				}
			).ToArray();

			var formatter = new BinaryFormatter();

			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms))
			{
				formatter.Serialize(ms, renderConfig);
				return ms.GetBuffer();
			}
		}

		public uint TypeName { get; private set; }
	}

	class RenderConfigData
	{
		public RenderTarget[] RenderTargets { get; set; }
		public Dictionary<string, LayerConfiguration[]> LayerConfigurations { get; set; }
		public Dictionary<string, ResourceGeneratorModifier[]> ResourceGenerators { get; set; }

		public class RenderTarget
		{
			public string Name { get; set; }
			public PixelInternalFormat Format { get; set; }
		}

		public class LayerConfiguration
		{
			public string Name { get; set; }
			public string[] RenderTargets { get; set; }
			public string ResourceGenerator { get; set; }
		}

		public class ResourceGeneratorModifier
		{
			public string Type { get; set; }
			public string Material { get; set; }
		}
	}
}
