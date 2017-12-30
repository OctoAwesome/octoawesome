#version 400
uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

out vec3 psNormal;
out vec2 psTexcoord;
flat out uint psTexIndex;

void main()
{
	const vec2[] uvLookup = vec2[4](vec2(0.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0));
	
	const vec3[] normalLookup = vec3[6](vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0));
	vec4 position = vec4((inputData & 0xFF),((inputData >> 8) & 0xFF),((inputData >> 16) & 0xFF),1.0);
	psNormal = normalLookup[(inputData2 >> 24) & 0xF];
	psTexIndex = (inputData>>24);
	psTexcoord = uvLookup[(inputData2 >> 28) & 0xF];
	

	gl_Position = WorldViewProj*position;


}

