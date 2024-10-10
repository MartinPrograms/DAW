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

uniform vec4 clippingMask; // Clipping mask

void main() {
    vec2 texCoord = TexCoord;

    if(!ignoreAspectRatio) {
        float textureAspectRatio = size.x / size.y;

        if(textureAspectRatio > aspectRatio) {
            texCoord.x *= aspectRatio / textureAspectRatio;
            texCoord.x += (1.0 - aspectRatio / textureAspectRatio) / 2.0;
        } else {
            texCoord.y *= textureAspectRatio / aspectRatio;
            texCoord.y += (1.0 - textureAspectRatio / aspectRatio) / 2.0;
        }
    }

    if(flipY) {
        texCoord.x = 1.0 - texCoord.x;
    }

    vec4 result = texture2D(tex, texCoord);
    if(result.a < 0.1) {
        discard;
    }

    // Apply the clipping mask, this is a screen-space mask in screen coordinates, so we need to convert it to texture coordinates
    vec2 maskCoord = gl_FragCoord.xy / resolution;

    if(maskCoord.x < clippingMask.x || maskCoord.x > clippingMask.z || maskCoord.y < clippingMask.y || maskCoord.y > clippingMask.w) {
        discard;
    }

    result = result * color;

    // Output the final color
    fragColor = result;
}
