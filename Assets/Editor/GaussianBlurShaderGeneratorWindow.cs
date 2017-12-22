using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class GaussianBlurShaderGeneratorWindow : EditorWindow 
{
	protected float sigma = 1f;
	protected int radius = 2;
	protected bool singlePass = true;
	protected string result = string.Empty;	

	[MenuItem("Cracktron/Gaussian Blur Shader Generator")]
	public static void Init()
	{
		var window = EditorWindow.GetWindow<GaussianBlurShaderGeneratorWindow>();
		window.Show();
	}

	void OnGUI()
	{
		GUILayout.Label("Gaussian Blur Shader Generator");
		sigma = EditorGUILayout.FloatField("Sigma", sigma);
		radius = EditorGUILayout.IntField("Radius", radius);	

		if (GUILayout.Button("Generate"))
		{
			result = string.Empty;
			
			var mtx = GaussianMatrix1DHalf(sigma, radius);
			result = string.Join(" , ", mtx.Select(x => x.ToString()).ToArray());
			result += "\n";

			float[] optWeights;
			float[] optOffsets;
			GaussianMatrix1DHalfToLinearOptimized(mtx, out optWeights, out optOffsets);
			NormalizeHalfMatrix(ref optWeights);
			result += "weights: {" +  string.Join(", ", optWeights.Select(x => x.ToString()).ToArray()) + "}";
			result += "\n";
			result += "offsets: {" +  string.Join(", ", optOffsets.Select(x => x.ToString()).ToArray()) + "}";
			result += "\n";

			mtx = ExpandHalfMatrixToFull(mtx);
			result += string.Join(" , ", mtx.Select(x => x.ToString()).ToArray());
			result += "\n";

			NormalizeMatrix(ref mtx);
			result += string.Join(" , ", mtx.Select(x => x.ToString()).ToArray());			
			result += "\n";
		}

		EditorGUILayout.TextArea(result);
	}

	void GaussianMatrix1DHalfToLinearOptimized(float[] wIn, out float[] wOut, out float[] offsets)
	{
		int l = wIn.Length;
		int lOut = (wIn.Length + 1)/2;

		wOut = new float[lOut];
		offsets = new float[lOut];

		wOut[0] = wIn[0];
		offsets[0] = 0.0f;

		int i = 1; //input iterator
		for (int o = 1; o < lOut; ++o)
		{		
 			wOut[o] = wIn[i] + wIn[i+1];
			offsets[o] = (i * wIn[i] + (i+1) * wIn[i+1]) / wOut[o];
			i += 2;
		}
	}
	void NormalizeHalfMatrix(ref float[] a)
	{	
		float s = a[0];
		for (var i = 1; i < a.Length; ++i)
		{
			s += a[i] * 2;
		}

		for (var i = 0; i < a.Length; ++i)
		{
			a[i] /= s;
		}
	}

	void NormalizeMatrix(ref float[] a)
	{
		float s = a[0];
		for (var i = 1; i < a.Length; ++i)
		{
			s += a[i];
		}

		for (var i = 0; i < a.Length; ++i)
		{
			a[i] /= s;
		}
	}

	float[] ExpandHalfMatrixToFull(float[] m)
	{
		int lO = m.Length * 2 - 1;
		int cO = lO/2;
		float[] res = new float[lO];

		for (var i = 1; i < m.Length; ++i)
		{
			res[cO-i] = m[i];
			res[cO+i] = m[i];
		}

		res[cO] = m[0];

		return res;
	}

	float[] GaussianMatrix1DHalf(float sigma, int radius)
	{
		float[] res = new float[radius+1];
		for (var x = 0; x <= radius; ++x)
		{		
			res[x] = Gaussian(sigma, x);
		}
		return res;
	}

	float GaussianSimpsonIntegration (float sigma, float a, float b)
	{
    	return ((b - a) / 6.0f) * (Gaussian(sigma, a) + 4.0f * Gaussian(sigma, (a + b) / 2.0f) + Gaussian(sigma, b));
	}

	//https://www.wikiwand.com/en/Gaussian_blur
	float Gaussian(float sigma, float x, float y)
	{
		float numer = Mathf.Exp(-((x * x + y * y)/(2 * sigma * sigma)));
		float denom = sigma * sigma * MathfConstants.Tau;

		return (float)(numer/denom);
	}

	float Gaussian(float sigma, float x)
	{
		float numer = Mathf.Exp(-((x * x)/(2 * sigma * sigma)));
		float denom = sigma * MathfConstants.TauSqrt;

		return (float)(numer/denom);
	}
}
