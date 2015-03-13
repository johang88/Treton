#version 440 core
#include "Core/Shaders/Lib/Core.glsl"
#include "Core/Shaders/Lib/Common.glsl"
#include "Core/Shaders/Deferred/GBufferCommon.glsl"

out VertexData
{
	vec2 texCoord;
	vec3 tangent;
	vec3 bitangent;
	vec3 normal;
} iVertex;

layout(location = 0) out vec4 oGBuffer0;
layout(location = 1) out vec4 oGBuffer1;
layout(location = 2) out vec4 oGBuffer2;

uniform sampler2D diffuseSampler;
uniform sampler2D normalSampler;
uniform sampler2D rmsSampler; 

void main() {
	vec4 diffuse = texture(diffuseSampler, iVertex.texCoord);
	
	mat3x3 tbn = mat3x3(normalize(iVertex.tangent), normalize(iVertex.bitangent), normalize(iVertex.normal));
	
	vec3 normal = DecodeNormal(texture(normalSampler, iVertex.texCoord));
	normal = normalize(tbn * normalize(normal));
	
	vec3 rms = texture(rmsSampler, iVertex.texCoord);
	
	float roughness = rms.x;
	float metallic = rms.y;
	float specular = rms.z;
	
	EncodeGBuffer(diffuse, normal, roughness, metallic, specular, oGBuffer0, oGBuffer1, oGBuffer2);
}