#version 400

uniform mat4 World;
uniform mat4 View;
uniform mat4 Proj;

in vec3 position;
in vec3 normal;
in vec2 textureCoord;

out vec4 psFragWorldSpace;
out vec3 psNormal;
out vec2 psTextureCoord;

void main()
{
	psNormal = normal;
	psTextureCoord = textureCoord;

    vec4 worldPos = World * vec4(position, 1.0);
	gl_Position = (Proj * View) * worldPos;
	psFragWorldSpace = worldPos;
}