Shader "Vertex Colors" {
	SubShader{
		BindChannels{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
		Pass{
	}
	}
}
