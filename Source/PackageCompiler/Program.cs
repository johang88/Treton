using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageCompiler
{
	class Program
	{
		static void Main(string[] args)
		{
			EntryPoint(args).Wait();
		}

		static async Task EntryPoint(string[] args)
		{
			// Touch all content compilers
			Treton.ContentPipeline.Compilers.Import.Touch();

			var options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				Console.WriteLine(options.GetUsage());
				return;
			}

			var compilers = new Treton.ContentPipeline.ContentCompilers();
			Treton.ContentPipeline.ContentCompilerDiscoverer.Discover(compilers);

			var packageCompiler = new Treton.ContentPipeline.PackageCompiler(compilers, options.InputPath, options.OutputPath, options.Platforms, options.CreateBundle);

			var packages = options.Packages.Split(',');
			foreach (var package in packages)
			{
				await packageCompiler.CompilePackage(package);
			}
		}
	}
}
