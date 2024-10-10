# A crossplatform C# Daw  
This project consists of a few components.  
-Audio Engine  
-Common code  
-The DAW itself  
-UI library  
-External audio unit hosts  

The audio engine is made using [PortAudio](https://github.com/csukuangfj/PortAudioSharp2) and handles processing audio. Currently it only supports playing samples, volume, panning and pitching of channels and the playback of.  
The common code currently consists of a simple easing library and some math extensions.  
The daw combines the audio engine, and ui library to create an interface useable by humans to create/modify/export audio.  
The drawing library draws stuff to the screen, it does so using OpenTK & OpenGL 3.3, for cross platform support.  
The Hosts library currently does not do anything, but will eventually be able to host plugins, a custom plugin library specifically for this project will also be made later.  

The main focus of this project is cross platform music production with as little as possible native dependencies.
