#version 130
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv; 

out vec2 texcoord;

uniform mat4 model;

void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht
	texcoord = in_uv;

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	gl_Position = model * vec4(in_position, 1);
}


