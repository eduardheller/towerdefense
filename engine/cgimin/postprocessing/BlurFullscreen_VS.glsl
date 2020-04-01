#version 130
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv; 

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 texcoord;

void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht
	texcoord = in_uv;

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	//gl_Position = modelview_projection_matrix * vec4(in_position, 1);

	gl_Position = vec4(in_position, 1);
}


