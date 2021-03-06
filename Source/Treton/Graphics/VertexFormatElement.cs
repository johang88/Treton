﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treton.Graphics
{
	public class VertexFormatElement
	{
		public readonly VertexFormatSemantic Semantic;
		public readonly VertexPointerType Type;
		public readonly byte Count;
		public readonly short Offset;
		public readonly short Divisor;

		public VertexFormatElement(VertexFormatSemantic semantic, VertexPointerType type, byte count, short offset, short divisor = 0)
		{
			Semantic = semantic;
			Type = type;
			Count = count;
			Offset = offset;
			Divisor = divisor;
		}
	}
}
