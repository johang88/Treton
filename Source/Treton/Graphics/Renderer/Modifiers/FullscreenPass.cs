using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer.Modifiers
{
	public class FullscreenPass : IModifier
	{
		private readonly Material _material;
		private readonly BatchBuffer _quadMesh;

		public FullscreenPass(Material material)
		{
			if (material == null)
				throw new ArgumentNullException("material");

			_material = material;

			_quadMesh = new BatchBuffer(material);
			_quadMesh.Begin();
			_quadMesh.AddQuad(new Vector2(-1, -1), new Vector2(2, 2), Vector2.Zero, new Vector2(1, 1));
			_quadMesh.End();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FullscreenPass()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			_quadMesh.Dispose();
		}

		public void Execute(RendererConfiguration config, RenderSystem renderSystem)
		{
			var mesh = _quadMesh.Mesh;
			var material = mesh.Materials[0];

			foreach (var materiaLayer in material.Layers)
			{
				foreach (var pass in materiaLayer.Passes)
				{					
					renderSystem.ClearShaders();
					pass.Bind(config.GlobalRenderTargets);

					foreach (var shader in pass.Shaders)
					{
						renderSystem.SetShader(shader);
					}

					renderSystem.DrawIndexed(mesh.Handle, mesh.SubMeshes[0].Offset, mesh.SubMeshes[0].Count);
				}
			}
		}
	}
}
