Shader Default/Error [
	
	Vertex [
		#version 330 core
		layout (location = 0) in vec3 aPosition;
		layout (location = 1) in vec3 color;
		
		uniform mat4 transform;

		out vec3 vColor;

		void main()
		{
			gl_Position = vec4(aPosition, 1.0) * transform;
			vColor = color;
		}
	]
	
	Fragment [
		#version 330 core
		out vec4 FragColor;

		void main()
		{
			FragColor = vec4(1.0f,0.0f,1.0f, 1.0f);
		}
	]
]