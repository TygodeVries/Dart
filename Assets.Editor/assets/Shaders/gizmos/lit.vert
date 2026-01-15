#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;
layout(location = 3) in vec4 tangent;

uniform vec3 light_direction;
uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;

out vec3 Pos;
out vec3 Normal;
out vec2 Uv;
out vec4 Tangent;
out vec3 LightDir;

void main()
{
    gl_Position = u_Projection * u_View * u_Model * vec4(aPosition, 1.0);

    Pos = vec3(u_Model * vec4(aPosition, 1.0));

    mat3 normalMatrix = transpose(inverse(mat3(u_Model)));
    Normal = normalize(normalMatrix * normal);

    vec3 T = normalize(normalMatrix * tangent.xyz);
    Tangent = vec4(T, tangent.w);

    LightDir = normalize(light_direction);
    Uv = uv;
}
