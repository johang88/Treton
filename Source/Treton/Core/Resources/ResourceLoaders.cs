using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core.Resources
{
	public interface IResourceLoaders
	{
		void Add(uint type, ILoader loader);
		void Remove(uint type);
		ILoader Get(uint type);
	}

	public class ResourceLoaders : IResourceLoaders
	{
		private readonly Dictionary<uint, ILoader> _loaders = new Dictionary<uint, ILoader>();

		public void Add(uint type, ILoader loader)
		{
			_loaders.Add(type, loader);
		}

		public void Remove(uint type)
		{
			_loaders.Remove(type);
		}

		public ILoader Get(uint type)
		{
			return _loaders[type];
		}
	}
}
