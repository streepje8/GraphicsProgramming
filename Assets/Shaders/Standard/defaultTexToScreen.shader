Shader Default/TexToScreen [
	
	Vertex [
		#version 330 core
		in vec3 vPosition;
		in vec2 vTexCoord;
		in vec3 vColor;
		
		
		uniform mat4 model;

		out vec3 pos;
		out vec3 color;
		out vec2 texCoord;

		void main()
		{
			gl_Position = vec4(vPosition, 1.0) * model;
			pos = (vec4(vPosition, 1.0) * model).xyz;
			color = vColor;
			texCoord = vTexCoord;
		}
	]
	
	Fragment [
		#version 330 core
		
		in vec3 color;
		in vec3 pos;
		in vec2 texCoord;

		uniform sampler2D texture0;
		
		out vec4 FragColor;
		
		
		
		void main()
		{
			FragColor = texture(texture0,texCoord);
		}
	]
]