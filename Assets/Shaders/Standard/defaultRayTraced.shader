Shader Default/RayTraced [
	
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
		
		uniform vec3 _CamPos;
		uniform vec3 _ViewParams;
		uniform mat4 _CamLocalToWorldMatrix;
		
		out vec4 FragColor;
		
		//Data structures
		struct Ray {
			vec3 origin;
			vec3 dir;
		};
		
		struct HitInfo {
			bool didHit;
			float dst;
			vec3 hitPoint;
			vec3 normal;
		};
		
		
		//Intersection Functions
		HitInfo HitSphere(Ray ray, vec3 center, float radius) {
			HitInfo info;
			vec3 offset = ray.origin - center;
			
			//ABC formule!!! HYPE
			float a = dot(ray.dir,ray.dir);
			float b = 2 * dot(offset, ray.dir);
			float c = dot(offset,offset)-radius * radius;
			float d = b * b - 4 * a * c;
			if(d >= 0) {
				float dst = (-b -sqrt(d)) / (2 * a);
				
				if(dst >= 0) { //Make sure we do not collide behind the camera
					info.didHit = true;
					info.dst = dst;
					info.hitPoint = ray.origin + ray.dir * dst;
					info.normal = normalize(info.hitPoint - center);
				}
			}
			return info;
		}
		
		
		void main()
		{
			vec3 viewPointLocal = vec3(texCoord- 0.5, 1) * _ViewParams;
			viewPointLocal.y *= -1;
			viewPointLocal.z *= -1;
			vec3 viewPoint = (vec4(viewPointLocal, 1) * _CamLocalToWorldMatrix).xyz;
			
			Ray ray;
			ray.origin = _CamPos;
			ray.dir = normalize(viewPoint - ray.origin);
			vec4 outcol = vec4(ray.dir,1);
			HitInfo hit = HitSphere(ray, vec3(0,0,-4), 1.0);
			if(hit.didHit) outcol = vec4(hit.normal,1);
			FragColor = outcol;
		}
	]
]