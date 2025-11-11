#version 330 core
in vec3 Pos;
in vec3 Normal;
in vec2 Uv;

uniform sampler2D u_Texture;

out vec4 FragColor;

void main()
{
    vec4 col = texture(u_Texture, Uv);
    if(col.r < 0.5)
    {
        discard;
    }

    FragColor = vec4(1, 1, 1, 1);
}