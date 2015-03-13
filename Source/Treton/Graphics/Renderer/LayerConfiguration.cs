using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer
{
	[Serializable]
	public class LayerConfiguration
	{
		public uint Name;
		public Layer[] Layers;
	}
}
