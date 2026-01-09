#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

out vec4 FragColor;

void main()
{
   FragColor = vec4(1.0, 0.0, 1.0, 1.0);
}