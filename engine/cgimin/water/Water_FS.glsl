#version 330
precision highp float;

in vec4 clipSpace;
in vec2 fragTexCoords;
in vec3 cameraDir;
in vec4 ShadowCoord;
in vec3 cameraSubPos;
in vec4 viewPosition;

uniform sampler2D reflection_texture;
uniform sampler2D refraction_texture;
uniform sampler2D dudv_texture;
uniform sampler2D normal_texture;
uniform sampler2DShadow shadowmap_texture;

uniform vec3 light_color;
uniform vec3 lightDir;
uniform float time;

out vec4 outputColor;

const float wave_strength = 0.01;
const float shininess = 2;
const float reflective = 22.5;

// Fog Start, Ende und Farbe
uniform float fogStart;
uniform float fogEnd;
uniform vec3 fogColor;

void main()
{
	vec2 NDC = (clipSpace.xy/clipSpace.w)/2.0 + 0.5;

	vec2 refractTexCoords = vec2(NDC.x,NDC.y);
	vec2 reflectTexCoords = vec2(NDC.x,-NDC.y);
	
	vec2 distort_1 = texture(dudv_texture, vec2(fragTexCoords.x+time, fragTexCoords.y)).rg * 0.1;
	distort_1 = fragTexCoords + vec2(distort_1.x, distort_1.y+time);

	vec2 distort = (texture(dudv_texture, distort_1).rg * 2.0 - 1.0) * wave_strength;

	refractTexCoords += distort;
	refractTexCoords = clamp(refractTexCoords,0.001,0.999);

	reflectTexCoords += distort;
	reflectTexCoords.x = clamp(reflectTexCoords.x,0.001,0.999);
	reflectTexCoords.y = clamp(reflectTexCoords.y,-0.999,-0.001);

	vec4 refraction = clamp(texture(refraction_texture,refractTexCoords),0.1,1.0);
    vec4 reflection =  clamp(texture(reflection_texture,reflectTexCoords),0.1,1.0);

	vec4 normalMapColour = texture(normal_texture,distort);
	vec3 normal = vec3(normalMapColour.r * 2.0 - 1.0, normalMapColour.b, normalMapColour.g * 2.0 -1.0);
	normal = normalize(normal);


	vec3 viewDir = normalize(cameraDir);
	float refFactor = dot(viewDir, normal);
	if(refFactor>=0){
		refFactor = pow(refFactor,0.5);
	}
	else
	{
		refFactor = 1.0;
	}


	vec3 nlightDir = normalize(lightDir);

	vec3 ref = reflect(nlightDir,normal);
	float spec = max(dot(ref,viewDir),0.0);
	spec = pow(spec,shininess);
	vec3 highlights = light_color * spec * reflective;


	vec4 color = mix(reflection,refraction,refFactor);
	outputColor = mix(color,vec4(0.0,0.3,0.5,1), 0.2) + vec4(highlights,0);




}