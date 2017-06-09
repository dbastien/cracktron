inline float PerpDot(float2 v1, float2 v2)
{
	return dot(v1, -v2.yx);
}