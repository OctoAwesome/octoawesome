uniform mat4 WorldViewProj;
uniform mat4 shadowWorldViewProj;

in uint inputData;
in uint inputData2;

out vec4 shadowPosition;
out vec3 psNormal;
out vec2 psTexcoord;
flat out uint psTexIndex;

void main()
{
	const vec2[] uvLookup = vec2[4](vec2(0.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0));
	
	const vec3[] normalLookup = vec3[6](vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0));
	vec4 position = vec4((inputData & 0xFFu),((inputData >> 8) & 0xFFu),((inputData >> 16) & 0xFFu),1.0);
	psNormal = normalLookup[(inputData2 >> 24) & 0xFu];
	psTexIndex = (inputData>>24);
	psTexcoord = uvLookup[(inputData2 >> 28) & 0xFu];
	

	gl_Position = WorldViewProj*position;
    shadowPosition = shadowWorldViewProj* position;

}

