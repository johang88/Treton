using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class Texture : IDisposable
	{
		public readonly Core.Resources.ResourceId Id;

		public int Handle { get; private set; }
		public TextureTarget TextureTarget { get; private set; }
		public PixelInternalFormat Format { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public bool IsMutable { get; private set; }

		private Texture(Core.Resources.ResourceId id)
		{
			Id = id;
		}

		/// <summary>
		/// Resize the texture, immutable textures will be recreated as their storage requirments
		/// can not be changed otherwise.
		/// </summary>
		/// <param name="width">New width</param>
		/// <param name="height">New height</param>
		public void Resize(int width, int height)
		{
			Width = width;
			Height = height;

			if (IsMutable)
			{
				// TODO ....
				GL.Ext.TextureImage2D(Handle, TextureTarget, 0, (int)Format, Width, Height, 0, PixelFormat.Red, PixelType.Byte, IntPtr.Zero);
			}
			else
			{
				// Recreate the texture so that we may change the backing storage
				GL.DeleteTexture(Handle);
				Handle = GL.GenTexture();

				GL.Ext.TextureStorage2D(Handle, (ExtDirectStateAccess)TextureTarget, 1, (ExtDirectStateAccess)Format, Width, Height);
			}
		}

		public static Texture CreateMutable(Core.Resources.ResourceId id, TextureTarget target, int width, int height, PixelInternalFormat format)
		{
			var texture = new Texture(id);
			texture.TextureTarget = target;
			texture.IsMutable = true;
			texture.Format = format;
			texture.Width = width;
			texture.Height = height;

			texture.Handle = GL.GenTexture();

			// TODO ....
			GL.Ext.TextureImage2D(texture.Handle, texture.TextureTarget, 0, (int)texture.Format, texture.Width, texture.Height, 0, PixelFormat.Red, PixelType.Byte, IntPtr.Zero);

			return texture;
		}

		public static Texture CreateImmutable(Core.Resources.ResourceId id, TextureTarget target, int width, int height, PixelInternalFormat format)
		{
			var texture = new Texture(id);
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
