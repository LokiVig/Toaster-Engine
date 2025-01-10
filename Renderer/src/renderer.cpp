#include "include/renderer.h"

void Renderer::Initialize()
{
	glewExperimental = true;

	if (glfwInit() == 0)
	{
		(void)fprintf(stderr, "Failed to initialize GLFW\n");
		return;
	}

	glfwWindowHint(GLFW_SAMPLES, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

	m_pWindow = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, m_pszTitle, nullptr, nullptr);

	if (!m_pWindow)
	{
		printf("Renderer::Initialize(): ERROR; Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible.\n");
		glfwTerminate();
		return;
	}

	glfwMakeContextCurrent(m_pWindow);

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

	glfwSetInputMode(m_pWindow, GLFW_STICKY_KEYS, GL_TRUE);
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
	
	Brush debugBrush;
	debugBrush.bbox = BBox(vec3(-15, -15, -15), vec3(15, 15, 15));
	debugBrush.GenerateVertices();

	DrawBrush(debugBrush);

	glfwSwapBuffers(m_pWindow);
	glfwPollEvents();
}

void Renderer::RenderText(const char* text, int x, int y)
{

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