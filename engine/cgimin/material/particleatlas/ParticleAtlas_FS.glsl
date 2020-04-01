#version 130
precision highp float;

uniform sampler2D sampler;

in vec2 tex_offset1;
in vec2 tex_offset2;
in float blend;

out vec4 outputColor;

void main()
{
	vec4 colour1 = texture(sampler, tex_offset1);
	vec4 colour2 = texture(sampler, tex_offset2);
    outputColor = mix(colour1,colour2,blend);

	if(outputColor.x <0.11 && outputColor.y <0.11 && outputColor.z < 0.11){
		discard;
	}
}


