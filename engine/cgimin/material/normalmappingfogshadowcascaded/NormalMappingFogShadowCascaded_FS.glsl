#version 330
precision highp float;

const int NUM_CASCADES = 3;

// Basis Textur und Normalmap
uniform sampler2D color_texture;
uniform sampler2D normalmap_texture;

// Shadowmapping Texturen
uniform sampler2DShadow shadowmap_texture1;
uniform sampler2DShadow shadowmap_texture2;
uniform sampler2DShadow shadowmap_texture3;

uniform float dist3;
uniform float dist2;
uniform float dist1;


// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;

// Parameter für direktionales Licht
uniform vec3 light_direction;
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;
uniform vec4 light_specular_color;

// Parameter für die Specalar-Intensität "Shininess"
uniform float specular_shininess;

// Fog Start, Ende und Farbe
uniform float fogStart;
uniform float fogEnd;
uniform vec3 fogColor;

// input vom Vertex-Shader
in vec2 fragTexcoord;
in mat3 fragTBN;
in vec4 fragV;
in vec4 ShadowCoord[NUM_CASCADES];

// fog
in vec4 viewPosition;

// die finale Farbe
// out vec4 outputColor;

void main()
{	
	// die Vertex-Normale berechnen
    vec3 normal = texture(normalmap_texture, fragTexcoord).rgb;
	normal = normalize(normal * 2.0 - 1.0); 
	normal = normalize(fragTBN * normal); 

	// den half-vector berechnen
	vec3 h = normalize(light_direction + vec3(fragV));
	float ndoth = clamp(dot( normal, h ), 0, 100.0);
	float specularIntensity = pow(ndoth, specular_shininess);

	// die Helligkeit berechnen, resultierund aus dem Winkel 
	float brightness = clamp(dot(normalize(normal), light_direction), 0, 1);
	
	// surfaceColor ist die farbe aus der Textur...
	vec4 surfaceColor = texture(color_texture, fragTexcoord);
	
	float visibility = 1.0;
	

	if (-viewPosition.z < dist1) {
		visibility = texture(shadowmap_texture1, vec3(ShadowCoord[0].xy, ShadowCoord[0].z/ShadowCoord[0].w * 0.998));
	} else if (-viewPosition.z < dist2) {
		visibility = texture(shadowmap_texture2, vec3(ShadowCoord[1].xy, ShadowCoord[1].z/ShadowCoord[1].w * 0.998));
	} else if (-viewPosition.z < dist3) {
		visibility = texture(shadowmap_texture3, vec3(ShadowCoord[2].xy, ShadowCoord[2].z/ShadowCoord[2].w * 0.998));
	}	

	


	//				 Ambiente Farbe						  + Diffuser Farbanteil								  + Speculares Licht
    // outputColor = (surfaceColor * light_ambient_color) + (surfaceColor * brightness * light_diffuse_color) + specularIntensity * light_specular_color;
	// obere Zeile surfaceColor ausgeklammert
	gl_FragData[0] = surfaceColor * (light_ambient_color +  brightness * light_diffuse_color * visibility) + specularIntensity * (0.5 + visibility * 0.5) * light_specular_color;

	float fogFactor = (fogEnd - length(viewPosition.xyz)) / (fogEnd - fogStart);
	fogFactor = clamp(fogFactor, 0, 1);
	gl_FragData[0] = fogFactor * gl_FragData[0] + ((1 - fogFactor) * vec4(fogColor, 1));
}