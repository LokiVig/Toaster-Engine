#pragma once

// C(++) standard
#include <stdio.h>
#include <stdlib.h>
#include <vector>

// OpenGL
#define GLEW_STATIC
#include <GL/glew.h>

// Helps skip putting a GLEW DLL to Sys32
#pragma comment (lib, "glew32s.lib")

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>

// Namespaces
using namespace std;
using namespace glm;

// Toaster Engine
#include "bbox.h"
#include "vertex.h"
#include "entity.h"
#include "brush.h"
#include "scene.h"