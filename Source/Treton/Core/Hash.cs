using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Core
{
	public static class Hash
	{
		private static Murmur.Murmur32 _murmur = Murmur.MurmurHash.Create32();

		public static uint HashString(string data)
		{
			var bytes = Encoding.UTF8.GetBytes(data);
			var hash = _murmur.ComputeHash(bytes);

			return BitConverter.ToUInt32(hash, 0);
		}
	}
}
