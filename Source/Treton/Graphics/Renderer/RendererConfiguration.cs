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
		internal void Initialize()
		{
			// Create render targets
			foreach (var definition in GlobalRenderTargetDefinitions)
			{
				// Ehhh todo ...
			}
		}

		[Serializable]
		public class RenderTargetDefinition
		{
			public uint Name;
			public PixelFormat PixelFormat;
			public PixelInternalFormat PixelInternalFormat;
		}
	}
}
