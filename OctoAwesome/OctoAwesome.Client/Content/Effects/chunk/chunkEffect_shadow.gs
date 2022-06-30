#version 400

#ifndef CASCADES
#define CASCADES 2
#endif


layout (triangles, invocations = CASCADES) in;
layout (triangle_strip, max_vertices = 3) out;

uniform mat4 CropMatrices[CASCADES];

void main()
{
    for (int i = 0; i < 3; ++i)
    {
        gl_Position = CropMatrices[gl_InvocationID] * gl_in[i].gl_Position;
        gl_Layer = gl_InvocationID;
        EmitVertex();
    }

    EndPrimitive();
}
