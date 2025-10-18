#version 330 core

in vec3 Pos;
in vec3 Normal;
in vec2 Uv;
in vec4 Tangent;

uniform sampler2D u_Texture;
uniform sampler2D u_NormalMap;
uniform sampler2D u_Rough;

uniform float u_shininess;
uniform vec3 u_camera_pos;

// Lighting (points)
uniform vec3 u_point_light_pos[16];
uniform vec3 u_point_light_col[16];
uniform vec3 u_point_light_data[16];
uniform int u_pointLight_Count;

// Lighting (direct)
uniform vec3 u_sun_Direction;
uniform vec3 u_sun_Color;

// Lighting (ambient)
uniform vec3 u_ambient_color;

out vec4 FragColor;

void main()
{
    vec4 col = texture(u_Texture, Uv);
    vec3 normalMap = texture(u_NormalMap, Uv).rgb * 2.0 - 1.0;
    vec3 rough = texture(u_Rough, Uv).rgb;


    vec3 viewDir = normalize(u_camera_pos - Pos);  // View direction

    vec3 T = normalize(Tangent.xyz);
    vec3 N = normalize(Normal);
    vec3 B = cross(N, T) * Tangent.w;

    mat3 TBN = mat3(T, B, N);
    vec3 normal = normalize(TBN * normalMap);

    // The resulting light
    vec3 light = u_ambient_color;

    // Calculate direct lighting
    light = light + u_sun_Color * max(dot(normal, -u_sun_Direction), 0.0);

    for(int i = 0; i < u_pointLight_Count; i++)
    {
        vec3 point_pos = u_point_light_pos[i];

        vec3 toLight = point_pos - Pos;
        float dist = length(toLight);
        vec3 lightDir = normalize(toLight);

        float dotV = max(dot(normal, lightDir), 0.0);

        float attenuation = clamp(1.0 - dist * (1 / u_point_light_data[i].x), 0.0, 1.0); 

        vec3 diffuse = u_point_light_col[i] * dotV * attenuation * 10;

        vec3 halfDir = normalize(lightDir + viewDir);
        float dotNH = max(dot(normal, halfDir), 0.0);
        float spec = pow(dotNH, (1 - rough.x) * 16) * attenuation;

        vec3 specular = u_point_light_col[i] * spec * 10;

        light += diffuse + specular;
    }

    FragColor = vec4(col.rgb * light, col.a);
}