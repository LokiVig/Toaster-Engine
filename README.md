# Toaster Engine
A game engine designed mostly in C#, intended to have Source engine- and Quake/DOOM-esque aspects. This is mostly a passion project, on and off updates will occur and major issues will be pushed.

## About
Inspired by Source, Quake and DOOM, I thought to myself "why not make my own engine?", and, since it's the language I'm most proficient with, I'm doing it mostly using C#. \
It most likely means the engine won't be as optimized as one made in, e.g. C++ would be, and such an example is the lack of an efficient C# graphics library, resorting me to use C++ for the engine's rendering system.

## Building
The provided solution file should be all you need to compile using VS22 or Rider, but the Renderer does need its own set of header and .lib files.

The files can be downloaded from [this .zip file.](https://cdn.discordapp.com/attachments/753315536791666798/1328106346121396264/RendererDependencies.zip?ex=67857eeb&is=67842d6b&hm=e39509455b4d26d7d1bc7db37c37bb484c882cba9fc0b58e1d2f1973b8808608&) Placing them in `Toaster-Engine/Renderer/` should be everything you need to do.
