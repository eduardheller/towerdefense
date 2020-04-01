#version 330 core
precision highp float;

// basis Textur und Normalmap
uniform sampler2D color_texture;
uniform sampler2D normalmap_texture;
uniform sampler2DShadow shadowmap_texture;


// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;

// Parameter für direktionales Licht
uniform vec3 light_direction;
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;
uniform vec4 light_specular_color;

// Parameter für die Specalar-Intensität "Shininess"
uniform float specular_shininess;

// input vom Vertex-Shader
in vec2 fragTexcoord;
in mat3 fragTBN;
in vec4 fragV;
in vec4 ShadowCoord;

// die finale Farbe
out vec4 outputColor;

void main()
{	
	// die Vertex-Normale berechnen
    vec3 normal = texture(normalmap_texture, fragTexcoord).rgb;
	normal = normalize(normal * 2.0 - 1.0); 
	normal = normalize(fragTBN * normal); 

	// den half-vector berechnen
	vec3 h = normalize(light_direction + vec3(fragV));
	float ndoth = clamp(dot( normal, h ), 0, 100);
	float specularIntensity = pow(ndoth, specular_shininess);

	// die Helligkeit berechnen, resultierund aus dem Winkel 
	float brightness = clamp(dot(normalize(normal), light_direction), 0, 1);
	
	// surfaceColor ist die farbe aus der Textur...
	
	

	vec4 surfaceColor = texture(color_texture, fragTexcoord);
	
	float visibility = 1.0;
	vec3 shadowTexCoord = vec3(ShadowCoord.xy, (ShadowCoord.z)/ShadowCoord.w * 0.998);
	vec3 inCmp = shadowTexCoord - vec3(0.5, 0.5, 0.5);

	if (dot(inCmp, inCmp) < 0.25) {
		//visibility = texture(shadowmap_texture, shadowTexCoord);

		visibility = texture(shadowmap_texture, shadowTexCoord + vec3( -0.94201624, -0.39906216, 0.0 ) / 2800.0);
		visibility += texture(shadowmap_texture, shadowTexCoord + vec3( 0.94558609, -0.76890725, 0.0 ) / 2800.0);
		visibility += texture(shadowmap_texture, shadowTexCoord + vec3( -0.094184101, -0.92938870, 0.0 ) / 2800.0);
		visibility += texture(shadowmap_texture, shadowTexCoord + vec3( 0.34495938, 0.29387760, 0.0 ) / 2800.0);

		visibility *= 0.25;

		if (visibility < 0.25) visibility = 0.0;
	}



	//				 Ambiente Farbe						  + Diffuser Farbanteil								  + Speculares Licht
    // outputColor = (surfaceColor * light_ambient_color) + (surfaceColor * brightness * light_diffuse_color) + specularIntensity * light_specular_color;
	// obere Zeile surfaceColor ausgeklammert
	 outputColor = surfaceColor * (light_ambient_color +  brightness * light_diffuse_color * visibility) + specularIntensity * visibility * light_specular_color;

}