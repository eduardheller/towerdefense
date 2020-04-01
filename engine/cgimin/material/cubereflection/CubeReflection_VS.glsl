#version 130
precision highp float;


// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv; 

// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 modelview_projection_matrix;

// "model_matrix" wird als Parameter erwartet, vom Typ Matrix4 
uniform mat4 model_matrix;

// Kamera-Position wird übergeben
uniform vec4 camera_position;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec3 texcoord;

void main()
{
	vec3 v = normalize(model_matrix *  vec4(in_position,1) - camera_position).xyz;
	vec3 normal =  normalize(model_matrix * vec4(in_normal,0)).xyz;

	texcoord = normalize(reflect(v, normal));
	

	gl_Position = modelview_projection_matrix * vec4(in_position, 1);
}


