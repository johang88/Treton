using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer
{
	[Serializable]
	public class RendererConfiguration
	{
		public LayerConfiguration[] LayerConfigurations;
		[NonSerialized]
		public Texture[] GlobalRenderTargets;
		public RenderTargetDefinition[] GlobalRenderTargetDefinitions;
		public ResourceGenerator[] ResourceGenerators;

		/// <summary>
		/// Initialize all internal resources
		/// </summary>
		internal void Initialize(Core.Resources.IResourceManager resourceManager, RenderSystem renderSystem)
		{
			// Setup render targets
			GlobalRenderTargets = new Texture[GlobalRenderTargetDefinitions.Length];
			for (var i = 0; i < GlobalRenderTargetDefinitions.Length; i++)
			{
				GlobalRenderTargets[i] = Texture.CreateImmutable(TextureTarget.Texture2D, renderSystem.Width, renderSystem.Height, GlobalRenderTargetDefinitions[i].Format);
			}

			// Init resource generators
			foreach (var genereator in ResourceGenerators)
			{
				genereator.Initialize(resourceManager);
			}

			// Init layers
			foreach (var layerConfig in LayerConfigurations)
			{
				foreach (var layer in layerConfig.Layers)
				{
					layer.Initialize(GetRenderTarget, GetResourceGenerator);
				}
			}
		}

		private Texture GetRenderTarget(uint name)
		{
			for (var i = 0; i < GlobalRenderTargetDefinitions.Length; i++)
			{
				if (GlobalRenderTargetDefinitions[i].Name == name)
				{
					return GlobalRenderTargets[i];
				}
			}

			return null;
		}

		private ResourceGenerator GetResourceGenerator(uint name)
		{
			for (var i = 0; i < ResourceGenerators.Length; i++)
			{
				if (ResourceGenerators[i].Name == name)
				{
					return ResourceGenerators[i];
				}
			}

			return null;
		}

		internal void Teardown()
		{
			for (var i = 0; i < GlobalRenderTargets.Length; i++)
			{
				GlobalRenderTargets[i].Dispose();
				GlobalRenderTargets[i] = null;
			}

			// Init resource generators
			foreach (var genereator in ResourceGenerators)
			{
				genereator.Dispose();
			}
		}

		[Serializable]
		public class RenderTargetDefinition
		{
			public uint Name;
			public PixelInternalFormat Format;
		}
	}
}
