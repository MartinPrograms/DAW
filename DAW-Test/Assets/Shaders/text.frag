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


    // Apply the clipping mask in screen space
    vec2 screenPos = gl_FragCoord.xy;
    screenPos.y = resolution.y - screenPos.y; // Flip Y axis, because 0,0 is top-left in screen

    vec4 rect = clipRect;


    if (rect.x == 0 && rect.y == 0 && rect.z == 0 && rect.w == 0) {
        // No clipping mask
    }else

    if (!invertClipRect) {
        if (screenPos.x >= rect.x && screenPos.x <= rect.z && screenPos.y >= rect.y && screenPos.y <= rect.w) {
            discard;
        }
    }
    else {
        if (screenPos.x < rect.x || screenPos.x > rect.z || screenPos.y < rect.y || screenPos.y > rect.w) {
            discard;
        }
    }
    

    fragColor = texColor * color;
}