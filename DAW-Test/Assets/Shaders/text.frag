#version 330 core

out vec4 fragColor;
in vec2 TexCoord;
// TEXT SHADER Fragment Shader

uniform sampler2D tex;

uniform vec4 color;

void main()
{
    vec4 texColor = texture(tex, TexCoord);

    texColor.a = pow(texColor.a, 2.2);

    if (texColor.a < 0.1) // very important to discard transparent pixels
    {
        discard;
    }
    
    fragColor = texColor * color;
}