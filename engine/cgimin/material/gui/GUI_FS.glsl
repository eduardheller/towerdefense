﻿#version 130
precision highp float;

uniform sampler2D sampler; 

in vec2 texcoord;

uniform float alpha;
out vec4 outputColor;

void main()
{


    outputColor = texture(sampler, texcoord);
	outputColor.a *= alpha;
}