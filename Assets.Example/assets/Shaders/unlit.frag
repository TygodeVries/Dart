#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

%show uniform vec4 u_Color;

out vec4 FragColor;

void main()
{
   FragColor = u_Color;
}