#version 330
precision highp float;


// Basis Textur und Normalmap
uniform sampler2D color_texture;
uniform samplerCube cube_texture;

// Parameter für direktionales Licht
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;

// Fog Start, Ende und Farbe
uniform float fogStart;
uniform float fogEnd;
uniform vec3 fogColor;

// input vom Vertex-Shader
in vec2 fragTexcoord;
in vec3 fragCubeTexCoord;

in vec4 viewPosition;
in float brightness;

void main()
{	
	vec4 cubeColor = texture(cube_texture, fragCubeTexCoord);
	vec4 surfaceColor = texture(color_texture, fragTexcoord);
		
	gl_FragData[0] = surfaceColor * (light_ambient_color +  brightness * light_diffuse_color) + cubeColor;

	float fogFactor = (fogEnd - length(viewPosition.xyz)) / (fogEnd - fogStart);
	fogFactor = clamp(fogFactor, 0, 1);
	gl_FragData[0] = fogFactor * gl_FragData[0] + ((1 - fogFactor) * vec4(fogColor, 1));

}