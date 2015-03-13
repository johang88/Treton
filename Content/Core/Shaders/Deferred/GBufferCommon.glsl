
void EncodeGBuffer(
	in vec4 diffuse, in vec3 normal, 
	in float roughness, in float metallic, in float specular,
	out vec4 gbuffer0, out vec4 gbuffer1, out vec4 gbuffer2
){
	roughness = max(0.01, roughness);

	gbuffer0 = vec4(EncodeDiffuse(diffuse.xyz), 0);
	gbuffer1 = vec4(EncodeNormal(normal), 0);
	gbuffer2 = vec4(roughness, metallic, specular, 0);
}

void DecodeGBuffer(
	in vec4 gbuffer0, in vec4 gbuffer1, in vec4 gbuffer2,
	out vec3 diffuse, out vec3 normal,
	out float roughness, out float metallic, out float specular
) {

	diffuse = DecodeDiffuse(gbuffer0.xyz);
	normal = DecodeNormal(gbuffer1.xyz);
	roughness = gbuffer2.x;
	metallic = gbuffer2.y;
	specular = gbuffer2.z;
}