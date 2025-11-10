#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

uniform sampler2D u_Texture;
uniform sampler2D u_NormalMap;
uniform sampler2D u_Rough;

uniform float u_shininess;
uniform vec3 u_camera_forwards;

out vec4 FragColor;

void main()
{
    vec4 col = texture(u_Texture, Uv);
    vec3 normalMap = texture(u_NormalMap, Uv).rgb * 2.0 - 1.0;

    vec2 sample = vec2(Uv.x, Uv.y * u_camera_forwards.y);

    FragColor = vec4(sample, 0, 0);
}