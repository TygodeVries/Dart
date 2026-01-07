#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

uniform sampler2D u_Texture;

uniform float u_shininess;
uniform vec3 u_camera_pos;

out vec4 FragColor;

void main()
{
    vec4 col = texture(u_Texture, Uv);
    FragColor = vec4(col.rgb, 1.0);
}