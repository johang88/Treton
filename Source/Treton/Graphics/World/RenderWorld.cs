using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Treton.Graphics.World
{
	public class RenderWorld
	{
		private const int MaxMeshInstnaces = 1024;

		private int[] _indexMap = new int[MaxMeshInstnaces];
		private Matrix4[] _worldMatrices= new Matrix4[MaxMeshInstnaces];
		private Mesh[] _meshes = new Mesh[MaxMeshInstnaces];
		private int _meshCount = 0;
		private int _lastId = 0;

		public RenderWorld()
		{

		}

		public int AddMesh(Mesh mesh)
		{
			if (mesh == null)
				throw new ArgumentNullException("mesh");

			var index = _meshCount++;
			var id = _lastId++;

			_indexMap[id] = index;

			_meshes[index] = mesh;
			_worldMatrices[index] = Matrix4.Identity;

			return id;
		}

		public void RemoveMesh(int id)
		{
			// todo
		}

		public void SetWorldMatrix(int id, Matrix4 world)
		{
			var index = _indexMap[id];
			_worldMatrices[index] = world;
		}

		internal void GetMeshInstnaces(out int count, out Matrix4[] worldMatrices, out Mesh[] meshes)
		{
			count = _meshCount;
			worldMatrices = _worldMatrices;
			meshes = _meshes;
		}
	}
}
