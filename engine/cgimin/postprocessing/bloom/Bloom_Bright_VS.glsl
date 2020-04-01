﻿#version 130
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec2 in_uv; 

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 texcoord;

void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht
	texcoord = in_uv;

	gl_Position = vec4(in_position, 1);
}
