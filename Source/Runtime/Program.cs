using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treton.Framework;

namespace Runtime
{
	class Application : IApplication
	{
		private readonly Engine _engine;

		public Application(Engine engine)
		{
			_engine = engine;
		}

		public void Initialize()
		{
		}

		public void Update(double dt)
		{
		}

		public void Shutdown()
		{
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			var configuration = EngineConfiguration.Load();
			using (var engine = new Engine(configuration))
			{
				engine.Run(new Application(engine));
			}
		}
	}
}
