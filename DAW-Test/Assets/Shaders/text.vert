#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

// TEXT SHADER Vertex Shader

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform float aspectRatio;

uniform int layer;

uniform vec3 position;
uniform float far;
uniform vec2 size;

out vec2 TexCoord;

void main()
{
    // This is exclusively a 2D renderer, the Z coordinate will act as the layer
    vec3 aPos2D = vec3(aPos.xy, float(-layer));

    aPos2D.x = aPos2D.x * size.x + position.x;
    aPos2D.y = aPos2D.y * -size.y + -position.y;
    aPos2D.z = position.z - far / 2.0 + float(-layer) * far / 100.0;

    // The position is the center of the sprite
    
    mat4 proj = projection;
    gl_Position = proj * view * model * vec4(aPos2D, 1.0);
    
    vec2 coords = vec2(1.0 - aTexCoord.y,  aTexCoord.x); // Flip the texture coordinates
    TexCoord = coords;
}