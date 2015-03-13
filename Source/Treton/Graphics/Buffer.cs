using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Treton.Graphics
{
	public class Buffer : IDisposable
	{
		public int Handle { get; private set; }
		public readonly BufferTarget Target;
		public readonly bool IsMutable;
		public readonly VertexFormat VertexFormat;

		public Buffer(BufferTarget target, bool mutable)
		{
			Target = target;
			IsMutable = mutable;
			VertexFormat = null;

			Handle = GL.GenBuffer();
		}

		public Buffer(BufferTarget target, VertexFormat vertexFormat, bool mutable)
		{
			Target = target;
			IsMutable = mutable;
			VertexFormat = vertexFormat;

			Handle = GL.GenBuffer();
		}

		public void SetData<T>(T[] data)
			where T : struct
		{
			var dataLength = new IntPtr(data.Length * Marshal.SizeOf(typeof(T)));

			if (IsMutable)
			{
				GL.Ext.NamedBufferData(Handle, dataLength, data, (ExtDirectStateAccess)BufferUsageHint.StreamDraw);
			}
			else
			{
				GL.Ext.NamedBufferStorage(Handle, dataLength, data, 0);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Buffer()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Handle != 0)
			{
				GL.DeleteBuffer(Handle);
				Handle = 0;
			}
		}
	}
}
