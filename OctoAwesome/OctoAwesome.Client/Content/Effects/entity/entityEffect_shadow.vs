#version 400

uniform mat4 World;
uniform mat4 ViewProjection;

in vec3 position;
in vec3 normal;
in vec2 textureCoord;


void main()
{
	gl_Position = (ViewProjection * World) * vec4(position, 1.0);
}