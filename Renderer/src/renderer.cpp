#include "include/renderer.h"

// STB Image
#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

void Renderer::Initialize()
{
	glewExperimental = true;

	if (glfwInit() == 0)
	{
		(void)fprintf(stderr, "Failed to initialize GLFW\n");
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

	// Initialize buffers and arrays
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &EBO);

	glBindVertexArray(VAO);

	// Allow for keyboard input
	glfwSetInputMode(m_pWindow, GLFW_STICKY_KEYS, GL_TRUE);

	// Load the bitmap font for text rendering
	Bitmap fontBitmap = Bitmap_Load("resources/textures/engine/consolas.bmp");
	m_fontTexture = Bitmap_CreateTexture(fontBitmap);
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

	glClearColor(0.25f, 0.25f, 0.75f, 1.0f);

	glClear(GL_COLOR_BUFFER_BIT);

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

	RenderText("Holy fuck, toaster in an engine", 700, 800);
	
	/*Brush debugBrush;
	debugBrush.bbox = BBox(vec3(-15, -15, -15), vec3(15, 15, 15));
	debugBrush.GenerateVertices();

	DrawBrush(debugBrush);*/

	glfwSwapBuffers(m_pWindow);
	glfwPollEvents();
}

void Renderer::RenderText(const char* text, int x, int y)
{
	glBindTexture(GL_TEXTURE_2D, m_fontTexture);

	float scale = 32.0f; // Character size in pixels
	float xOffset = x;

	for (const char* c = text; *c != '\0'; ++c)
	{
		if (*c < 32 || *c > 126)
		{
			continue;
		}

		float u1, v1, u2, v2;
		Bitmap_GetCharTexCoords(*c, u1, v1, u2, v2);

		float vertices[] =
		{
			xOffset,         y,         u1, v2,
			xOffset + scale, y,         u2, v2,
			xOffset + scale, y + scale, u2, v1,
			xOffset,         y + scale, u1, v1
		};

		unsigned int indices[] =
		{
			0, 1, 2, 0, 2, 3
		};

		glBindBuffer(GL_ARRAY_BUFFER, VBO);
		glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

		glEnableVertexAttribArray(0);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (void*)0);

		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), (void*)(2 * sizeof(float)));

		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

		xOffset += scale; // Move to the next character
	}

	glBindTexture(GL_TEXTURE_2D, 0);
}

void Renderer::RenderText3D(const char* text, float x, float y, float z)
{

}

void Renderer::DrawBrush(const Brush brush)
{
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
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, uv));
	glEnableVertexAttribArray(1);

	// Unbind VAO
	glBindVertexArray(0);

	// Render the brush
	glBindVertexArray(VAO);
	glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
	glBindVertexArray(0);
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