using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics
{
	public class VertexFormat
	{
		public readonly int Size;
		public readonly VertexFormatElement[] Elements;

		public VertexFormat(VertexFormatElement[] elements)
		{
			if (elements == null)
				throw new ArgumentNullException("elements");

			Elements = elements;
			Size = 0;

			foreach (var element in Elements)
			{
				if (element.Type == VertexPointerType.Float || element.Type == VertexPointerType.Int)
					Size += 4 * element.Count;
				else if (element.Type == VertexPointerType.Short)
					Size += 2 * element.Count;
			}
		}
	}
}
