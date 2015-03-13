using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Treton.Core.IO
{
	public interface IFileSystem
	{
		string DataDirectory { get; }
		string WriteDirectory { get; }

		string GetDataPath(string path);

		Stream OpenRead(string path, bool preferWriteDirectory = true);
		Stream OpenWrite(string path);
	}

	public class FileSystem : IFileSystem
	{
		public FileSystem(string dataDirectory, string writeDirectory)
		{
			if (dataDirectory == null) 
				throw new ArgumentNullException("dataDirectory");
			if (writeDirectory == null) 
				throw new ArgumentNullException("writeDirectory");

			DataDirectory = dataDirectory;
			WriteDirectory = writeDirectory;
		}

		public string DataDirectory { get; private set; }
		public string WriteDirectory { get; private set; }

		private IEnumerable<string> GetPaths(string path, bool dataDirectory, bool writeDirectory)
		{
			if (writeDirectory)
				yield return Path.Combine(WriteDirectory, path);

			if (dataDirectory)
				yield return Path.Combine(DataDirectory, path);
		}

		private string GetPath(string path, bool preferWriteDirectory = true)
		{
			var paths = GetPaths(path, true, preferWriteDirectory);
			path = paths.FirstOrDefault(File.Exists);

			if (path == null) 
				throw new FileNotFoundException("File not found", path);

			return path;
		}

		public string GetDataPath(string path)
		{
			return Path.Combine(DataDirectory, path);
		}

		public Stream OpenRead(string path, bool preferWriteDirectory = true)
		{
			return File.OpenRead(GetPath(path, true));
		}

		public Stream OpenWrite(string path)
		{
			var paths = GetPaths(path, false, true).First();
			return File.OpenWrite(path);
		}

		public static FileSystem CreateDefault(string dataDirectory)
		{
			var entryAssembly = Assembly.GetEntryAssembly();
			var assemblyName = entryAssembly.GetName().Name;

			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var writeDirectory = Path.Combine(appDataPath, assemblyName);

			if (!Directory.Exists(writeDirectory))
			{
				Directory.CreateDirectory(writeDirectory);
			}

			return new FileSystem(dataDirectory, writeDirectory);
		}
	}
}
