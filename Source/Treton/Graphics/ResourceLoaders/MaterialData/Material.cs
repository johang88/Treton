using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.ResourceLoaders.MaterialData
{
	[Serializable]
	public class Material
	{
		public Shader[] Shaders;
		public Layer[] Layers;
	}
}
