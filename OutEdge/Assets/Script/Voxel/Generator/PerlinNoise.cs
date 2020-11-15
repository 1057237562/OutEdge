
using System;

public class NoiseGeneratorPerlin
{
    private SimplexNoise[] noiseLevels;
    private int levels;

    public NoiseGeneratorPerlin(Random seed, int levelsIn)
    {
        this.levels = levelsIn;
        this.noiseLevels = new SimplexNoise[levelsIn];

        for (int i = 0; i < levelsIn; ++i)
        {
            this.noiseLevels[i] = new SimplexNoise(seed);
        }
    }

    public float getValue(float x, float y)
    {
        float f0 = 0.0f;
        float f1 = 1.0f;

        for (int i = 0; i < levels; ++i)
        {
            f0 += noiseLevels[i].getValue(x * f1, y * f1) / f1;
            f1 /= 2.0f;
        }

        return f0;
    }

    public float[] getRegion(float[] p_151599_1_, float p_151599_2_, float p_151599_4_, int p_151599_6_, int p_151599_7_, float p_151599_8_, float p_151599_10_, float p_151599_12_)
    {
        return getRegion(p_151599_1_, p_151599_2_, p_151599_4_, p_151599_6_, p_151599_7_, p_151599_8_, p_151599_10_, p_151599_12_, 0.5f);
    }

    public float[] getRegion(float[] p_151600_1_, float p_151600_2_, float p_151600_4_, int p_151600_6_, int p_151600_7_, float p_151600_8_, float p_151600_10_, float p_151600_12_, float p_151600_14_)
    {
        if (p_151600_1_ != null && p_151600_1_.Length >= p_151600_6_ * p_151600_7_)
        {
            for (int i = 0; i < p_151600_1_.Length; ++i)
            {
                p_151600_1_[i] = 0.0f;
            }
        }
        else
        {
            p_151600_1_ = new float[p_151600_6_ * p_151600_7_];
        }

        float f1 = 1.0f;
        float f0 = 1.0f;

        for (int j = 0; j < levels; ++j)
        {
            noiseLevels[j].add(p_151600_1_, p_151600_2_, p_151600_4_, p_151600_6_, p_151600_7_, p_151600_8_ * f0 * f1, p_151600_10_ * f0 * f1, 0.55f / f1);
            f0 *= p_151600_12_;
            f1 *= p_151600_14_;
        }

        return p_151600_1_;
    }
}