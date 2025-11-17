#version 430 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aLifeTime;

layout(binding = 0) uniform sampler2D properties;

out float vLifeTime;
out vec3 vColor;
void main()
{
    vLifeTime = aLifeTime.x;
    vec4 t = texture(properties, vec2(aLifeTime.x, aLifeTime.z));
    gl_PointSize = t.a;
    vColor = t.rgb;
    gl_Position = vec4(aPosition, 1.0);
}