using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MaterialData = Treton.Graphics.ResourceLoaders.MaterialData;

namespace Treton.ContentPipeline.Compilers.Material
{
	public class ShaderCompiler : ICompiler
	{
		public ShaderCompiler()
		{
			TypeName = Core.Hash.HashString("material");
		}

		public async Task<byte[]> Compile(ICompilationContext context, System.IO.Stream stream)
		{
			Dictionary<string, List<Pass>> material;
			using (var reader = new StreamReader(stream))
			{
				var source = await reader.ReadToEndAsync();
				material = JsonConvert.DeserializeObject<Dictionary<string, List<Pass>>>(source);
			}

			var output = await CompileMaterial(material, context);

			var formatter = new BinaryFormatter();

			using (var ms = new MemoryStream())
			using (var writer = new BinaryWriter(ms))
			{
				formatter.Serialize(ms, output);
				return ms.GetBuffer();
			}
		}

		private async Task<MaterialData.Material> CompileMaterial(Dictionary<string, List<Pass>> material, ICompilationContext context)
		{
			var shaders = new List<MaterialData.Shader>();
			var layers = new List<MaterialData.Layer>();

			foreach (var layer in material)
			{
				var passes = new List<MaterialData.Pass>();

				foreach (var pass in layer.Value)
				{
					var passShaders = new List<int>();

					await AddShader(context, OpenTK.Graphics.OpenGL.ShaderType.VertexShader, pass.VertexShader, shaders, passShaders);
					await AddShader(context, OpenTK.Graphics.OpenGL.ShaderType.FragmentShader, pass.FragmentShader, shaders, passShaders);

					passes.Add(new MaterialData.Pass
					{
						Shaders = passShaders.ToArray()
					});
				}

				layers.Add(new MaterialData.Layer
				{
					Name = Core.Hash.HashString(layer.Key),
					Passes = passes.ToArray()
				});
			}

			return new MaterialData.Material
			{
				Shaders = shaders.ToArray(),
				Layers = layers.ToArray()
			};
		}

		private async Task AddShader(ICompilationContext context, OpenTK.Graphics.OpenGL.ShaderType type, ShaderSource shader, List<MaterialData.Shader> shaders, List<int> pass)
		{
			if (shader == null)
				return;

			// TODO: Skip identical shaders and stuff

			string source;
			using (var stream = context.OpenDependency(shader.Source))
			using (var reader = new StreamReader(stream))
			{
				source = await reader.ReadToEndAsync();
			}

			// TODO: fix define stuff
			var preprocessor = new Shaders.Preprocessor(context);
			source = await preprocessor.Process(source);

			var index = shaders.Count;
			shaders.Add(new MaterialData.Shader
			{
				Type = type,
				Source = source
			});

			pass.Add(index);
		}

		public uint TypeName { get; private set; }
	}
}
