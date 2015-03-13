#version 440 core

out gl_PerVertex
{
	vec4 gl_Position;
};

out Output
{
	vec4 oColor;
};

void main()
{
	const vec4 vertices[3] = vec4[3](vec4( 0.5, -0.5, 0.5, 1.0),
	                                 vec4(-0.5, -0.5, 0.5, 1.0),
	                                 vec4( 0.5,  0.5, 0.5, 1.0));
									 
	const vec4 colors[3] = vec4[3](vec4(1, 0, 0, 1),
	                               vec4(0, 1, 0, 1),
	                               vec4(0, 0, 1, 1));
									 
	oColor = colors[gl_VertexID];
	gl_Position = vertices[gl_VertexID];
}
