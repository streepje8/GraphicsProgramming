Shader Custom/Terrain [
	
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
		
		uniform sampler2D texture0;
		
		

		void main()
		{
			gl_Position = vec4(vPosition + vec3(0,texture(texture0,vTexCoord).r * 30,0),1.0) * model * view * projection; //
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
		
		uniform sampler2D texture1;

		void main()
		{
			vec3 lightDir = vec3(-1,-1,-1);
			vec3 normalmapnormal = texture(texture1, texCoord).rbg * 2 - 1;
			float light = clamp((-dot(normalize(normalmapnormal),lightDir) + 1) / 2,0.4,1); //Ambient + diffuse
			//if(dot(normalize(normalmapnormal),lightDir) > 0.95) FragColor = vec4(1); //Specular
			FragColor = vec4(light); //
		}
	]
]