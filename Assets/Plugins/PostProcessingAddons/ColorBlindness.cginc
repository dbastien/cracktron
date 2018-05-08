#ifndef COLOR_BLINDNESS
#define COLOR_BLINDNESS

//https://en.wikipedia.org/wiki/LMS_color_space
//http://ixora.io/projects/colorblindness/color-blindness-simulation-research/

float3 LinearRGBToLMS(float3 lrgb)
{
	const float3x3 LinearRGBToLMS_Matrix =
	{
		0.31399022, 0.63951294, 0.04649755,
		0.15537241, 0.75789446, 0.08670142,
		0.01775239, 0.10944209, 0.87256922
	};

	return mul(LinearRGBToLMS_Matrix, lrgb);
}

float3 LMSToLinearRGB(float3 lms)
{
	const float3x3 LMSToLinearRGB_Matrix =
	{
		 5.47221206, -4.64196010,  0.16963708,
		-1.12524190,  2.29317094, -0.16789520,
		 0.02980165, -0.19318073,  1.16364789
	};

	return mul(LMSToLinearRGB_Matrix, lms);
}

float3 Protanopia(float3 lms)
{
	const float3x3 LMSProtanopia_Matrix =
	{
		 0.0, 1.05118294, -0.05116099,
		 0.0, 1.0       ,  0.0,
		 0.0, 0.0       ,  1.0
	};

	return mul(LMSProtanopia_Matrix, lms);
}

float3 Deuteranopia(float3 lms)
{
	const float3x3 LMSDeuteranopia_Matrix =
	{
		 1.0      , 0.0,  0.0,
		 0.9513092, 0.0,  0.04866992,
		 0.0      , 0.0,  1.0
	};

	return mul(LMSDeuteranopia_Matrix, lms);
}

float3 Tritanopia(float3 lms)
{
	const float3x3 LMSTritanopia_Matrix =
	{
		  1.0       , 0.0       ,  0.0,
		  0.0       , 1.0       ,  0.0,
		 -0.86744736, 1.86727089,  0.0
	};

	return mul(LMSTritanopia_Matrix, lms);
}

#endif //COLOR_BLINDNESS