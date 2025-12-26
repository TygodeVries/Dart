#version 330 core

in vec4 vColor;
out vec4 FragColor;
void main()
{
    vec2 p = gl_PointCoord;
    float r = clamp(1 - 2 * length(p - vec2(0.5,0.5)), 0, 1);
    vec4 color = vColor;
    color.a *= r;
    FragColor = vec4(color);
}