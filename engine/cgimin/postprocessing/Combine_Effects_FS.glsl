#version 130
precision highp float;

uniform sampler2D original; 
uniform sampler2D effected; 

in vec2 texcoord;
out vec4 outputColor;

const float a = 1;
void main()
{	

	vec4 I = texture(original,texcoord);
    vec4 IH = texture(effected,texcoord);
	vec4 M = I - IH;
	outputColor = I+IH;
}