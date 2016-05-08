#version 400
uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

out vec3 psNormal;
out vec2 psTexcoord;


void main()
{
	const vec2[] uvLookup = {vec2(0.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0)};
	
	const vec3[] normalLookup = {vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0)};
	vec4 position = vec4(inputData & 0xFF,(inputData >> 8) & 0xFF,(inputData >> 16) & 0xFF,1.0);
	psNormal = normalLookup[(inputData >> 24) & 0xF];
	//psTexcoord = uvLookup[(inputData >> 28) & 0xF];
	psTexcoord = vec2(float(inputData2 >> 16) / 65536.0,float(inputData2 & 0xFFFF) / 65536.0);
	gl_Position = WorldViewProj*position;


}

