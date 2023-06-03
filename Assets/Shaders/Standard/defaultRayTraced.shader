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
		#define PI 3.1415926538
		#define MAX_BOUNCE_COUNT 5
		
		in vec3 color;
		in vec3 pos;
		in vec2 texCoord;
		
		uniform vec3 _CamPos;
		uniform vec3 _ViewParams;
		uniform mat4 _CamLocalToWorldMatrix;
		uniform int _Frame;
		uniform vec2 _ScreenSize;
		
		out vec4 FragColor;
		
		//Data structures
		struct Ray {
			vec3 origin;
			vec3 dir;
		};
		
		struct RTMaterial {
			vec4 color;
			vec4 emissionColor;
			float emissionStrength;
		};
		
		struct HitInfo {
			bool didHit;
			float dst;
			vec3 hitPoint;
			vec3 normal;
			RTMaterial material;
		};
		
		
		//Collection of stolen utility functions from https://github.com/SebLague/Ray-Tracing/blob/main/Assets/Scripts/Shaders/RayTracing.shader
		
		// PCG (permuted congruential generator). Thanks to:
		// www.pcg-random.org and www.shadertoy.com/view/XlGcRh
		uint NextRandom(inout uint state)
		{
			state = state * 747796405u + 2891336453u;
			uint result = ((state >> ((state >> 28) + 4u)) ^ state) * 277803737u;
			result = (result >> 22) ^ result;
			return result;
		}

		float RandomValue(inout uint state)
		{
			return NextRandom(state) / 4294967295.0; // 2^32 - 1 //
		}

		// Random value in normal distribution (with mean=0 and sd=1)
		float RandomValueNormalDistribution(inout uint state)
		{
			// Thanks to https://stackoverflow.com/a/6178290
			float theta = 2 * 3.1415926 * RandomValue(state);
			float rho = sqrt(-2 * log(RandomValue(state)));
			return rho * cos(theta);
		}

		// Calculate a random direction
		vec3 RandomDirection(inout uint state)
		{
			// Thanks to https://math.stackexchange.com/a/1585996
			float x = RandomValueNormalDistribution(state);
			float y = RandomValueNormalDistribution(state);
			float z = RandomValueNormalDistribution(state);
			return normalize(vec3(x, y, z));
		}

		vec2 RandomPointInCircle(inout uint rngState)
		{
			float angle = RandomValue(rngState) * 2 * PI;
			vec2 pointOnCircle = vec2(cos(angle), sin(angle));
			return pointOnCircle * sqrt(RandomValue(rngState));
		}

		vec2 mod2(vec2 x, vec2 y)
		{
			return x - y * floor(x/y);
		}
	
		
		//Intersection Functions
		HitInfo HitSphere(Ray ray, vec3 center, float radius, RTMaterial material) {
			HitInfo info;
			vec3 offset = ray.origin - center;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			info.material = material;
			
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
		HitInfo HitBox(Ray ray, vec3 center, vec3 boxMin, vec3 boxMax, RTMaterial material) {
			HitInfo info;
			vec3 offset = ray.origin - center;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			info.material = material;
			
			vec3 rayInvDir = 1.0 / ray.dir;
			vec3 tbot = rayInvDir * (boxMin - offset);
			vec3 ttop = rayInvDir * (boxMax - offset);
			vec3 tmin = min(ttop, tbot);
			vec3 tmax = max(ttop, tbot);
			vec2 t = max(tmin.xx, tmin.yz);
			float t0 = max(t.x, t.y);
			t = min(tmax.xx, tmax.yz);
			float t1 = min(t.x, t.y);
			vec2 result = vec2(t0, t1);
			info.hitPoint = ray.origin + ray.dir * mix(result.x,result.y,result.x < 0);
			
			vec3 box_hit = info.hitPoint - center;
			vec3 box_intersect_normal = box_hit / max(max(abs(box_hit.x), abs(box_hit.y)), abs(box_hit.z));
			box_intersect_normal = clamp(box_intersect_normal, vec3(0.0,0.0,0.0), vec3(1.0,1.0,1.0));
			box_intersect_normal = normalize(floor(box_intersect_normal * 1.0000001));
			
			
			info.normal = box_intersect_normal;
			info.didHit = t1 > max(t0, 0.0);
			info.dst = distance(ray.origin, info.hitPoint);
			
			return info;
		}
		/*
		HitInfo HitBox(Ray ray, vec3 center, vec3 minPos, vec3 maxPos, RTMaterial material) {
			HitInfo info;
			vec3 offset = ray.origin - center;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			info.material = material;
			
			
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
			box_hit.z = -box_hit.z;
			box_hit.x = -box_hit.x;
			box_hit.y = -box_hit.y;
			vec3 box_intersect_normal = box_hit / max(max(abs(box_hit.x), abs(box_hit.y)), abs(box_hit.z));
			box_intersect_normal = clamp(box_intersect_normal, vec3(0.0,0.0,0.0), vec3(1.0,1.0,1.0));
			box_intersect_normal = normalize(floor(box_intersect_normal * 1.0000001));
			info.normal = box_intersect_normal;
			info.hitPoint = box_hit;
			info.didHit = box.x > 0;
			info.dst = distance(ray.origin,box_hit);
			return info;
		}
		*/
		
		//Stolen from https://cmichel.io/howto-raytracer-ray-plane-intersection-theory
		HitInfo HitPlane(Ray ray, vec3 center, vec3 normal)
		{
			HitInfo info;
			info.didHit = false;
			info.dst = 0;
			info.hitPoint = vec3(0,0,0);
			info.normal = vec3(0,0,0);
			info.material.color = vec4(1,0,0,1);
			

	
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
		
		HitInfo CalculateScene(Ray ray) {
			HitInfo closest;
			closest.didHit = false;
			closest.dst = 9999999;
			closest.hitPoint = vec3(0,0,0);
			closest.normal = vec3(0,0,0);
			
			RTMaterial light;
			light.color = vec4(0,0,0,0);
			light.emissionColor = vec4(1,1,1,1);
			light.emissionStrength = 2;
			
			RTMaterial ground;
			ground.color = vec4(0.6,1,1,1);
			
			RTMaterial sphere;
			sphere.color = vec4(0.2,0.2,1,1);
			RTMaterial sphereT;
			sphereT.color = vec4(0.2,1,0.2,1);
			RTMaterial sphereTT;
			sphereTT.color = vec4(1,0.2,0.2,1);
			
			//Loop through all objects
			
			HitInfo hit = HitSphere(ray, vec3(3,3,-3), 0.5, light);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitBox(ray, vec3(0,-51,0), vec3(-50), vec3(50), ground); //
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(2,0,-4), 1, sphere);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(0.5,0,-3), 0.7, sphereT);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(-2,0.5,-3), 1.3, sphereTT);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			return closest;
		}
		
		vec4 Trace(Ray baseRay, inout uint state) {
			vec4 incomingLight = vec4(0,0,0,1);
			vec4 rayColor = vec4(1);
			Ray ray;
			ray.origin = baseRay.origin;
			ray.dir = baseRay.dir;
			
			
			for(int i = 0; i <= MAX_BOUNCE_COUNT; i++) {
				HitInfo info = CalculateScene(ray);
				if(info.didHit) {
					ray.origin = info.hitPoint;
					RTMaterial material = info.material; // +
					vec3 newDir = info.normal + RandomDirection(state);
					if(dot(newDir, info.normal) < 0) newDir = -newDir;
					ray.dir = normalize(newDir);
					
					vec4 emittedLight = material.emissionColor * material.emissionStrength;
					incomingLight += emittedLight * rayColor;
					rayColor *= material.color;
				} else {
					break;
				}
			}
			
			return incomingLight;
		}
		
		
		void main()
		{
			vec3 viewPointLocal = vec3(texCoord- 0.5, 1) * _ViewParams;
			viewPointLocal.y *= -1;
			viewPointLocal.z *= -1;
			vec3 viewPoint = (vec4(viewPointLocal, 1) * _CamLocalToWorldMatrix).xyz;
			uint pixelIndex = uint(floor((texCoord.y * uint(_ScreenSize.y)) * uint(_ScreenSize.x) + texCoord.x * uint(_ScreenSize.x)));
			uint state = pixelIndex + uint(_Frame) * 719393u;
			
			Ray ray;
			ray.origin = _CamPos;
			ray.dir = normalize(viewPoint - ray.origin);
			
			vec4 outcol = vec4(0);
			for(int i = 0; i < 400; i++) {
				outcol += Trace(ray, state);
			}
			outcol /= 20;
			FragColor = outcol;
		}
	]
]