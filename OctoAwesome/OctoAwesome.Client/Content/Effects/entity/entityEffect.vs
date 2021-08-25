#version 400

uniform mat4 World;
uniform mat4 ViewProjection;
uniform mat4 DepthBiasViewProj;

in vec3 position;
in vec3 normal;
in vec2 textureCoord;

out vec3 psNormal;
out vec2 psTextureCoord;
out vec4 psShadowCoord;

void main()
{
	psNormal = normal;
	psTextureCoord = textureCoord;

	gl_Position = (ViewProjection * World) * vec4(position, 1.0);
	psShadowCoord = (DepthBiasViewProj * World) * vec4(position, 1.0);
}