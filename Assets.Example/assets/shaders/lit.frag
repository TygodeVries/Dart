#version 330 core

in vec3 FragPos;
in vec3 WorldNormal;
in vec3 WorldTangent;
in vec4 TangentSpace;
in vec2 UV;

uniform sampler2D u_Texture;
uniform sampler2D u_NormalMap;
uniform sampler2D u_Rough;
uniform samplerCube u_Sky;
uniform vec3 u_camera_pos;

// Lights
uniform int u_pointLight_Count;
uniform vec3 u_point_light_pos[16];
uniform vec3 u_point_light_col[16];
uniform vec3 u_point_light_data[16];
uniform int u_hasSky;

uniform vec3 u_sun_Direction;
uniform vec3 u_sun_Color;
uniform vec3 u_ambient_color;

out vec4 FragColor;

void main()
{
    vec4 albedo = texture(u_Texture, UV);
    vec3 rough = texture(u_Rough, UV).rgb;
    vec3 normalMap = texture(u_NormalMap, UV).rgb * 2.0 - 1.0;

    vec3 N = normalize(WorldNormal);
    vec3 T = normalize(WorldTangent - dot(WorldTangent, N) * N); // Gram-Schmidt
    vec3 B = cross(N, T) * TangentSpace.w;

    mat3 TBN = mat3(T, B, N);

    vec3 normal = normalize(TBN * normalMap);

    vec3 viewDir = normalize(u_camera_pos - FragPos);

    // Start building light
	
	
	vec3 lightVal = vec3(0, 0, 0) * rough.x;
	if(u_hasSky == 1)
	{
	  lightVal = vec3(texture(u_Sky, normal)) * rough.x;
	}

    // Sun
    lightVal += u_sun_Color * max(dot(normal, -u_sun_Direction), 0.0) * (1 - rough.x);

    // Point lights
    for(int i = 0; i < u_pointLight_Count; i++)
    {
        vec3 toLight = u_point_light_pos[i] - FragPos;
        float dist = length(toLight);
        vec3 lightDir = normalize(toLight);

        float dotNL = max(dot(normal, lightDir), 0.0);
        float attenuation = clamp(1.0 / (dist * dist) * (1.0 / u_point_light_data[i].x), 0.0, 1.0);
        vec3 diffuse = u_point_light_col[i] * dotNL * attenuation * 10.0;

        vec3 halfDir = normalize(lightDir + viewDir);
        float dotNH = max(dot(normal, halfDir), 0.0);
        float spec = pow(dotNH, (1.0 - rough.x) * 16.0) * attenuation;
        vec3 specular = u_point_light_col[i] * spec * 10.0;

        lightVal += diffuse + specular;
    }

    FragColor = vec4(albedo.rgb * lightVal, albedo.a);
}
