using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treton.Core.Resources;
using OpenTK.Graphics.OpenGL;

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
			public readonly Sampler[] Samplers;

			public Pass(Shader[] shaders, Sampler[] samplers)
			{
				Shaders = shaders;
				Samplers = samplers;
			}

			public void Bind(Texture[] scopeTextures)
			{
				int bindIndex = 0;

				foreach (var sampler in Samplers)
				{
					foreach (var shader in Shaders)
					{
						if (!shader.HasUniform(sampler.Name))
							continue;

						// Find the texture
						foreach (var texture in scopeTextures)
						{
							if (texture.Id.Name == sampler.Source.Name)
							{
								GL.ActiveTexture(TextureUnit.Texture0 + bindIndex);
								GL.BindTexture(texture.TextureTarget, texture.Handle);
								GL.ProgramUniform1(shader.Handle, shader.GetUniformLocation(sampler.Name), bindIndex);

								bindIndex++;
							}
						}
					}
				}
			}
		}

		public class Sampler
		{
			public readonly uint Name;
			public readonly SamplerType SamplerType;
			public readonly ResourceId Source;
			public readonly Texture SourceTexture; // Set to null for rt sources (resolved at runtime)

			public Sampler(uint name, SamplerType type, ResourceId source, Texture sourceTexture)
			{
				Name = name;
				SamplerType = type;
				Source = source;
				SourceTexture = sourceTexture;
			}
		}

		public enum SamplerType
		{
			RenderTarget,
			Texture
		}
	}
}
