using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.ContentPipeline
{
	public interface ICompiler
	{
		Task<byte[]> Compile(ICompilationContext context, System.IO.Stream stream);
		uint TypeName { get; }
	}
}
