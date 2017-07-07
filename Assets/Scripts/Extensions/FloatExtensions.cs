public static class FloatExtensions
{
    public static float Remap01(this float x, float minIn, float maxIn)
    {
        return (x - minIn) / (maxIn - minIn);
    }

    public static float Remap(this float x, float minIn, float maxIn, float minOut, float maxOut)
    {
        return minOut + (maxOut - minOut) * Remap01(x, minIn, maxIn);
    }
}
