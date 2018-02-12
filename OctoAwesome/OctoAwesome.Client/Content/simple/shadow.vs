#version 400
uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

void main()
{
	vec4 position = vec4((inputData & 0xFF),((inputData >> 8) & 0xFF),((inputData >> 16) & 0xFF),1.0);

	gl_Position = WorldViewProj*position;
}

