#pragma once

#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

class Shader
{
public:
	unsigned int m_id;

	Shader(const char* vertexPath, const char* fragmentPath, const char* geometryPath = nullptr)
	{
		//
		// Retrieve the vertex/fragment source code from the filepath
		//

		string vertexCode;
		string fragmentCode;
		string geometryCode;
		ifstream vShaderFile;
		ifstream fShaderFile;
		ifstream gShaderFile;

		// Ensure ifstream objects can throw exceptions
		vShaderFile.exceptions(ifstream::failbit | ifstream::badbit);
		fShaderFile.exceptions(ifstream::failbit | ifstream::badbit);
		gShaderFile.exceptions(ifstream::failbit | ifstream::badbit);

		try
		{
			// Open files
			vShaderFile.open(vertexPath);
			fShaderFile.open(fragmentPath);
			stringstream vShaderStream, fShaderStream;

			// Read file's buffer contents into streams
			vShaderStream << vShaderFile.rdbuf();
			fShaderStream << fShaderFile.rdbuf();

			// Close file handlers
			vShaderFile.close();
			fShaderFile.close();

			// Convert stream into string
			vertexCode = vShaderStream.str();
			fragmentCode = fShaderStream.str();

			// If geometry shader path is present, also load a geometry shader
			if (geometryPath != nullptr)
			{
				gShaderFile.open(geometryPath);
				stringstream gShaderStream;
				gShaderStream << gShaderFile.rdbuf();
				gShaderFile.close();
				geometryCode = gShaderStream.str();
			}
		}
		catch (ifstream::failure& e)
		{
			printf("Shader::Shader(const char*, const char*, const char*): ERROR; File not successfully read: %s\n", e.what());
		}

		const char* vShaderCode = vertexCode.c_str();
		const char* fShaderCode = fragmentCode.c_str();

		//
		// Compile shaders
		//

		unsigned int vertex, fragment;

		// Vertex shader
		vertex = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertex, 1, &vShaderCode, NULL);
		glCompileShader(vertex);
		CheckCompileErrors(vertex, "VERTEX");

		// Fragment shader
		fragment = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragment, 1, &fShaderCode, NULL);
		glCompileShader(fragment);
		CheckCompileErrors(fragment, "FRAGMENT");

		// If geometry shader is given, compile geometry shader
		unsigned int geometry;

		if (geometryPath != nullptr)
		{
			const char* gShaderCode = geometryCode.c_str();
			geometry = glCreateShader(GL_GEOMETRY_SHADER);
			glShaderSource(geometry, 1, &gShaderCode, NULL);
			glCompileShader(geometry);
			CheckCompileErrors(geometry, "GEOMETRY");
		}

		// Shader program
		m_id = glCreateProgram();
		glAttachShader(m_id, vertex);
		glAttachShader(m_id, fragment);

		if (geometryPath != nullptr)
		{
			glAttachShader(m_id, geometry);
		}

		glLinkProgram(m_id);
		CheckCompileErrors(m_id, "PROGRAM");

		// Delete the shaders as they're linked into our program now and no longer necessary
		glDeleteShader(vertex);
		glDeleteShader(fragment);

		if (geometryPath != nullptr)
		{
			glDeleteShader(geometry);
		}
	}

	// Activate the shader
	void Use()
	{
		// Make sure we aren't a nullptr
		if (!this)
		{
			return;
		}

		glUseProgram(m_id);
	}

	//
	// Utility uniform functions
	//

	void SetBool(const string& name, bool value) const
	{
		glUniform1i(glGetUniformLocation(m_id, name.c_str()), (int)value);
	}

	void SetInt(const string& name, int value) const
	{
		glUniform1i(glGetUniformLocation(m_id, name.c_str()), value);
	}

	void SetFloat(const string& name, float value) const
	{
		glUniform1f(glGetUniformLocation(m_id, name.c_str()), value);
	}

	void SetVec2(const string& name, vec2& value) const
	{
		glUniform2fv(glGetUniformLocation(m_id, name.c_str()), 1, &value[0]);
	}

	void SetVec2(const string& name, float x, float y)const
	{
		glUniform2f(glGetUniformLocation(m_id, name.c_str()), x, y);
	}

	void SetVec3(const string& name, vec3& value) const
	{
		glUniform3fv(glGetUniformLocation(m_id, name.c_str()), 1, &value[0]);
	}

	void SetVec3(const string& name, float x, float y, float z) const
	{
		glUniform3f(glGetUniformLocation(m_id, name.c_str()), x, y, z);
	}

	void SetVec4(const string& name, vec4& value) const
	{
		glUniform4fv(glGetUniformLocation(m_id, name.c_str()), 1, &value[0]);
	}

	void SetVec4(const string& name, float x, float y, float z, float w) const
	{
		glUniform4f(glGetUniformLocation(m_id, name.c_str()), x, y, z, w);
	}

	void SetMat2(const string& name, const mat2& mat) const
	{
		glUniformMatrix2fv(glGetUniformLocation(m_id, name.c_str()), 1, GL_FALSE, &mat[0][0]);
	}

	void SetMat3(const string& name, const mat3& mat) const
	{
		glUniformMatrix3fv(glGetUniformLocation(m_id, name.c_str()), 1, GL_FALSE, &mat[0][0]);
	}

	void SetMat4(const string& name, const mat4& mat) const
	{
		glUniformMatrix4fv(glGetUniformLocation(m_id, name.c_str()), 1, GL_FALSE, &mat[0][0]);
	}

private:
	// Utility function for checking shader compilation/linking errors
	void CheckCompileErrors(GLuint shader, const char* type)
	{
		GLint success;
		GLchar infoLog[1024];

		if (type != "PROGRAM")
		{
			glGetShaderiv(shader, GL_COMPILE_STATUS, &success);

			if (!success)
			{
				glGetShaderInfoLog(shader, 1024, NULL, infoLog);
				printf("Shader::CheckCompileErrors(GLuint, const char*): ERROR; Shader compilation error of type: \"%s\"\n%s\n -- --------------------------------------------------- --\n", type, infoLog);
			}
		}
		else
		{
			glGetProgramiv(shader, GL_LINK_STATUS, &success);

			if (!success)
			{
				glGetProgramInfoLog(shader, 1024, NULL, infoLog);
				printf("Shader::CheckCompileErrors(GLuint, const char*): ERROR; Program linking error of type: \"%s\"\n%s\n -- --------------------------------------------------- --\n", type, infoLog);
			}
		}
	}
};