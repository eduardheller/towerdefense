#version 130
precision highp float;


uniform sampler2D sampler; 
in vec2 texcoord;

uniform float alpha;
uniform float width;
uniform float edge;
uniform vec3 color;

out vec4 outputColor;

void main()
{

	float distance = 1.0 - texture(sampler, texcoord).a;
	float a = 1.0 - smoothstep(width, width+edge, distance);
	outputColor = vec4(color,a*alpha);

}


