using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.ContentPipeline.Compilers.Material
{
	class Pass
	{
		public ShaderSource VertexShader { get; set; }
		public ShaderSource FragmentShader { get; set; }
		public Dictionary<string, Sampler> Samplers { get; set; }
	}

	class Sampler
	{
		public Treton.Graphics.Material.SamplerType Type { get; set; }
		public string Source { get; set; }
	}
}
