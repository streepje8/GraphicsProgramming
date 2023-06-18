Shader Default/ProgressiveRefiner [
	
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

		uniform int _Frame;
		uniform vec2 _ScreenSize;
		uniform sampler2D texture0;
		uniform sampler2D texture1;
		
		out vec4 FragColor;
		
		
		
		void main()
		{
			vec2 uv = texCoord;
			uv.y *= -1;
			vec4 oldCol = texture(texture0,uv);
			
			int size = 5;
			float separation = 3;
			float threshold = 0.4;
			float amount = 1;
			
			vec4 newCol = texture(texture1,texCoord);
			float weight = 1.0 / (_Frame + 1);
			FragColor = clamp(oldCol * (1 - weight) + newCol * weight,0.0,1.0);
		}
	]
]