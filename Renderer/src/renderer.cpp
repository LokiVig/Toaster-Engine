#include "include/renderer.h"

// STB Image
#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

void Renderer::Initialize()
{
	glewExperimental = true;

	if (glfwInit() == 0)
	{
		printf("Renderer::Initialize(): ERROR; Failed to initialize GLFW\n");
		return;
	}

	// Set hints for our window
	glfwWindowHint(GLFW_SAMPLES, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

	// Create the window
	m_pWindow = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, m_pszTitle, nullptr, nullptr);

	if (!m_pWindow)
	{
		printf("Renderer::Initialize(): ERROR; Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible.\n");
		glfwTerminate();
		return;
	}

	// Initialize a specific font to use
	/*if (!InitializeFont("resources/fonts/consola.ttf", 24))
	{
		glfwTerminate();
		return;
	}*/

	// We should immediately focus on the newly created window
	glfwMakeContextCurrent(m_pWindow);

	// Load the icon
	GLFWimage images[2];
	images[0].pixels = stbi_load("resources/textures/engine/toastengine_icon.png", &images[0].width, &images[0].height, 0, 4); // Regular icon
	images[1].pixels = stbi_load("resources/textures/engine/toastengine_icon_small.png", &images[1].width, &images[1].height, 0, 4); // Small icon
	glfwSetWindowIcon(m_pWindow, 2, images);
	stbi_image_free(images[0].pixels); // Free icon
	stbi_image_free(images[1].pixels); // Free small icon

	// Make sure GLEW initializes correctly
	if (glewInit() != GLEW_OK)
	{
		printf("Renderer::Initialize(): ERROR; Failed to initialize GLEW.\n");
		glfwTerminate();
		return;
	}

	// OpenGL states
	glEnable(GL_CULL_FACE);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	// Allow for keyboard input
	glfwSetInputMode(m_pWindow, GLFW_STICKY_KEYS, GL_TRUE);
}

int Renderer::InitializeFont(const char* filepath, int pixelFontSize)
{
	if (FT_Init_FreeType(&m_ftLibrary))
	{
		printf("Renderer::Initialize(const char*, int): ERROR; Could not initialize FreeType library.\n");
		return 0;
	}

	if (FT_New_Face(m_ftLibrary, filepath, 0, &m_ftFace))
	{
		printf("Renderer::InitializeFont(const char*, int): ERROR; Failed to load font: \"%s\"\n", filepath);
		return 0;
	}

	FT_Set_Pixel_Sizes(m_ftFace, 0, pixelFontSize);
	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

	for (unsigned char c = 0; c < 128; c++)
	{
		if (FT_Load_Char(m_ftFace, c, FT_LOAD_RENDER))
		{
			printf("Renderer::InitializeFont(const char*, int): ERROR; Failed to load Glyph '%c'\n", c);
			continue;
		}

		unsigned int texture;
		glGenTextures(1, &texture);
		glBindTexture(GL_TEXTURE_2D, texture);
		glTexImage2D(
			GL_TEXTURE_2D, 
			0, 
			GL_RED,
			m_ftFace->glyph->bitmap.width,
			m_ftFace->glyph->bitmap.rows,
			0,
			GL_RED, 
			GL_UNSIGNED_BYTE, 
			m_ftFace->glyph->bitmap.buffer
		);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		Character character =
		{
			texture,
			ivec2(m_ftFace->glyph->bitmap.width, m_ftFace->glyph->bitmap.rows),
			ivec2(m_ftFace->glyph->bitmap_left, m_ftFace->glyph->bitmap_top),
			static_cast<unsigned int>(m_ftFace->glyph->advance.x)
		};
		m_characters.insert(pair(c, character));
	}

	glBindTexture(GL_TEXTURE_2D, 0);
	FT_Done_Face(m_ftFace);
	FT_Done_FreeType(m_ftLibrary);

	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 6 * 4, NULL, GL_DYNAMIC_DRAW);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 4, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 0);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);

	return 1;
}

void Renderer::SetScene(Scene* pScene)
{
	m_pScene = pScene;
}

