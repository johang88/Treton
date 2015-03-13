using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.ContentPipeline
{
	public class ContentCompilers
	{
		private readonly Dictionary<uint, ICompiler> _compilers = new Dictionary<uint, ICompiler>();

		public void Add(ICompiler compiler)
		{
			_compilers.Add(compiler.TypeName, compiler);
		}

		public async Task<byte[]> Compile(ICompilationContext context, uint type, System.IO.Stream stream)
		{
			var compiler = _compilers[type];
			return await compiler.Compile(context, stream);
		}
	}
}
