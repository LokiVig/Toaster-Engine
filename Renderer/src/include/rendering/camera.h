#pragma once

class Camera
{
public:
	Camera(vec3 position, vec3 target)
		: m_position(position), m_target(target)
	{
		m_direction = normalize(m_position - m_target);
		m_cameraRight = normalize(cross(m_up, m_direction));
		m_cameraUp = normalize(cross(m_direction, m_cameraRight));
		m_view = lookAt(vec3(0.0f, 0.0f, 3.0f),
						vec3(0.0f, 0.0f, 0.0f),
						vec3(0.0f, 1.0f, 0.0f));

	}

public:
	vec3 m_position;
	vec3 m_target;
	vec3 m_direction;
	vec3 m_cameraRight;
	vec3 m_cameraUp;

	mat4 m_view;

private:
	vec3 m_up = vec3(0.0f, 0.0f, 1.0f);
};