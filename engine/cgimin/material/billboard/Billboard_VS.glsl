#version 130
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec2 in_uv; 

uniform mat4 projection_matrix;

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 view_matrix;
uniform vec3 center;
uniform vec2 billboard_size;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 fragTexcoord;


void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht.
	fragTexcoord = in_uv;
	
	vec3 cameraright_worldspace = vec3(view_matrix[0][0], view_matrix[1][0], view_matrix[2][0]);
	vec3 cameraup_worldspace = vec3(view_matrix[0][1], view_matrix[1][1], view_matrix[2][1]);

	vec3 vertexPosition_worldspace =
		center + cameraright_worldspace * in_position.x * billboard_size.x
		+ cameraup_worldspace * in_position.y * billboard_size.y;

    gl_Position = projection_matrix * view_matrix * vec4(vertexPosition_worldspace, 1);
}


