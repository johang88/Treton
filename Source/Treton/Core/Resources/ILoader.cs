using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core.Resources
{
	public interface ILoader
	{
		Task<object> Load(ResourceId id, System.IO.Stream stream);
		Task Unload(object resource);
	}
}
