using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Framework
{
	public class EngineConfiguration
	{
		public string Title { get; set; }
		public string DataDirectory { get; set; }
		public string CorePackage { get; set; }
		public RendererConfiguration Renderer { get; set; }

		public static EngineConfiguration Load(string path = "Treton.config.v")
		{
			var data = System.IO.File.ReadAllText(path);
			return JsonConvert.DeserializeObject<EngineConfiguration>(data);
		}

		public class RendererConfiguration
		{
			public int Width { get; set; }
			public int Height { get; set; }
			public FullscreenMode FullscreenMode { get; set; }
			public DisplayIndex Display { get; set; }
			public string RenderConfig { get; set; }
		}
	}

	public enum FullscreenMode
	{
		Windowed,
		Borderless,
		Fullscreen
	}
}
