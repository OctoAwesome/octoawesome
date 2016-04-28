#version 130
uniform mat4 WorldViewProj;

in vec3 position;
in vec3 normal;
in vec2 texCoord;

out vec3 psNormal;
out vec2 psTexcoord;

void main()
{
	psTexcoord = texCoord;
	psNormal = normalize(normal);
	gl_Position = WorldViewProj*vec4(position,1.0);


}

