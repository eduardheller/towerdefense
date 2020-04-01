#version 330
precision highp float;

uniform sampler2D sampler; 

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;

// Parameter für direktionales Licht
uniform vec3 light_direction;
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;
uniform vec4 light_specular_color;
uniform vec3 color;

// Parameter für die Specalar-Intensität "Shininess"
uniform float specular_shininess;

// input vom Vertex-Shader
in vec2 fragTexcoord;
in vec3 fragNormal;
in vec4 fragV;

// die finale Farbe
out vec4 outputColor;

void main()
{	
	// die Vertex-Normale berechnen
    vec3 normal = normalize(mat3(model_matrix) * fragNormal);

	// den half-vector berechnen
	vec3 h = normalize(light_direction + vec3(fragV));
	float ndoth = dot( normal, h );
	float specularIntensity = pow(ndoth, specular_shininess);

	// die Helligkeit berechnen, resultierund aus dem Winkel 
	float brightness = clamp(dot(normalize(normal), light_direction), 0, 1);
	
	// surfaceColor ist die farbe aus der Textur...
	vec4 surfaceColor = texture(sampler, fragTexcoord) + vec4(color,1);

	//				 Ambiente Farbe						  + Diffuser Farbanteil								  + Speculares Licht
    // outputColor = (surfaceColor * light_ambient_color) + (surfaceColor * brightness * light_diffuse_color) + specularIntensity * light_specular_color;
	// obere Zeile surfaceColor ausgeklammert
	 outputColor = surfaceColor * (light_ambient_color +  brightness * light_diffuse_color) + specularIntensity * light_specular_color;

}