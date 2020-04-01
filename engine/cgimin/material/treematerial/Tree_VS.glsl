#version 330
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv; 


// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 modelview_projection_matrix;

// Kamera-Position wird übergeben
uniform vec4 camera_position;

// "model_matrix" Matrix wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 model_matrix;


uniform vec4 clipping_vector;

uniform float time;
uniform float animTime;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 fragTexcoord;

// die Normale wird ebenfalls an den Fragment-Shader übergeben
out vec3 fragNormal;
out vec4 fragV;

void main()
{
	vec3 newVertex;

	newVertex.x = in_position.x;
	newVertex.y = in_position.y;
	newVertex.z = in_position.z;


	if (in_position.y > 0) {
		newVertex.x = in_position.x + 0.1 * in_position.y * sin(((animTime+time)/5) + 0.5);
		newVertex.y = in_position.y;
		newVertex.z = in_position.z + 0.1 * in_position.y * sin(((animTime+time)/5)+ 0.5);
	}
	

	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht.
	fragTexcoord = in_uv;

	vec4 worldPos = model_matrix * vec4(newVertex,1);

	gl_ClipDistance[0] = dot(worldPos,clipping_vector);

	// die Normale wird an den Fragment-Shader gegeben.
	fragNormal = in_normal;

	// position übergeben
	fragV = normalize(camera_position - model_matrix *  vec4(newVertex,1));

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	gl_Position = modelview_projection_matrix * vec4(newVertex, 1);
}


