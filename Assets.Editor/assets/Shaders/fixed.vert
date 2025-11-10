#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aColor;
layout(location = 2) in float aAngle;
layout(location = 3) in vec2 aSize;
layout(location = 4) in vec2 aModelPosition;
out vec4 fColor;

void main()
{
    vec2 cs = vec2(cos(aAngle), sin(aAngle));
    mat2 a = mat2(cs.x,cs.y,-cs.y,cs.x);
    vec2 pos = a * vec2(aPosition.x * aSize.x / 2, aPosition.y * aSize.y / 2);
    gl_Position = vec4(pos+aModelPosition, aPosition.z, 1.0);

    fColor = vec4(aColor, 1.0);
}