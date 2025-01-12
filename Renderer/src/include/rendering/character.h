#pragma once

struct Character
{
	unsigned int textureID; // ID handle of the glyph texture
	ivec2 size; // Size of glyph
	ivec2 bearing; // Offset from baseline to left/top of glyph
	unsigned int advance; // Offset to advance to next glyph
};