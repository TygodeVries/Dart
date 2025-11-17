#version 330 core

in float vLifeTime;
in vec3 vColor;
out vec4 FragColor;
void main()
{
    vec2 p = gl_PointCoord;
    float r = length(p - vec2(0.5,0.5)) + 0.5;
    FragColor = vec4(vColor,r);
}