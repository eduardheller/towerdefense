#version 330
precision highp float;

uniform sampler2D sampler;
uniform float alpha;
in vec2 fragTexcoord;

out vec4 outputColor;

void main()
{

    outputColor = texture(sampler, fragTexcoord);
	outputColor.a *= alpha;
}


