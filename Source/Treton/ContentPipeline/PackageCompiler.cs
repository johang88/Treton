using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Treton.Core.Resources;

namespace Treton.ContentPipeline
{
	public class PackageCompiler
	{
		private readonly ContentCompilers _compilers;
		private readonly string _inputPath;
		private readonly string _outputPath;
		private readonly string _platform;
		private readonly bool _createBundles;

		public PackageCompiler(ContentCompilers compilers, string inputPath, string outputPath, string platform, bool createBundles)
		{
			if (compilers == null)
				throw new ArgumentNullException("compilers");
			if (string.IsNullOrWhiteSpace(inputPath))
				throw new ArgumentNullException("inputPath");
			if (string.IsNullOrWhiteSpace(outputPath))
				throw new ArgumentNullException("outputPath");
			if (string.IsNullOrWhiteSpace("platform"))
				throw new ArgumentNullException("platform");

			_compilers = compilers;
			_inputPath = inputPath;
			_outputPath = outputPath;
			_platform = platform;
			_createBundles = createBundles;
		}

		public async Task CompilePackage(string packageName)
		{
			var packagePath = Path.Combine(_inputPath, packageName + ".package");

			if (!Directory.Exists(_outputPath))
			{
				Directory.CreateDirectory(_outputPath);
			}

			var packageEntries = DeserializePackage(packagePath);

			var context = new CompilationContext(_compilers, _inputPath);
			packageEntries.ForEach(p => context.Queue(p));

			var resources = new List<ResourceId>();
			var bundle = new List<byte[]>();

			// Compile all resources
			while (context.HasNext())
			{
				var entry = context.Next();
				var entryPath = Path.Combine(_inputPath, entry);
				var resourceId = context.CreateResourceId(entry);
				var outputPath = Path.Combine(_outputPath, resourceId.ToString());
				var extension = Path.GetExtension(entryPath);

				var platformSpecificPath = Path.ChangeExtension(entryPath, null) + "." + _platform + "." + extension;
				if (File.Exists(platformSpecificPath))
				{
					entryPath = platformSpecificPath;
				}

				byte[] compiledData;

				using (var inputStream = File.OpenRead(entryPath))
				{
					compiledData = await _compilers.Compile(context, resourceId.Type, inputStream);
				}

				if (_createBundles)
				{
					bundle.Add(compiledData);
				}
				else
				{
					using (var outputStream = File.OpenWrite(outputPath))
					{
						await outputStream.WriteAsync(compiledData, 0, compiledData.Length);
					}
				}

				resources.Add(resourceId);
			}

			// Make sure that resources are loaded in reverse dependency order,
			// this makes the runtime implementation simpler as the dependency lookup will never fail
			resources.Reverse();
			bundle.Reverse();

			using (var outputStream = File.OpenWrite(Path.Combine(_outputPath, packageName + ".package")))
			using (var writer = new BinaryWriter(outputStream))
			{
				writer.Write('T'); writer.Write('P'); writer.Write('A'); writer.Write('K');
				writer.Write(_createBundles);
				writer.Write(resources.Count);

				// Index list
				foreach (var resource in resources)
				{
					writer.Write(resource.Name);
					writer.Write(resource.Type);
				}

				// Offset / Size list
				var offset = writer.BaseStream.Position + bundle.Count * (sizeof(int) + sizeof(long));
				foreach (var resource in bundle)
				{
					writer.Write(resource.Length);
					writer.Write(offset);

					offset += resource.Length;
				}

				// Resource data
				foreach (var resource in bundle)
				{
					writer.Write(resource);
				}
			}
		}

		private List<string> DeserializePackage(string path)
		{
			var content = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<List<string>>(content);
		}
	}
}
