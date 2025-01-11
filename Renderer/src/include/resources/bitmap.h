#pragma once

#include <cstdio>
#include <cstdlib>
#include <type_traits>

#include <GL/glew.h>

using namespace std;

struct Bitmap
{
	int width;
	int height;
	unsigned char* data;
};

Bitmap Bitmap_Load(const char* filepath);
GLuint Bitmap_CreateTexture(const Bitmap& bitmap);
void Bitmap_GetCharTexCoords(char c, float& u1, float& v1, float& u2, float& v2);