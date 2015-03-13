using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Treton.Graphics
{
	public class BatchBuffer : IDisposable
	{
		private bool _isDisposed = false;
		private float[] _vertexData;
		private int[] _indexData;
		private int _indexCount;
		private int _dataCount;

		public readonly Mesh Mesh;
		private Mesh.SubMesh _subMesh;

		private readonly VertexFormat _vertexFormat;

		public BatchBuffer(Material material, VertexFormat vertexFormat = null, int initialTriangleCount = 128)
		{
			if (initialTriangleCount <= 0)
				throw new ArgumentException("invalid initialTriangleCount");

			var dataCount = initialTriangleCount * (3 + 3 + 3 + 2); // vec3 pos, vec3 normal, vec3 tangent, vec2 texcoord
			_vertexData = new float[dataCount];
			_indexData = new int[initialTriangleCount];

			if (vertexFormat != null)
			{
				_vertexFormat = vertexFormat;
			}
			else
			{
				_vertexFormat = new VertexFormat(new VertexFormatElement[]
				{
					new VertexFormatElement(VertexFormatSemantic.Position, VertexPointerType.Float, 3, 0),
					new VertexFormatElement(VertexFormatSemantic.TexCoord, VertexPointerType.Float, 2, sizeof(float) * 3),
				});
			}


			_subMesh = new Graphics.Mesh.SubMesh
			{
				Offset = 0,
				Count = 0
			};

			Mesh = new Mesh(_vertexFormat, new byte[0], new byte[0], new Mesh.SubMesh[] { _subMesh }, new Material[] { material });
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!isDisposing || _isDisposed)
				return;

			Mesh.Dispose();

			_isDisposed = true;
		}

		private void CheckSize(int requiredComponents)
		{
			var requiredSize = _dataCount + requiredComponents;
			if (_vertexData.Length <= requiredSize)
			{
				Array.Resize(ref _vertexData, _vertexData.Length * 2);
			}
		}

		public void Begin()
		{
			_indexCount = 0;
			_dataCount = 0;
		}

		public void End()
		{
			Mesh.VertexBuffer.SetData(_vertexData);
			Mesh.IndexBuffer.SetData(_indexData);
			_subMesh.Count = _indexCount;
		}

		public void AddVector2(float x, float y)
		{
			CheckSize(2);

			_vertexData[_dataCount++] = x;
			_vertexData[_dataCount++] = y;
		}

		public void AddVector2(ref Vector2 v)
		{
			CheckSize(3);

			_vertexData[_dataCount++] = v.X;
			_vertexData[_dataCount++] = v.Y;
		}

		public void AddVector3(float x, float y, float z)
		{
			CheckSize(3);

			_vertexData[_dataCount++] = x;
			_vertexData[_dataCount++] = y;
			_vertexData[_dataCount++] = z;
		}

		public void AddVector3(ref Vector3 v)
		{
			CheckSize(3);

			_vertexData[_dataCount++] = v.X;
			_vertexData[_dataCount++] = v.Y;
			_vertexData[_dataCount++] = v.Z;
		}

		public void AddVector4(float x, float y, float z, float w)
		{
			CheckSize(4);

			_vertexData[_dataCount++] = x;
			_vertexData[_dataCount++] = y;
			_vertexData[_dataCount++] = z;
			_vertexData[_dataCount++] = w;
		}

		public void AddVector4(ref Vector4 v)
		{
			CheckSize(4);

			_vertexData[_dataCount++] = v.X;
			_vertexData[_dataCount++] = v.Y;
			_vertexData[_dataCount++] = v.Z;
			_vertexData[_dataCount++] = v.W;
		}

		public void AddTriangle(int v1, int v2, int v3)
		{
			var requiredIndexBufferSize = _indexCount + 3;
			if (_indexData.Length <= requiredIndexBufferSize)
			{
				Array.Resize(ref _indexData, _indexData.Length * 2);
			}

			_indexData[_indexCount++] = v1;
			_indexData[_indexCount++] = v2;
			_indexData[_indexCount++] = v3;
		}

		/// <summary>
		/// Utility method to add a single 2d quad (2 triangles) to the buffer.
		/// Indexes will be setup automatically so there is no need to call AddTriangle manually.
		/// </summary>
		/// <param name="position">Position of the quad</param>
		/// <param name="size">Size of the quad, relative to the position</param>
		/// <param name="uvPositon">UV Position of the quad</param>
		/// <param name="uvSize">UV Size of the quad, relative to the UV position</param>
		public void AddQuad(Vector2 position, Vector2 size, Vector2 uvPositon, Vector2 uvSize)
		{
			var firstIndex = _dataCount / (_vertexFormat.Size / sizeof(float));

			AddVector3(position.X, position.Y, 0);
			AddVector2(ref uvPositon);

			AddVector3(position.X, position.Y + size.Y, 0);
			AddVector2(uvPositon.X, uvPositon.Y + uvSize.Y);

			AddVector3(position.X + size.X, position.Y + size.Y, 0);
			AddVector2(uvPositon.X + uvSize.X, uvPositon.Y + uvSize.Y);

			AddVector3(position.X + size.X, position.Y, 0);
			AddVector2(uvPositon.X + uvSize.X, uvPositon.Y);

			AddTriangle(firstIndex, firstIndex + 2, firstIndex + 1);
			AddTriangle(firstIndex, firstIndex + 3, firstIndex + 2);
		}

		public void AddQuadInverseUV(Vector2 position, Vector2 size, Vector2 uvPositon, Vector2 uvSize, Vector4 color)
		{
			var firstIndex = _dataCount / (_vertexFormat.Size / sizeof(float));

			AddVector3(position.X, position.Y, 0);
			AddVector2(uvPositon.X, uvPositon.Y + uvSize.Y);
			AddVector4(ref color);

			AddVector3(position.X, position.Y + size.Y, 0);
			AddVector2(ref uvPositon);
			AddVector4(ref color);

			AddVector3(position.X + size.X, position.Y + size.Y, 0);
			AddVector2(uvPositon.X + uvSize.X, uvPositon.Y);
			AddVector4(ref color);

			AddVector3(position.X + size.X, position.Y, 0);
			AddVector2(uvPositon.X + uvSize.X, uvPositon.Y + uvSize.Y);
			AddVector4(ref color);

			AddTriangle(firstIndex, firstIndex + 2, firstIndex + 1);
			AddTriangle(firstIndex, firstIndex + 3, firstIndex + 2);
		}
	}
}
