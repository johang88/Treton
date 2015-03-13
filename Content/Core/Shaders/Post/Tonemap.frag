#version 440 core
#include "Core/Shaders/Lib/Core.glsl"
#include "Core/Shaders/Lib/Common.glsl"
#include "Core/Shaders/Deferred/GBufferCommon.glsl"

out VertexData
{
	vec2 texCoord;
} iVertex;

layout(location = 0) out vec4 oColor;

uniform sampler2D sceneSampler;

void main() {
	vec4 scene = texture(samplerScene, iVertex.texCoord);
	oColor = scene;
}