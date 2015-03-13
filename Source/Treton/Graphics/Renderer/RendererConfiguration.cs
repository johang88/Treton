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

		/// <summary>
		/// Initialize all internal resources
		/// </summary>
		internal void Initialize(RenderSystem renderSystem)
		{
			// Setup render targets
			GlobalRenderTargets = new Texture[GlobalRenderTargetDefinitions.Length];
			for (var i = 0; i < GlobalRenderTargetDefinitions.Length; i++)
			{
				GlobalRenderTargets[i] = Texture.CreateImmutable(TextureTarget.Texture2D, renderSystem.Width, renderSystem.Height, GlobalRenderTargetDefinitions[i].Format);
			}
		}

		internal void Teardown()
		{
			for (var i = 0; i < GlobalRenderTargets.Length; i++)
			{
				GlobalRenderTargets[i].Dispose();
				GlobalRenderTargets[i] = null;
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
