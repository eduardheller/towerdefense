#version 330
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv; 

// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 modelview_projection_matrix;

// Kamera-Position wird �bergeben
uniform vec4 camera_position;

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 fragTexcoord;

// die Normale wird ebenfalls an den Fragment-Shader �bergeben
out vec3 fragNormal;
out vec4 fragV;

void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht.
	fragTexcoord = in_uv;

	// die Normale wird an den Fragment-Shader gegeben.
	fragNormal = in_normal;

	// position �bergeben
	fragV = normalize(camera_position - model_matrix *  vec4(in_position,1));

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	gl_Position = modelview_projection_matrix * vec4(in_position, 1);
}


