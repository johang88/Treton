using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class Mesh : IDisposable
	{
		public int Handle { get; private set; }
		public Buffer VertexBuffer { get; private set; }
		public Buffer IndexBuffer { get; private set; }
		public SubMesh[] SubMeshes { get; private set; }
		public Material[] Materials;

		public Mesh(VertexFormat vertexFormat, byte[] vertexData, byte[] indexData, SubMesh[] subMeshes, Material[] materials, bool mutable = false)
		{
			if (vertexFormat == null)
				throw new ArgumentNullException("vertexFormat");
			if (vertexData == null)
				throw new ArgumentNullException("vertexData");
			if (indexData == null)
				throw new ArgumentNullException("indexData");
			if (subMeshes == null)
				throw new ArgumentNullException("subMeshes");
			if (materials == null)
				throw new ArgumentNullException("materials");
			if (subMeshes.Length != materials.Length)
				throw new ArgumentException("subMeshes <-> materials length mismatch");

			Handle = GL.GenVertexArray();

			SubMeshes = subMeshes;
			Materials = materials;

			VertexBuffer = new Buffer(BufferTarget.ArrayBuffer, vertexFormat, mutable);
			IndexBuffer = new Buffer(BufferTarget.ElementArrayBuffer, mutable);

			VertexBuffer.SetData(vertexData);
			IndexBuffer.SetData(indexData);

			GL.BindVertexArray(Handle);
			GL.BindBuffer(IndexBuffer.Target, IndexBuffer.Handle);
			GL.BindVertexArray(0);

			GL.Ext.VertexArrayBindVertexBuffer(Handle, 0, VertexBuffer.Handle, IntPtr.Zero, vertexFormat.Size);

			for (var v = 0; v < vertexFormat.Elements.Length; v++)
			{
				var element = vertexFormat.Elements[v];
				var index = (int)element.Semantic;

				GL.Ext.EnableVertexArrayAttrib(Handle, index);
				GL.Ext.VertexArrayVertexAttribBinding(Handle, index, 0);
				GL.Ext.VertexArrayVertexAttribFormat(Handle, index, element.Count, (ExtDirectStateAccess)element.Type, false, element.Offset);
				GL.Ext.VertexArrayVertexBindingDivisor(Handle, 0, element.Divisor);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Mesh()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Handle != 0)
			{
				GL.DeleteVertexArray(Handle);
				Handle = 0;
			}

			if (VertexBuffer != null)
			{
				VertexBuffer.Dispose();
			}

			if (IndexBuffer != null)
			{
				IndexBuffer.Dispose();
			}
		}

		public void Bind()
		{
			GL.BindVertexArray(Handle);
		}

		public struct SubMesh
		{
			public int Offset;
			public int Count;

			public SubMesh(int offset, int count)
			{
				Offset = offset;
				Count = count;
			}
		}
	}
}
