#version 440 core
#include "Core/Shaders/Lib/Core.glsl"
#include "Core/Shaders/Lib/Common.glsl"
#include "Core/Shaders/Deferred/GBufferCommon.glsl"

in VertexData
{
	vec2 texCoord;
} iVertex;

layout(location = 0) out vec4 oColor;

uniform sampler2D sceneSampler;

vec3 filmicalu(vec3 c) {
	c = max(vec3(0), c - 0.004);
	c = (c* (6.2 * c + 0.5)) / (c * (6.2 * c + 1.7) + 0.06);
	
	// 1/2.2 built in, need to go back to linear
	// return pow(c, vec3(2.2));
	return c;
}


void main() {
	vec3 scene = texture(sceneSampler, iVertex.texCoord).xyz;
	
	vec3 color = filmicalu(scene);
	
	// oColor.xyz  = pow(color, vec3(1.0 / 2.2));
	oColor.xyz = color;
	oColor.w = 1;
}