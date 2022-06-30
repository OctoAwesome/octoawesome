#version 400

uniform mat4 World;

in vec3 position;
in vec3 normal;
in vec2 textureCoord;


void main()
{
	gl_Position = World * vec4(position, 1.0);
}