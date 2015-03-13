#version 440 core
#include "Core/Shaders/Lib/Core.glsl"

layout(location = ATTRIB_POSITION) in vec3 iPosition;
layout(location = ATTRIB_TEXCOORD_0) in vec2 iTexCoord;

out gl_PerVertex
{
	vec4 gl_Position;
};

out VertexData
{
	vec2 texCoord;
} oVertex;

void main()
{
	oVertex.texCoord = iTexCoord;
	gl_Position = vec4(iPosition, 1);
}