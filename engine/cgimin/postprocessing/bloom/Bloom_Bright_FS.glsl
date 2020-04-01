#version 130
precision highp float;

uniform sampler2D sampler; 

in vec2 texcoord;
out vec4 outputColor;


vec4 saturate(vec4 inp) 
{ 
	return clamp(inp, 0.0, 1.0); 
} 

void main(void) 
{ 


	// Texturen auslesen 
	vec4 outp = texture2D(sampler, texcoord ); 
	outputColor = vec4(0,0,0,1);
	float brightness = dot(outp.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.5)
        outputColor = outp*brightness*0.5;




	
	
}