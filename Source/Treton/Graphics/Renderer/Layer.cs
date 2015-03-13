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

		internal void Initialize(Func<uint, Texture> renderTargetLocator)
		{
			RenderTargets = new Texture[RenderTargetNames.Length];
			for (var i = 0; i < RenderTargetNames.Length; i++ )
			{
				RenderTargets[i] = renderTargetLocator(RenderTargetNames[i]);
			}
		}
	}
}
