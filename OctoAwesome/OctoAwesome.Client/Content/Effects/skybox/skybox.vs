uniform mat4 WorldViewProj;

in vec4 position;
in vec4 color;
in vec2 textureCoordinate;
out vec2 psTexCoord;


void main()
{
    psTexCoord = textureCoordinate;
    
	gl_Position = WorldViewProj*position;
}

