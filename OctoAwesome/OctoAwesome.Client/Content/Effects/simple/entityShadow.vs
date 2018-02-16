in vec3 position;

uniform mat4 WorldViewProj;

void main(void)
{
   gl_Position = WorldViewProj*vec4(position, 1.0);
}