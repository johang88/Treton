﻿using OpenTK;
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

		public void Render(World.RenderWorld renderWorld, Camera camera, Viewport viewport)
		{
			var layerConfiguration = _configuration.LayerConfigurations[_activeLayerConfiguration];

			int meshCount; Matrix4[] worldMatrices; Mesh[] meshes;
			renderWorld.GetMeshInstnaces(out meshCount, out worldMatrices, out meshes);

			// todo: make this not suck
			foreach (var layer in layerConfiguration.Layers)
			{
				for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
				{
					var mesh = meshes[meshIndex];
					mesh.Bind();
					
					for (var subMeshIndex = 0; subMeshIndex < mesh.SubMeshes.Length; subMeshIndex++)
					{
						var material = mesh.Materials[subMeshIndex];

						foreach (var materiaLayer in material.Layers)
						{
							if (materiaLayer.Name != layer.Name)
								break;

							foreach (var pass in materiaLayer.Passes)
							{
								_renderSystem.ClearShaders();

								foreach (var shader in pass.Shaders)
								{
									_renderSystem.SetShader(shader);
								}

								_renderSystem.Draw(mesh.SubMeshes[subMeshIndex].Offset, mesh.SubMeshes[subMeshIndex].Count);
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