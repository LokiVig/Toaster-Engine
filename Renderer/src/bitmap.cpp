#include "include/resources/bitmap.h"

Bitmap Bitmap_Load(const char* filepath)
{
	FILE* file = nullptr;
	fopen_s(&file, filepath, "rb");

	if (!file)
	{
		printf("LoadBMP(const char*): ERROR; Failed to open BMP file: \"%s\".\n", filepath);
		exit(-1);
	}

	unsigned char header[54];
	fread(header, sizeof(unsigned char), 54, file);

	int width = *(int*)&header[18];
	int height = *(int*)&header[22];
	int dataOffset = *(int*)&header[10];

	fseek(file, dataOffset, SEEK_SET);

	int dataSize = width * height * 3;
	unsigned char* data = new unsigned char[dataSize];
	fread(data, sizeof(unsigned char), dataSize, file);

	fclose(file);

	// Flip image vertically (BMP is stored upside-down)
	/*for (int i = 0; i < height / 2; i++)
	{
		for (int j = 0; j < width * 3; j++)
		{
			swap(data[i * width * 3 + j], data[(height - 1 - i) * width * 3 + j]);
		}
	}*/

	return { width, height, data };
}

GLuint Bitmap_CreateTexture(const Bitmap& bitmap)
{
	GLuint texture;
	glGenTextures(1, &texture);
	glBindTexture(GL_TEXTURE_2D, texture);

	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, bitmap.width, bitmap.height, 0, GL_BGR, GL_UNSIGNED_BYTE, bitmap.data);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

	delete[] bitmap.data; // Free raw pixel data

	return texture;
}

void Bitmap_GetCharTexCoords(char c, float& u1, float& v1, float& u2, float& v2)
{
	const int gridCols = 16; // Number of columns in the font texture
	const int gridRows = 16; // Number of rows in the font texture

	int index = c - 32; // ASCII offset (starts at 32 for printable characters)
	int col = index % gridCols;
	int row = index / gridCols;

	float cellWidth = 1.0f / gridCols;
	float cellHeight = 1.0f / gridRows;

	u1 = col * cellWidth;
	v1 = 1.0f - (row + 1) * cellHeight; // Flip vertically for OpenGL
	u2 = (col + 1) * cellWidth;
	v2 = 1.0f - row * cellHeight;
}