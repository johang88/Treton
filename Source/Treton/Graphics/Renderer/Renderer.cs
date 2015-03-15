using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics.Renderer
{
	public class Renderer
	{
		private readonly RenderSystem _renderSystem;
		private readonly RendererConfiguration _configuration;

		private int _activeLayerConfiguration = 0;

		public Renderer(RenderSystem renderSystem, RendererConfiguration configuration)
		{
			if (renderSystem == null)
				throw new ArgumentNullException("renderSystem");
			if (configuration == null)
				throw new ArgumentNullException("configuration");

			_renderSystem = renderSystem;
			_configuration = configuration;

			SetLayerConfiguration(Core.Hash.HashString("Default"));
		}

		/// <summary>
		/// Call when the render window has been resized.
		/// This will resize all current render targets. 
		/// The new dimension are fetched from the rendersystem.
		/// </summary>
		public void Resize()
		{
			var width = _renderSystem.Width;
			var height = _renderSystem.Height;

			foreach (var renderTarget in _configuration.GlobalRenderTargets)
			{
				renderTarget.Resize(width, height);
			}
		}

		public void Render(World.RenderWorld renderWorld, Camera camera, Viewport viewport)
		{
			var layerConfiguration = _configuration.LayerConfigurations[_activeLayerConfiguration];

			int meshCount; Matrix4[] worldMatrices; Mesh[] meshes;
			renderWorld.GetMeshInstances(out meshCount, out worldMatrices, out meshes);

			// todo: make this not suck
			foreach (var layer in layerConfiguration.Layers)
			{
				_renderSystem.SetRenderTarget(layer.RenderTargets);

				GL.ClearColor(Color4.AliceBlue);
				GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

				if (layer.ResourceGenerator != null)
				{
					layer.ResourceGenerator.Execute(_configuration, _renderSystem);
				}
				else
				{
					for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
					{
						var mesh = meshes[meshIndex];
						mesh.Bind();

						for (var subMeshIndex = 0; subMeshIndex < mesh.SubMeshes.Length; subMeshIndex++)
						{
							if (!mesh.Materials[subMeshIndex].HasLayer(layer.Name))
								continue;

							var material = mesh.Materials[subMeshIndex];
							var materiaLayer = material.GetLayer(layer.Name);

							if (materiaLayer.Name != layer.Name)
								break;

							foreach (var pass in materiaLayer.Passes)
							{
								_renderSystem.ClearShaders();

								foreach (var shader in pass.Shaders)
								{
									_renderSystem.SetShader(shader);
								}

								_renderSystem.Draw(mesh.Handle, mesh.SubMeshes[subMeshIndex].Offset, mesh.SubMeshes[subMeshIndex].Count);
							}
						}
					}
				}
			}
		}

		public void SetLayerConfiguration(uint layerName)
		{
			_activeLayerConfiguration = Array.FindIndex(_configuration.LayerConfigurations, l => l.Name == layerName);
		}
	}
}
