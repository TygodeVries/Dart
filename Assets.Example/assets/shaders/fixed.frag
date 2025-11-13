#version 330 core

in float vLifeTime;
out vec4 FragColor;
void main()
{
    vec2 p = gl_PointCoord;
    float r = length(p - vec2(0.5,0.5)) + 0.5;
    FragColor = vec4(1,vLifeTime / 10,0,r);
}