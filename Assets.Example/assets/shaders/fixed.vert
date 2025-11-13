#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in float aLifeTime;

out float vLifeTime;
void main()
{
    vLifeTime = aLifeTime;
    gl_PointSize = aLifeTime * 10;
    gl_Position = vec4(aPosition, 1.0);
}