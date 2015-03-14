using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.ResourceLoaders.MaterialData
{
	[Serializable]
	public class Sampler
	{
		public uint Name;
		public Treton.Graphics.Material.SamplerType Type;
		public Core.Resources.ResourceId Source;
	}
}
