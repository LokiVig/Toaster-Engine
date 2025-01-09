# Toaster Engine
A game engine designed mostly in C#, intended to have Source engine- and Quake/DOOM-esque aspects. This is mostly a passion project, on and off updates will occur and major issues will be pushed.

## About
Inspired by Source, Quake and DOOM, I thought to myself "why not make my own engine?", and, since it's the language I'm most proficient with, I'm doing it mostly using C#. \
It most likely means the engine won't be as optimized as one made in, e.g. C++ would be, and such an example is the lack of an efficient C# graphics library, resorting me to use C++ for the engine's rendering system.

## Building
The provided solution file should be all you need to compile using VS22 or Rider, but the Renderer does need its own set of header and .lib files.

The dependencies needed are: \
[GLFW](https://github.com/glfw/glfw/releases/download/3.4/glfw-3.4.zip) \
[GLM](https://github.com/g-truc/glm) \
[GLEW](https://sourceforge.net/projects/glew/files/glew/2.1.0/glew-2.1.0-win32.zip/download)

They should be placed in their respective folders, include and library files from GLFW should be featured as:\
`include/glfw`\
`lib/glfw3.lib`

For glm:\
`include/glm`

For GLEW:\
`include/GL`\
`lib/glew32.lib` (from lib/Release/x64/)
