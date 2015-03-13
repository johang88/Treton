#version 440 core
#include "Core/Shaders/Lib/Core.glsl"

layout(location = ATTRIB_POSITION) in vec3 iPosition;
layout(location = ATTRIB_TEXCOORD_0) in vec2 iTexCoord;
layout(location = ATTRIB_NORMAL) in vec3 iNormal;
layout(location = ATTRIB_TANGENT) in vec3 iTangent;

out gl_PerVertex
{
	vec4 gl_Position;
};

out VertexData
{
	vec2 texCoord;
	vec3 tangent;
	vec3 bitangent;
	vec3 normal;
} oVertex;

void main()
{
	oVertex.texCoord = iTexCoord;
	
	vec3 normal = normalize(iNormal);
	vec3 tangent = normalize(iTangent);
	vec3 bitangent = normalize(cross(normal, tangent));
	
	oVertex.normal = (worldMatrix * vec4(normal, 0));
	oVertex.tangent = (worldMatrix * vec4(tangent, 0));
	oVertex.bitangent = (worldMatrix * vec4(bitangent, 0));
	
	gl_Position = worldViewProjectionMatrix * vec4(iPosition, 1);
}