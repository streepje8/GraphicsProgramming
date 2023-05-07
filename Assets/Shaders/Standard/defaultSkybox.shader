Shader Default/Sky [
	
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
		out vec2 fragCoord;

		void main()
		{
			gl_Position = vec4(vPosition, 1.0) * model * view * projection;
			pos = gl_Position.xyz;
			color = vColor;
			fragCoord = vTexCoord;
			normal = (vNormal * mat3(transpose(inverse(model)))).xyz;
		}
	]
	
	Fragment [
		#version 330 core
		in vec3 color;
		in vec3 normal;
		in vec3 pos;
		in vec2 fragCoord;
		
		out vec4 fragColor;
		
		uniform float iTime;
		uniform vec2 iResolution;
		uniform sampler2D texture0;
		uniform vec3 cameraPosition;
		
		vec3 lerp(vec3 a, vec3 b, float t) {
			return a + (b-a) * t;
		}

		void main()
		{
			vec3 topColor = vec3(1);
			vec3 botColor = vec3(0);
			
			vec3 viewDir = normalize(pos - cameraPosition);
			
			fragColor = vec4(lerp(botColor,topColor, viewDir.y), 1);
		}
	]
]