using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer.Modifiers
{
	public interface IModifier : IDisposable
	{
		void Execute(RendererConfiguration config, RenderSystem renderSystem);
	}
}
