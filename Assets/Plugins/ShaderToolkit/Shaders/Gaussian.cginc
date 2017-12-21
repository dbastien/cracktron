float4 gaussian(sampler2D tex, float2 texelSize, float2 uv, float2 dir)
{
//    const int taps = 3;
//    float offset[3] = { 0.0, 1.3846153846, 3.2307692308 };
//    float weight[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };

//    const int taps = 4;
//    float offset[4] = {0, 1.45843, 3.403985, 5.351806};
 //   float weight[4] = {0.1329808, 0.2322771, 0.135327, 0.05115604};

    const int taps = 8;
    float weight[8] = {0.04462541, 0.08833525, 0.08476666, 0.07870257, 0.070701, 0.06145185, 0.05167937, 0.04205059};
    float offset[8] = {0, 1.496901, 3.492769, 5.488638, 7.484509, 9.480382, 11.47626, 13.47214};

    float4 col = tex2D(tex, uv) * weight[0];

    float2 scaledDir = dir * texelSize;

    [unroll]
    for (int i = 1; i < taps; ++i)
    {
        col += tex2D(tex, uv + scaledDir * offset[i]) * weight[i];
        col += tex2D(tex, uv - scaledDir * offset[i]) * weight[i];
    }

    return col;
}