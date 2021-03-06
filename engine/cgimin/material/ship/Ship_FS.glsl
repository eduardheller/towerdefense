﻿#version 130
precision highp float;

// basis Textur und Normalmap
uniform sampler2D color_texture;

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;

// Parameter für direktionales Licht
uniform vec3 light_direction;
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;
uniform vec4 light_specular_color;
uniform float alpha;

// Parameter für die Specalar-Intensität "Shininess"
uniform float specular_shininess;

// input vom Vertex-Shader
in vec2 fragTexcoord;
in mat3 fragTBN;
in vec4 fragV;

in vec3 normal;

// die finale Farbe
out vec4 outputColor;

void main()
{	

	// den half-vector berechnen
	vec3 h = normalize(light_direction + vec3(fragV));
	float ndoth = clamp(dot( normal, h ), 0, 100.0);
	float specularIntensity = pow(ndoth, specular_shininess);

	// die Helligkeit berechnen, resultierund aus dem Winkel 
	float brightness = clamp(dot(normalize(normal), light_direction), 0, 1);
	
	// surfaceColor ist die farbe aus der Textur...
	vec4 surfaceColor = texture(color_texture, fragTexcoord);

	//				 Ambiente Farbe						  + Diffuser Farbanteil								  + Speculares Licht
    // outputColor = (surfaceColor * light_ambient_color) + (surfaceColor * brightness * light_diffuse_color) + specularIntensity * light_specular_color;
	// obere Zeile surfaceColor ausgeklammert
	 outputColor = surfaceColor * (light_ambient_color +  brightness * light_diffuse_color) + specularIntensity * light_specular_color;
	 outputColor.a *= alpha;

}