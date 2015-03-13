using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core.Resources
{
	public struct ResourceId
	{
		public readonly uint Name;
		public readonly uint Type;

		public ResourceId(uint name, uint type)
		{
			Name = name;
			Type = type;
		}

		public ResourceId(string name, string type)
		{
			Name = Hash.HashString(name);
			Type = Hash.HashString(type);
		}

		public override string ToString()
		{
			return ToHex(Name) + ToHex(Type);
		}

		static string ToHex(uint x)
		{
			return x.ToString("X", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();
		}
	}
}
