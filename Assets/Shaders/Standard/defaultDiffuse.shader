Shader Default/Diffuse [
	
	Vertex [
		#version 330 core
		in vec3 vPosition;
		in vec2 vTexCoord;
		in vec3 vNormal;
		in vec3 vColor;
		
		
		uniform mat4 model;
		uniform mat4 view;
		uniform mat4 projection;

		out vec3 pos;
		out vec3 color;
		out vec3 normal;
		out vec2 texCoord;

		void main()
		{
			gl_Position = vec4(vPosition, 1.0) * model * view * projection;
			pos = gl_Position.xyz;
			color = vColor;
			texCoord = vTexCoord;
			normal = vNormal;
		}
	]
	
	Fragment [
		#version 330 core
		in vec3 color;
		in vec3 normal;
		in vec3 pos;
		in vec2 texCoord;
		
		
		out vec4 FragColor;

		uniform sampler2D texture0;

		void main()
		{
			FragColor = texture(texture0, texCoord * vec2(1.0f,-1.0f))* vec4(color, 1.0f); //texture(texture0, texCoord)* vec4(vColor, 1.0f)
		}
	]
]