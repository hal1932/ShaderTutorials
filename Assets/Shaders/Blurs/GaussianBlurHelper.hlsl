
half4 CalcGaussianFromPascalsTriangle(TEXTURE2D_PARAM(tex, samp), float2 uv, float2 offsetUnit) {
    half4 color = half4(0, 0, 0, 1);

#ifdef GAUSSIAN_KERNEL_3
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit) * (1.0 / 4.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv) * (2.0 / 4.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit) * (1.0 / 4.0);
#endif

#ifdef GAUSSIAN_KERNEL_5
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * 2) * (1.0 / 16.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * 1) * (4.0 / 16.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv) * (6.0 / 16.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * 1) * (4.0 / 16.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * 2) * (1.0 / 16.0);
#endif

#ifdef GAUSSIAN_KERNEL_7
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * 3) * (1.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * 2) * (6.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * 1) * (15.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv) * (20.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * 1) * (15.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * 2) * (6.0 / 64.0);
    color += SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * 3) * (1.0 / 64.0);
#endif

    return color;
}

half4 CalcGaussianFromWeights(TEXTURE2D_PARAM(tex, samp), float2 uv, float2 offsetUnit, float weights[GAUSSIAN_WEIGHTS_MAX], int weightCount) {
    half4 color = half4(0, 0, 0, 1);
    color += SAMPLE_TEXTURE2D(tex, samp, uv) * weights[0];
    for (int i = 1; i < weightCount; ++i) {
        half4 left = SAMPLE_TEXTURE2D(tex, samp, uv - offsetUnit * i);
        half4 right = SAMPLE_TEXTURE2D(tex, samp, uv + offsetUnit * i);
        color += (left + right) * weights[i];
    }
    return color;
}
