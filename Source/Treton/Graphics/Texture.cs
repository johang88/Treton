﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class Texture : IDisposable
	{
		public int Handle { get; private set; }
		public TextureTarget TextureTarget { get; private set; }
		public PixelInternalFormat Format { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public bool IsMutable { get; private set; }

		private Texture()
		{
		}

		public static Texture CreateImmutable(TextureTarget target, int width, int height, PixelInternalFormat format)
		{
			var texture = new Texture();
			texture.TextureTarget = target;
			texture.IsMutable = false;
			texture.Format = format;
			texture.Width = width;
			texture.Height = height;

			texture.Handle = GL.GenTexture();

			GL.Ext.TextureStorage2D(texture.Handle, (ExtDirectStateAccess)texture.TextureTarget, 1, (ExtDirectStateAccess)texture.Format, texture.Width, texture.Height);

			return texture;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Texture()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Handle != 0)
			{
				GL.DeleteTexture(Handle);
				Handle = 0;
			}
		}
	}
}
