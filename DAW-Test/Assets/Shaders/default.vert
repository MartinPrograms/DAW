#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform vec2 size;
uniform vec3 position;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float aspectRatio;
uniform float far;
uniform float rotation;

uniform int layer;

void main()
{
    vec3 aPos2 = aPos;

    aPos2.x = aPos2.x * size.x + position.x;
    aPos2.y = aPos2.y * -size.y + -position.y;

    aPos2.z = position.z - far / 2.0 + float(-layer) * far / 100.0;

    // Rotation along the center of the sprite
    vec3 center = vec3(position.x, -position.y, position.z);
    center += vec3(size.x / 2.0, -size.y / 2.0, 0.0);
    vec3 rotated = aPos2 - center;
    float s = sin(rotation);
    float c = cos(rotation);
    float xnew = rotated.x * c - rotated.y * s;
    float ynew = rotated.x * s + rotated.y * c;
    aPos2 = vec3(xnew, ynew, rotated.z) + center;

    gl_Position = projection * view * model * vec4(aPos2, 1.0);
    
    TexCoord = aTexCoord;
}