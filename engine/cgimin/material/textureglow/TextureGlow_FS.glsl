#version 330
precision highp float;

uniform sampler2D sampler; 
uniform vec4 glowcolor;
in vec2 texcoord;



void main()
{
	vec4 color = texture(sampler, texcoord);

	color = color * glowcolor;
	gl_FragData[1] = color;
	gl_FragData[0] = color;
}


