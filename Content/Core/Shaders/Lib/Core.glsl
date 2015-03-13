
#define ATTRIB_POSITION 0
#define ATTRIB_NORMAL 1
#define ATTRIB_TANGENT 2
#define ATTRIB_TEXCOORD_0 3
#define ATTRIB_TEXCOORD_1 4
#define ATTRIB_COLOR 5
#define ATTRIB_BONE_INDEX 6
#define ATTRIB_BONE_WEIGHT 7
// NOTE: It's a 4x4 matrix so it takes up 4 slots
#define ATTRIB_INSTANCE_TRANSFORM 8

layout(std140, binding = 0) uniform PerFrame
{
	// Camera
	vec4 cameraPosition;
	vec4 cameraParameters; // Near, Far, z, w
	
	// Matrices
	mat4 viewMatrix;
	mat4 projectionMatrix;
	mat4 itViewProjectionMatrix;
	mat4 itViewMatrix;
	mat4 itViewProjectionMatrix;
	mat4 inverseViewProjection;
};

layout(std140, binding = 1) uniform PerInstance
{
	mat4 worldMatrix;
	mat4 worlViewMatrix;
	mat4 worldViewProjectionMatrix;
};

float GetCameraNearClipDistance()
{
	return cameraParameters.x;
}

float GetCameraFarClipDistance()
{
	return cameraParameters.y;
}