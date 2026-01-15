#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUV;
layout(location = 3) in vec4 aTangent;

uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;

out vec3 FragPos;
out vec3 WorldNormal;
out vec3 WorldTangent;
out vec4 TangentSpace; // tangent.w for handedness
out vec2 UV;

void main()
{
    vec4 worldPos = u_Model * vec4(aPosition, 1.0);
    FragPos = worldPos.xyz;

    // Transform normal and tangent to world space
    WorldNormal = normalize(mat3(u_Model) * aNormal);
    WorldTangent = normalize(mat3(u_Model) * aTangent.xyz);
    TangentSpace = aTangent;

    UV = aUV;

    gl_Position = u_Projection * u_View * worldPos;
}
