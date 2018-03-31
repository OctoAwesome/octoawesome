uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

void main()
{

    
	vec4 position = vec4((inputData & 0xFFu),((inputData >> 8) & 0xFFu),((inputData >> 16) & 0xFFu),1.0);

	gl_Position = WorldViewProj*position;
}

