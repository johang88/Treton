using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class Texture
	{
		public readonly int Handle;
		public readonly TextureTarget TextureTarget;
		public readonly PixelFormat PixelFormat;
		public readonly PixelInternalFormat PixelInternalFormat;
	}
}
