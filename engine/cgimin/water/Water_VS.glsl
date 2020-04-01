#version 330
precision highp float;

// input aus der VAO-Datenstruktur
in vec3 in_position;
in vec2 in_uv;

// "modelview_projection_matrix" wird als Parameter erwartet, vom Typ Matrix4
uniform mat4 modelview_projection_matrix;
uniform vec3 camera_position;
uniform mat4 model;
uniform float time;
uniform mat4 DepthBiasMVP;

out vec4 ShadowCoord;
out vec4 clipSpace;
out vec2 fragTexCoords;
out vec3 cameraDir;
out vec3 lightDir;
out vec3 cameraSubPos;
out vec4 viewPosition;

const float tiling = 0.2;
const float amplitude = 0.1;
const float waveLength = 1.1;
const float speed = 15.0;

float getWave(float amplitude, float waveLength, float speed, float t, vec3 pos, vec2 dir)
{
	float w = 3.14159*2/waveLength;
	float phi = speed * w;
	
	return amplitude * sin(dot(dir,vec2(pos.x,pos.z))*w+t*phi);
}

void main()
{
	vec3 pos = in_position;
	vec4 worldPos = model * vec4(pos, 1);
	
	float h1 = getWave(0.1,1.1,22.0,time,worldPos.xyz,vec2(0.2,0.5));
	float h2 = getWave(0.1,1.1,12.0,time,worldPos.xyz,vec2(0.5,0.2));
	float h3 = getWave(0.2,2.1,32.0,time,worldPos.xyz,vec2(0.3,0.2));
	float h4 = getWave(0.3,3.1,22.0,time,worldPos.xyz,vec2(0.1,0.1));
	pos.y = h1+h2+h3+h4+0.0f;

	worldPos = model * vec4(pos, 1);

	ShadowCoord = DepthBiasMVP * vec4(pos,1);
	clipSpace = modelview_projection_matrix * vec4(pos, 1);
	gl_Position = clipSpace;
	
	viewPosition = gl_Position;

	cameraSubPos = camera_position;
	fragTexCoords = in_uv * tiling;
	cameraDir = camera_position - worldPos.xyz;

}


