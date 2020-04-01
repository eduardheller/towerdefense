#version 330
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv;

// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 modelview_projection_matrix;

// Die "Model-Matrix" wird als Parameter erwaretet, um die TBN-Matrix in den World-Space zu transformieren
uniform mat4 model_matrix;

uniform mat4 model_view_matrix;

// Kamera-Position wird übergeben
uniform vec4 camera_position;

// Lichtrichtung
uniform vec3 light_direction;

out vec2 fragTexcoord;
out vec3 fragCubeTexCoord;

out vec4 viewPosition;
out float brightness;

void main()
{
	
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht.
	fragTexcoord = in_uv;

	// position übergeben
	vec3 fragV = vec3(-normalize(camera_position - model_matrix *  vec4(in_position,1)));
	fragV.x = -fragV.x;
	fragCubeTexCoord = normalize(reflect(fragV, in_normal));

	brightness = clamp(dot(normalize(in_normal), light_direction), 0, 1);

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	gl_Position = modelview_projection_matrix * vec4(in_position, 1);
	viewPosition = model_view_matrix * vec4(in_position, 1.0);
}


