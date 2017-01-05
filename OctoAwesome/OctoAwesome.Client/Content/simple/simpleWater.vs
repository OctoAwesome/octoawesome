#version 400
uniform mat4 WorldView;
uniform mat4 Proj;
uniform float time;
in uint inputData;
in uint inputData2;

out vec3 v;
out vec3 psNormal;
out vec2 psTexcoord;
out vec4 psPosition;
flat out uint psTexIndex;

uniform vec3 viewDir;

void main()
{
	const vec2[] uvLookup = vec2[4](vec2(0.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0));
	
	const vec3[] normalLookup = vec3[6](vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0));
	psPosition = vec4((inputData & 0xFF),((inputData >> 8) & 0xFF),((inputData >> 16) & 0xFF),1.0);
	psNormal = normalLookup[(inputData2 >> 24) & 0xF];
	psTexIndex = (inputData>>24);
	psTexcoord = uvLookup[(inputData2 >> 28) & 0xF];
	psPosition = psPosition + vec4(0.0,0.0,(sin(psPosition.x*3.14/16.0+time+psPosition.y*3.14/16))/4.0-0.3,0.0);
    psPosition = Proj*WorldView*psPosition;
	gl_Position = psPosition;
    psPosition = Proj*WorldView*vec4(viewDir,1.0)-psPosition;

}

