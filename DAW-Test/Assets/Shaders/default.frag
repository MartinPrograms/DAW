#version 330 core

out vec4 fragColor;
in vec2 TexCoord;

uniform vec4 color;
uniform sampler2D tex;

uniform vec2 resolution;  // Resolution of the screen
uniform vec2 size;        // Size of the square (in pixels)
uniform float aspectRatio; // Aspect ratio (width / height) of the screen
uniform bool flipY;       // Whether to flip the Y axis
uniform bool ignoreAspectRatio; // Whether to ignore the aspect ratio

uniform bool invertClipRect; // Whether to invert the clipping mask
uniform vec4 clipRect; // Clipping mask

void main() {
    vec2 texCoord = TexCoord;

    if (!ignoreAspectRatio) {
        float textureAspectRatio = size.x / size.y;

        if (textureAspectRatio > aspectRatio) {
            texCoord.x *= aspectRatio / textureAspectRatio;
            texCoord.x += (1.0 - aspectRatio / textureAspectRatio) / 2.0;
        } else {
            texCoord.y *= textureAspectRatio / aspectRatio;
            texCoord.y += (1.0 - textureAspectRatio / aspectRatio) / 2.0;
        }
    }

    if (flipY) {
        texCoord.x = 1.0 - texCoord.x;
    }

    vec4 result = texture2D(tex, texCoord);
    if (result.a < 0.1) {
        discard;
    }

    // Apply the clipping mask in screen space
    vec2 screenPos = gl_FragCoord.xy / resolution; // Normalize screen position to [0, 1]

    if (clipRect.x == 0 && clipRect.y == 0 && clipRect.z == 0 && clipRect.w == 0) {
        // No clipping mask, do nothing
    } else if (screenPos.x < clipRect.x || screenPos.x > clipRect.z || screenPos.y < clipRect.y || screenPos.y > clipRect.w) {
        if (!invertClipRect) {
            discard; // Outside the clipping rect, discard the fragment
        }
    } else if (invertClipRect) {
        discard; // Inside the clipping rect but inverted, discard the fragment
    }

    result *= color;

    // Output the final color
    fragColor = result;
}
