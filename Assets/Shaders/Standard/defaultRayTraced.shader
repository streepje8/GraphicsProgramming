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
		#define MAX_BOUNCE_COUNT 50
		#define RAYS_PER_PIXEL 80

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
			float specularProbability;
			float smoothness;
		};
		
		struct HitInfo {
			bool didHit;
			float dst;
			vec3 hitPoint;
			vec3 normal;
			RTMaterial material;
		};
		
		
		//Collection of stolen utility functions from https://github.com/SebLague/Ray-Tracing/blob/main/Assets/Scripts/Shaders/RayTracing.shader
		// Crude sky colour function for background light
		vec3 GetEnvironmentLight(Ray ray)
		{			
			vec3 GroundColour = vec3(0.35,0.3,0.35);
			vec3 SkyColourHorizon = vec3(1,1,1);
			vec3 SkyColourZenith = vec3(0.0788092,0.36480793,0.7264151);
			float SunFocus = 500;
			float SunIntensity = 10;
		
		
			float skyGradientT = pow(smoothstep(0, 0.4, ray.dir.y), 0.35);
			float groundToSkyT = smoothstep(-0.01, 0, ray.dir.y);
			vec3 skyGradient = mix(SkyColourHorizon, SkyColourZenith, skyGradientT);
			float sun = max(0,pow(max(0, dot(ray.dir, vec3(1,1,1))), SunFocus) * SunIntensity);
			sun=0;
			// Combine ground, sky, and sun
			vec3 composite = mix(GroundColour, skyGradient, groundToSkyT) + (sun * float(groundToSkyT>=1));
			return composite;
		}
		
		
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
			light.emissionStrength = 1;
			light.specularProbability = 0;
			light.smoothness = 0;
			
			RTMaterial ground;
			ground.color = vec4(0.6,1,1,1);
			
			RTMaterial sphere;
			sphere.color = vec4(0.43,0.43,0.43,1);
			sphere.specularProbability = 0.6;
			sphere.smoothness = 0.9;
			RTMaterial sphereT;
			sphereT.color = vec4(0.2,1,0.2,1);
			sphereT.specularProbability = 0.7;
			sphereT.smoothness = 0.7;
			RTMaterial sphereTT;
			sphereTT.color = vec4(1,0.2,0.2,1);
			sphereTT.emissionColor = vec4(0,0,0,1);
			sphereTT.specularProbability = 0.7;
			sphereTT.smoothness = 0.7;
			sphereTT.emissionStrength = 0;
			
			//Loop through all objects
			
			HitInfo hit = HitSphere(ray, vec3(3,3,-5), 0.5, light);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitBox(ray, vec3(0,-51,0), vec3(-50), vec3(50), ground); //
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(2,0,-4), 1, sphere);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(-0.5,0,-1), 0.7, sphereT);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(-2,0.5,-3), 1.3, sphereTT);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			
			/* //Box thingy scene
			//Light
			RTMaterial light;
			light.color = vec4(0,0,0,0);
			light.emissionColor = vec4(1,1,1,1);
			light.emissionStrength = 1;
			light.specularProbability = 0;
			light.smoothness = 0;
		
			HitInfo hit = HitSphere(ray, vec3(0,2.1,0), 0.2, light);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			//Ceiling
			RTMaterial white;
			white.color = vec4(1);
			white.emissionColor = vec4(0);
			white.emissionStrength = 0;
			white.specularProbability = 0;
			white.smoothness = 0;
			hit = HitSphere(ray, vec3(0,32,0), 30, white);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			//Floor
			hit = HitSphere(ray, vec3(0,-30,0), 30, white);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			//Walls
			RTMaterial red;
			red.color = vec4(1,0.2,0.2,1);
			red.emissionColor = vec4(0);
			red.emissionStrength = 0;
			red.specularProbability = 0;
			red.smoothness = 0;
			RTMaterial blue;
			blue.color = vec4(0.2,0.2,1,1);
			blue.emissionColor = vec4(0);
			blue.emissionStrength = 0;
			blue.specularProbability = 0;
			blue.smoothness = 0;
			RTMaterial green;
			green.color = vec4(0.2,1,0.2,1);
			green.emissionColor = vec4(0);
			green.emissionStrength = 0;
			green.specularProbability = 0;
			green.smoothness = 0;
			RTMaterial purple;
			purple.color = vec4(0.6,0.2,0.8,1);
			purple.emissionColor = vec4(0);
			purple.emissionStrength = 0;
			purple.specularProbability = 0;
			purple.smoothness = 0;
			
			hit = HitSphere(ray, vec3(31,1,0), 30, red);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(-31,1,0), 30, blue);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(0,1,-31), 30, green);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			hit = HitSphere(ray, vec3(0,1,31), 30, purple);
			if(hit.didHit && hit.dst < closest.dst) closest = hit;
			
			//Subject
			RTMaterial subject;
			subject.color = vec4(1);
			subject.emissionColor = vec4(0);
			subject.emissionStrength = 0;
			subject.specularProbability = 1;
			subject.smoothness = 0.8;
			hit = HitSphere(ray, vec3(0,1,0), 0.2, subject);
			if(hit.didHit && hit.dst < closest.dst) closest = hit; */
			
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
					RTMaterial material = info.material;
					bool isSpecularBounce = material.specularProbability >= RandomValue(state);
					vec3 diffuseDir = info.normal + RandomDirection(state);
					if(dot(diffuseDir, info.normal) < 0) diffuseDir = -diffuseDir;
					vec3 specularDir = reflect(ray.dir,info.normal);
					ray.dir = normalize(mix(diffuseDir, specularDir, material.smoothness * float(isSpecularBounce)));
					
					vec4 emittedLight = material.emissionColor * material.emissionStrength;
					incomingLight += emittedLight * rayColor;
					rayColor *= material.color;
				} else {
					incomingLight += vec4(GetEnvironmentLight(ray) * 0.02,1) * rayColor;
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
			
			vec3 camRight = normalize((vec4(1,0,0, 1) * _CamLocalToWorldMatrix).xyz);
			vec3 camUp = normalize((vec4(0,1,0, 1) * _CamLocalToWorldMatrix).xyz);
			vec3 camForward = normalize((vec4(0,0,1, 1) * _CamLocalToWorldMatrix).xyz);
			
			
			float DivergeStrength = 0.3f;
			
			vec4 outcol = vec4(0);
			for(int i = 0; i < RAYS_PER_PIXEL; i++) {
				Ray ray;
				ray.origin = _CamPos;
				vec2 jitter = RandomPointInCircle(state) * DivergeStrength / _ScreenSize.x;
				vec3 jitteredViewPoint = viewPoint + camRight * jitter.x + camUp * jitter.y;
				ray.dir = normalize(jitteredViewPoint - ray.origin);
				outcol += Trace(ray, state);
			}
			
			FragColor = outcol;
		}
	]
]