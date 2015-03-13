using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treton.Core.Resources;

namespace Treton.ContentPipeline
{
	public interface ICompilationContext
	{
		ResourceId Queue(string path);
		Stream OpenDependency(string path);
	}

	public class CompilationContext : ICompilationContext
	{
		private Queue<string> _toProcess = new Queue<string>();
		private HashSet<string> _content = new HashSet<string>();
		private HashSet<string> _dependencies = new HashSet<string>();
		
		private readonly ContentCompilers _compilers;
		private readonly string _basePath;

		public CompilationContext(ContentCompilers compilers, string basePath)
		{
			if (compilers == null)
				throw new ArgumentNullException("compilers");
			if (string.IsNullOrEmpty(basePath))
				throw new ArgumentNullException("basePath");

			_compilers = compilers;
			_basePath = basePath;
		}

		public ResourceId Queue(string path)
		{
			if (_content.Add(path))
			{
				_toProcess.Enqueue(path);
			}

			_dependencies.Add(path);

			return CreateResourceId(path);
		}

		internal bool HasNext()
		{
			return _toProcess.Count > 0;
		}

		internal string Next()
		{
			_dependencies.Clear();

			return _toProcess.Dequeue();
		}

		public System.IO.Stream OpenFile(string path)
		{
			return System.IO.File.OpenRead(System.IO.Path.Combine(_basePath, path));
		}

		public Stream OpenDependency(string path)
		{
			return OpenFile(path);
		}

		public ResourceId CreateResourceId(string path)
		{
			var extension = Path.GetExtension(path).Substring(1).ToLowerInvariant();
			var name = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)).Replace('\\', '/');

			return new Core.Resources.ResourceId(Core.Hash.HashString(name), Core.Hash.HashString(extension));
		}

		internal string[] Dependencies { get { return _dependencies.ToArray(); } }
	}
}
