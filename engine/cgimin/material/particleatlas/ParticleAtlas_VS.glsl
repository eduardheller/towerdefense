#version 130
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec2 in_uv; 

// instance member
in vec2 in_texoffset1;
in vec2 in_texoffset2;
in vec3 in_center;
in vec2 in_billboard_size;
in float in_blend;
in float in_angle;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform int rows;
uniform int columns;

// "texcoord" wird an den Fragment-Shader weitergegeben, daher als "out" deklariert
out vec2 tex_offset1;
out vec2 tex_offset2;
out float blend;

mat4 rotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}


void main()
{
	vec2 textureCoords = in_uv;

	mat4 model = rotationMatrix(vec3(0,0,1),in_angle);

	vec4 pos = model * vec4(in_position,1);

	textureCoords.x /= columns;
	textureCoords.y = 1.0 - textureCoords.y;
	textureCoords.y /= rows;
	
	tex_offset1 = textureCoords + in_texoffset1;
	tex_offset2 = textureCoords + in_texoffset2;
	blend = in_blend;
	
	vec3 cameraright_worldspace = vec3(view_matrix[0][0], view_matrix[1][0], view_matrix[2][0]);
	vec3 cameraup_worldspace = vec3(view_matrix[0][1], view_matrix[1][1], view_matrix[2][1]);

	vec3 vertexPosition_worldspace =
		in_center + cameraright_worldspace * pos.x * in_billboard_size.x
		+ cameraup_worldspace * pos.y * in_billboard_size.y;


    gl_Position = projection_matrix * view_matrix * vec4(vertexPosition_worldspace, 1);
}


