#version 330
precision highp float;

// Input vertex data, different for all executions of this shader.
in vec3 in_position;

// Values that stay constant for the whole mesh.
uniform mat4 modelview_projection_matrix;

void main(){

    // Output position of the vertex, in clip space : MVP * position
    gl_Position =  modelview_projection_matrix * vec4(in_position,1);

}