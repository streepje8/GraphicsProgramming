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
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			
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
		
		//Made using https://www.shadertoy.com/view/wtSyRd
		HitInfo HitBox(Ray ray, vec3 center, vec3 minPos, vec3 maxPos) {
			HitInfo info;
			vec3 offset = ray.origin - center;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			
			
			vec3 inverse_dir = 1.0 / ray.dir;
			vec3 tbot = inverse_dir * (minPos - offset);
			vec3 ttop = inverse_dir * (maxPos - offset);
			vec3 tmin = min(ttop, tbot);
			vec3 tmax = max(ttop, tbot);
			vec2 traverse = max(tmin.xx, tmin.yz);
			float traverselow = max(traverse.x, traverse.y);
			traverse = min(tmax.xx, tmax.yz);
			float traversehi = min(traverse.x, traverse.y);
			vec3 box = vec3(float(traversehi > max(traverselow, 0.0)), traversehi, traverselow);
			float is_box_hit = box.x;
			float box_t_max = box.y;
			float box_t_min = box.z;
			vec3 boxctr = (minPos + maxPos) / 2.0;
			vec3 box_hit = boxctr - (offset + (box_t_min * ray.dir));
			box_hit = -box_hit;
			vec3 box_intersect_normal = box_hit / max(max(abs(box_hit.x), abs(box_hit.y)), abs(box_hit.z));
			box_intersect_normal = clamp(box_intersect_normal, vec3(0.0,0.0,0.0), vec3(1.0,1.0,1.0));
			box_intersect_normal = normalize(floor(box_intersect_normal * 1.0000001));
			info.normal = box_intersect_normal;
			info.hitPoint = box_hit;
			info.didHit = box.x > 0;
			info.dst = distance(ray.origin,box_hit);
			return info;
		}
		
		//Stolen from https://cmichel.io/howto-raytracer-ray-plane-intersection-theory
		HitInfo HitPlane(Ray ray, vec3 center, vec3 normal)
		{
			HitInfo info;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);

	
			float denominator = dot(ray.dir, normal);

			if (abs(denominator) < 1e-6) return info;      // direction and plane parallel, no intersection

			float t = dot(center - ray.origin, normal) / denominator;
			if (t < 0) return info;    // plane behind ray's origin

			info.dst = t;
			info.hitPoint = ray.origin + ray.dir * t;
			info.normal = normal;
			info.didHit = true;
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
			vec4 outcol = vec4(normalize(ray.dir),1);
			HitInfo hitThree = HitPlane(ray, vec3(0,0,0), vec3(0,1,0));
			if(hitThree.didHit) outcol = vec4(hitThree.normal,1);
			HitInfo hit = HitBox(ray, vec3(0,0,-4), vec3(-0.5), vec3(0.5));
			if(hit.didHit) outcol = vec4(hit.normal,1);
			HitInfo hitTwo = HitSphere(ray, vec3(4,0,-4), 1);
			if(hitTwo.didHit) outcol = vec4(hitTwo.normal,1);
			FragColor = outcol;
		}
	]
]