﻿#version 330
precision highp float;

// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform vec4 pickingColor;

void main(){

    color = pickingColor;

}