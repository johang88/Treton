using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Treton.ContentPipeline
{
	public class ContentCompilerDiscoverer
	{
		public static void Discover(ContentCompilers compilers)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetExportedTypes().Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(ICompiler))).ToList();
				foreach (var type in types)
				{
					var compiler = (ICompiler)Activator.CreateInstance(type);
					compilers.Add(compiler);
				}
			}
		}
	}
}
