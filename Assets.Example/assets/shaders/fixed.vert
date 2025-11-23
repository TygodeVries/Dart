#version 430 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aLifeTime;

layout(binding = 0) uniform sampler2D properties;
layout(location = 0) uniform float texture_offset;
layout(location = 1) uniform ivec4 viewport;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out float vLifeTime;
out vec4 vColor;
void main()
{
    vLifeTime = aLifeTime.x;
    vec4 t = texture(properties, vec2(aLifeTime.x, aLifeTime.z));
    vec4 s = texture(properties, vec2(aLifeTime.x, aLifeTime.z + texture_offset));
    vec4 pos = uProjection * uView * uModel * vec4(aPosition, 1.0);

    gl_PointSize = s.r * viewport.z / pos.w;
    vColor = t;
    gl_Position = pos;
}