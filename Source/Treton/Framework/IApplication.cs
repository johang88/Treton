using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Framework
{
	public interface IApplication
	{
		void Initialize();
		void Update(double dt);
		void Shutdown();
	}
}
