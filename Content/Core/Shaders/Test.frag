#version 440 core

#include "Core/Shaders/Lib/Core.glsl"

in Input
{
	vec4 iColor;
};

layout(location = 0) out vec4 oColor;

void main() {
	vec3 color = pow(iColor.xyz, vec3(2.2));
	
	oColor = vec4(color, 1);
}