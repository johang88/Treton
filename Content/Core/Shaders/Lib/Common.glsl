#define PI 3.14159265358979323846264
#define PI_OVER_FOUR PI/4
#define PI_OVER_TWO PI/2

float Saturate(float v)
{
	return clamp(v, 0.0, 1.0);
}

float Square(float x)
{
	return x * x;
}

vec3 EncodeNormal(vec3 n)
{
	return n * 0.5 + 0.5;
}

vec3 DecodeNormal(vec3 n)
{
	return n * 2.0 - 1.0;
}

vec3 EncodeDiffuse(vec3 d)
{
	return sqrt(d);
}

vec3 DecodeDiffuse(vec3 d)
{
	return d * d;
}

vec3 DecodeWorldPosition(vec2 coord, float depth) {
	depth = depth * 2.0 - 1.0;
	
	vec3 clipSpacePosition = vec3(coord * 2.0 - 1.0, depth);
	vec4 worldPosition = inverseViewProjection * vec4(clipSpacePosition, 1);
	
	return worldPosition.xyz / worldPosition.w;
}