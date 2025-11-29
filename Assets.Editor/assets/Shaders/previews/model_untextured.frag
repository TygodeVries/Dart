#version 330 core

in vec3 Pos;
in vec3 Normal;
out vec4 FragColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 tintColor;

void main()
{
    float val = (dot(Normal, vec3(0.8, 1, 0.8)) + 1) / 2.0;
    FragColor = vec4(val, val, val, 1) * vec4(tintColor, 1);
}