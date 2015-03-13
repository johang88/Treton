using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics
{
	public class Material : IDisposable
	{
		private readonly Shader[] Shaders;
		public readonly Layer[] Layers;

		public Material(Shader[] shaders, Layer[] layers)
		{
			if (shaders == null)
				throw new ArgumentNullException("shaders");
			if (layers == null)
				throw new ArgumentNullException("layers");

			Shaders = shaders;
			Layers = layers;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Material()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			foreach (var shader in Shaders)
			{
				shader.Dispose();
			}
		}

		public class Layer
		{
			public readonly uint Name;
			public readonly Pass[] Passes;

			public Layer(uint name, Pass[] passes)
			{
				Name = name;
				Passes = passes;
			}
		}

		public class Pass
		{
			public readonly Shader[] Shaders;

			public Pass(Shader[] shaders)
			{
				Shaders = shaders;
			}
		}
	}
}
