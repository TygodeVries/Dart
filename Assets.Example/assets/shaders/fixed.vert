#version 430 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aLifeTime;

layout(binding = 0) uniform sampler2D properties;
layout(location = 0) uniform float texture_offset;

out float vLifeTime;
out vec4 vColor;
void main()
{
    vLifeTime = aLifeTime.x;
    vec4 t = texture(properties, vec2(aLifeTime.x, aLifeTime.z));
    vec4 s = texture(properties, vec2(aLifeTime.x, aLifeTime.z + texture_offset));
    gl_PointSize = s.r;
    vColor = t;
    gl_Position = vec4(aPosition, 1.0);
}