#version 400
uniform mat4 WorldViewProj;
uniform mat4 shadowWorldViewProj;

out vec4 shadowPosition;

in vec4 position;
in vec4 color;
in vec2 textureCoordinate;
out vec4 psColor;
out vec2 psTexCoord;


void main()
{
    psColor = color;
    psTexCoord = textureCoordinate;
    
	gl_Position = WorldViewProj*position;
    shadowPosition = shadowWorldViewProj* position;

}

