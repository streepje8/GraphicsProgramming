Shader Default/Lit [
	
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
			pos = (vec4(vPosition, 1.0) * model).xyz;
			color = vColor;
			texCoord = vTexCoord;
			normal = (vNormal * mat3(transpose(inverse(model)))).xyz;
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
			vec3 lightDir = vec3(1,1,1);
			float light = clamp((dot(normalize(normal),lightDir) + 1) / 2,0.2,1); //Ambient + diffuse
			vec4 outputCol = texture(texture0, texCoord) * vec4(light);
			if(dot(normalize(normal),lightDir) > 1.95) FragColor = vec4(1); //Specular
			else FragColor = outputCol;
		}
	]
]