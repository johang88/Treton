using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer
{
	[Serializable]
	public class Layer
	{
		public uint Name;
		[NonSerialized]
		public Texture[] RenderTargets;
		public uint[] RenderTargetNames;
		public uint? ResourceGeneratorName;
		public ResourceGenerator ResourceGenerator;

		internal void Initialize(Func<uint, Texture> renderTargetLocator, Func<uint, ResourceGenerator> resourceGeneratorLocator)
		{
			RenderTargets = new Texture[RenderTargetNames.Length];
			for (var i = 0; i < RenderTargetNames.Length; i++ )
			{
				RenderTargets[i] = renderTargetLocator(RenderTargetNames[i]);
			}

			if (ResourceGeneratorName != null)
			{
				ResourceGenerator = resourceGeneratorLocator(ResourceGeneratorName.Value);
			}
		}
	}
}
