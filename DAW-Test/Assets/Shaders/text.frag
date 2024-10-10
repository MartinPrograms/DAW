#version 330 core

out vec4 fragColor;
in vec2 TexCoord;
// TEXT SHADER Fragment Shader

uniform sampler2D tex;

uniform vec4 clipRect;
uniform bool invertClipRect;
uniform vec4 color;
uniform vec2 resolution;

void main()
{
    vec4 texColor = texture(tex, TexCoord);

    texColor.a = pow(texColor.a, 2.2);

    if (texColor.a < 0.1) // very important to discard transparent pixels
    {
        discard;
    }

    vec2 maskCoord = gl_FragCoord.xy / resolution;
    if (clipRect.x == 0 && clipRect.y == 0 && clipRect.z == 0 && clipRect.w == 0) // if no clipping mask
    {

    }else 
    if (maskCoord.x < clipRect.x || maskCoord.x > clipRect.z || maskCoord.y < clipRect.y || maskCoord.y > clipRect.w)
    {
        if (!invertClipRect)
        {
            fragColor = vec4(0, 255, 0, 255);
        
        return;}
    }

    else if (invertClipRect)
    {
        fragColor = vec4(0, 255, 0, 255);
        return;
    }

    fragColor = texColor * color;
}