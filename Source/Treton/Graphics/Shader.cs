using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Treton.Graphics
{
	public class Shader : IDisposable
	{
		public int Handle { get; private set; }
		public readonly ShaderType Type;
		public readonly Dictionary<uint, int> Uniforms = new Dictionary<uint, int>();

		public Shader(ShaderType type, string source)
		{
			Type = type;
			Handle = GL.CreateShaderProgram(type, 1, new string[] { source });

			int errorCode;
			GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out errorCode);

			if (errorCode != 1)
			{
				string errors;
				GL.GetProgramInfoLog(Handle, out errors);

				GL.DeleteProgram(Handle);
				Handle = 0;

				throw new Exception(errors);
			}

			// Discover all uniforms
			int uniformCount;
			GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out uniformCount);

			for (var i = 0; i < uniformCount; i++)
			{
				int size;
				ActiveUniformType uniformType;

				var name = GL.GetActiveUniform(Handle, i, out size, out uniformType);
				name = name.Replace("[0]", "");
				var location = GL.GetUniformLocation(Handle, name);

				Uniforms.Add(Core.Hash.HashString(name), location);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Shader()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Handle != 0)
			{
				GL.DeleteProgram(Handle);
				Handle = 0;
			}
		}

		public bool HasUniform(uint name)
		{
			return Uniforms.ContainsKey(name);
		}

		public int GetUniformLocation(uint name)
		{
			return Uniforms[name];
		}
	}
}