void Renderer::Render()
{
	// Whenever we get told to shut down, call the shutdown method
	if (glfwGetKey(m_pWindow, GLFW_KEY_ESCAPE) == GLFW_PRESS || glfwWindowShouldClose(m_pWindow))
	{
		Shutdown();
		return;
	}

	// Set the clear color (blue-ish)
	glClearColor(0.25f, 0.25f, 0.75f, 1.0f);

	// Clear the screen
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	if (m_pScene)
	{
		for (const Brush brush : m_pScene->m_brushes)
		{
			// Draw brushes
			DrawBrush(brush);
		}

		for (const Entity entity : m_pScene->m_entities)
		{
			// Draw entities
			DrawEntity(entity);
		}
	}

	/*Brush debugBrush;
	debugBrush.bbox = BBox(vec3(-15, -15, -15), vec3(15, 15, 15));
	debugBrush.GenerateVertices();

	DrawBrush(debugBrush);*/

	// Swap buffers to display what we've rendered
	// Also poll events to allow for window interactions
	glfwSwapBuffers(m_pWindow);
	glfwPollEvents();
}

void Renderer::RenderText(const char* text, float x, float y, float scale, float r, float g, float b)
{
	Shader shader("resources/shaders/text.vert", "resources/shaders/text.frag");
	shader.Use();
	glUniform3f(glGetUniformLocation(shader.m_id, "textColor"), r, g, b);
	glActiveTexture(GL_TEXTURE0);
	glBindVertexArray(VAO);

	for (int i = 0; i < strlen(text); i++)
	{
		Character ch = m_characters[text[i]];

		float xpos = x + ch.bearing.x * scale;
		float ypos = y - (ch.size.y - ch.bearing.y) * scale;

		float w = ch.size.x * scale;
		float h = ch.size.y * scale;

		float vertices[6][4] =
		{
			{xpos,     ypos + h, 0.0f, 0.0f},
			{xpos,     ypos,     0.0f, 1.0f},
			{xpos + w, ypos,     1.0f, 1.0f},

			{xpos,     ypos + h, 0.0f, 0.0f},
			{xpos + w, ypos,     1.0f, 1.0f},
			{xpos + w, ypos + h, 1.0f, 0.0f},
		};

		glBindTexture(GL_TEXTURE_2D, ch.textureID);

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices);
		glBindBuffer(GL_ARRAY_BUFFER, 0);

		glDrawArrays(GL_TRIANGLES, 0, 6);
		x += (ch.advance >> 6) * scale;
	}

	glBindVertexArray(0);
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Renderer::RenderText3D(const char* text, float x, float y, float z)
{

}

void Renderer::DrawBrush(const Brush brush)
{
	// Initialize buffers and arrays
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &EBO);

	glBindVertexArray(VAO);

	vector<unsigned int> indices =
	{
		0, 1, 2, 0, 2, 3, // Front face
		4, 5, 6, 4, 6, 7, // Back face
		4, 0, 3, 4, 3, 7, // Left face
		1, 5, 6, 1, 6, 2, // Right face
		3, 2, 6, 3, 6, 7, // Top face
		0, 1, 5, 0, 5, 4, // Bottom face
	};

	// Vertex buffer
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, brush.vertices.size() * sizeof(Vertex), brush.vertices.data(), GL_STATIC_DRAW);

	// Element buffer
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), indices.data(), GL_STATIC_DRAW);

	// Position attribute
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
	glEnableVertexAttribArray(0);

	// UV attribute
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_texCoords));
	glEnableVertexAttribArray(1);

	// Unbind VAO
	glBindVertexArray(0);

	// Render the brush
	glBindVertexArray(VAO);
	glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
	glBindVertexArray(0);

	// Unbind buffers
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
}

void Renderer::DrawEntity(const Entity entity)
{

}

void Renderer::Shutdown()
{
	// Ensure the window is destroyed if it exists
	if (m_pWindow)
	{
		glfwDestroyWindow(m_pWindow);
		m_pWindow = nullptr; // Prevent dangling pointer
	}

	// Terminate GLFW to release any allocated resources
	glfwTerminate();

	// Tell our C# game that we're shutting down
	m_shuttingDown = true;
}

bool Renderer::ShuttingDown()
{
	return m_shuttingDown;
}