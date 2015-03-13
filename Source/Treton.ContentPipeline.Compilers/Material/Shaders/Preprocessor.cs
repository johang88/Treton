using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Treton.ContentPipeline.Compilers.Material.Shaders
{
	class Preprocessor
	{
		private static readonly Regex _includeRegex = new Regex(@"^(#include\s""[ \t\w /\.]+"")", RegexOptions.Multiline);
		private readonly List<string> _includes = new List<string>();

		private readonly ICompilationContext _context;

		public Preprocessor(ICompilationContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			_context = context;
		}

		public async Task<string> Process(string source)
		{
			_includes.Clear();

			source = await ResolveIncludes(source);

			return source;
		}

		private async Task<string> ResolveIncludes(string source)
		{
			var sb = new StringBuilder();

			var parts = _includeRegex.Split(source);
			foreach (var part in parts)
			{
				if (!part.StartsWith("#include"))
				{
					sb.Append(part);
					continue;
				}

				var path = part.Substring(part.IndexOf('"') + 1);
				path = path.Substring(0, path.Length - 1);

				if (_includes.Contains(path))
					continue;

				_includes.Add(path);

				var includeSource = await ReadSource(path);
				includeSource = await ResolveIncludes(includeSource);

				sb.Append(includeSource);
			}

			return sb.ToString();
		}

		private async Task<string> ReadSource(string path)
		{
			using (var stream = _context.OpenDependency(path))
			using (var reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}
