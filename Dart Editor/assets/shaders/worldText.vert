#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 Pos;
out vec3 Normal;
out vec2 Uv;

void main()
{
    vec3 temp = aPosition;
    temp = temp;
    gl_Position = uProjection * uView * uModel * vec4(temp, 1.0);

    Pos = vec3(uModel * vec4(aPosition, 1.0));
    Normal = normal;
    Uv = uv;
}