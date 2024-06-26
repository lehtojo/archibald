shader_type spatial;

uniform vec3 albedo : source_color;
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;
uniform sampler2D texture_normal;
uniform sampler2D texture_normal2;
uniform sampler2D texture_water_level;
uniform vec2 wave_direction = vec2(1.5, 0.0);
uniform vec2 wave_direction2 = vec2(0.0, 1.0);
uniform float wave_amplitude = 100.0;
uniform float time_scale : hint_range(0, 0.2, 0.005) = 0.025;

void vertex() {
	vec2 time = (TIME * wave_direction) * time_scale;
	float height = texture(texture_water_level,UV + time).r - 0.5;
	VERTEX.y += height * wave_amplitude;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 time = (TIME * wave_direction) * time_scale;
	vec2 time2 = (TIME * wave_direction2) * time_scale;
	float height = texture(texture_water_level, UV * 30.0 + time).r - 0.5;
	vec3 normal_blend = mix(texture(texture_normal,UV + time).rgb, texture(texture_normal2,UV + time2).rgb, 0.5);

	ALBEDO = mix(albedo, vec3(1.0, 1.0, 1.0), height * 0.025);
	METALLIC = metallic;
	ROUGHNESS = roughness;
	SPECULAR = roughness;
	NORMAL_MAP = normal_blend;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
