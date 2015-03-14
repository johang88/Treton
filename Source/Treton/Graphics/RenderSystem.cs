using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class RenderSystem : IDisposable
	{
		private readonly int _pipelineHandle;

		private readonly int _frameBufferHandle;
		private int _numberOfActiveRenderTargets = 0;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public RenderSystem(int width, int height)
		{
			Width = width;
			Height = height;

			_pipelineHandle = GL.GenProgramPipeline();
			GL.BindProgramPipeline(_pipelineHandle);

			_frameBufferHandle = GL.GenFramebuffer();

			GL.Viewport(0, 0, Width, Height);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~RenderSystem()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			GL.DeleteFramebuffer(_frameBufferHandle);
			GL.DeleteProgramPipeline(_pipelineHandle);
		}

		public void Resize(int width, int height)
		{
			if (width <= 0)
				throw new ArgumentException("width <= 0");
			if (height <= 0)
				throw new ArgumentException("height <= 0");

			Width = width;
			Height = height;

			GL.Viewport(0, 0, Width, Height);
		}

		public void SetShader(Shader shader)
		{
			ProgramStageMask mask = ProgramStageMask.VertexShaderBit;
			switch (shader.Type)
			{
				case ShaderType.VertexShader:
					mask = ProgramStageMask.VertexShaderBit;
					break;
				case ShaderType.TessControlShader:
					mask = ProgramStageMask.TessControlShaderBit;
					break;
				case ShaderType.TessEvaluationShader:
					mask = ProgramStageMask.TessEvaluationShaderBit;
					break;
				case ShaderType.GeometryShader:
					mask = ProgramStageMask.GeometryShaderBit;
					break;
				case ShaderType.FragmentShader:
					mask = ProgramStageMask.FragmentShaderBit;
					break;
				case ShaderType.ComputeShader:
					mask = ProgramStageMask.ComputeShaderBit;
					break;
				default:
					break;
			}

			SetShader(mask, shader.Handle);
		}

		public void SetShader(ProgramStageMask mask, int shaderHandle)
		{
			GL.UseProgramStages(_pipelineHandle, mask, shaderHandle);
		}

		public void ClearShaders()
		{
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.VertexShaderBit, 0);
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.TessControlShaderBit, 0);
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.TessEvaluationShaderBit, 0);
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.GeometryShaderBit, 0);
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.FragmentShaderBit, 0);
			GL.UseProgramStages(_pipelineHandle, ProgramStageMask.ComputeShaderBit, 0);
		}

		public void Draw(int handle, int first, int count)
		{
			GL.BindVertexArray(handle);
			GL.DrawArrays(PrimitiveType.Triangles, first, count);
		}

		public void DrawIndexed(int handle, int baseVertex, int count)
		{
			GL.BindVertexArray(handle);
			GL.DrawElementsBaseVertex(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, IntPtr.Zero, baseVertex);
		}

		public void SetRenderTarget(params Texture[] textures)
		{
			if (textures == null || textures.Length == 0 || textures[0] == null)
			{
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
				return;
			}

			// Clean up the old mess
			for (var i = 0; i < _numberOfActiveRenderTargets; i++)
			{
				GL.Ext.NamedFramebufferTexture2D(_frameBufferHandle, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, 0, 0);
			}
			_numberOfActiveRenderTargets = 0;

			GL.Ext.NamedFramebufferTexture(_frameBufferHandle, FramebufferAttachment.DepthAttachment, 0, 0);

			// Setup new bindings
			for (var i = 0; i < textures.Length; i++)
			{
				var texture = textures[i];
				if (texture.Format == PixelInternalFormat.DepthComponent24)
				{
					GL.Ext.NamedFramebufferTexture(_frameBufferHandle, FramebufferAttachment.DepthAttachment, texture.Handle, 0);
				}
				else
				{
					GL.Ext.NamedFramebufferTexture2D(_frameBufferHandle, FramebufferAttachment.ColorAttachment0 + _numberOfActiveRenderTargets, texture.TextureTarget, texture.Handle, 0);
					_numberOfActiveRenderTargets++;
				}
			}

			// Check completeness
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferHandle);
			var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
			{
				throw new Exception("Framebuffer not complete, " + status.ToString());
			}
		}
	}
}
