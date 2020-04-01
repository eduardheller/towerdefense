#version 330 core
precision highp float;

//layout(location = 0) in vec3 squareVertices;
//layout(location = 1) in vec4 xyzs; // Position of the center of the particule and size of the square

attribute vec3 squareVertices;
attribute vec4 xyzs;



// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 projection_matrix;

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_view_matrix;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 fragTexcoord;

void main()
{

	vec3 pos = xyzs.xyz;

	pos+= squareVertices;
	
	
	fragTexcoord = squareVertices.xy + vec2(0.5, 0.5);

    //gl_Position = projection_matrix * (model_view_matrix * vec4(0.0, 0.0, 0.0, 1.0) + vec4(pos, 0.0));

	gl_Position = projection_matrix * model_view_matrix * vec4(pos, 1.0);

}
