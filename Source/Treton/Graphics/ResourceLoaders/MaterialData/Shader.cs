using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.ResourceLoaders.MaterialData
{
	[Serializable]
	public class Shader
	{
		public OpenTK.Graphics.OpenGL.ShaderType Type;
		public string Source;
	}
}
