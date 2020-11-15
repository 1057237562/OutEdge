
using System;
using UnityEngine;

public class SimplexNoise {
    
    private int[] perm;
    
    public SimplexNoise(System.Random random)
    {
        perm = new int[512];

        xo = random.NextDouble() * 256;
        yo = random.NextDouble() * 256;

        for (int i = 0; i < 256; perm[i] = i++) { }
        for (int l = 0; l < 256; ++l)
        {
            int j = random.Next(256 - l) + l;
            int k = perm[l];
            perm[l] = perm[j];
            perm[j] = k;
            perm[l + 256] = perm[l];
        }
    }

    const float F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
    const float G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0
    public static float SQRT_3 = Mathf.Sqrt(3.0f);
    private static int[][] grad3 = new int[][] { new int[] { 1, 1, 0 }, new int[] { -1, 1, 0 }, new int[] { 1, -1, 0 }, new int[] { -1, -1, 0 }, new int[] { 1, 0, 1 }, new int[] { -1, 0, 1 }, new int[] { 1, 0, -1 }, new int[] { -1, 0, -1 }, new int[] { 0, 1, 1 }, new int[] { 0, -1, 1 }, new int[] { 0, 1, -1 }, new int[] { 0, -1, -1 } };

    public double xo;
    public double yo;

    public float getValue(float x, float y)
    {
        float d3 = 0.5f * (SQRT_3 - 1.0f);
        float d4 = (x + y) * d3;
        int i = FastFloor(x + d4);
        int j = FastFloor(y + d4);
        float d5 = (3.0f - SQRT_3) / 6.0f;
        float d6 = (float)(i + j) * d5;
        float d7 = (float)i - d6;
        float d8 = (float)j - d6;
        float d9 = x - d7;
        float d10 = y - d8;
        int k;
        int l;

        if (d9 > d10)
        {
            k = 1;
            l = 0;
        }
        else
        {
            k = 0;
            l = 1;
        }

        float d11 = d9 - (float)k + d5;
        float d12 = d10 - (float)l + d5;
        float d13 = d9 - 1.0f + 2.0f * d5;
        float d14 = d10 - 1.0f + 2.0f * d5;
        int i1 = i & 255;
        int j1 = j & 255;
        int k1 = perm[i1 + perm[j1]] % 12;
        int l1 = perm[i1 + k + perm[j1 + l]] % 12;
        int i2 = perm[i1 + 1 + perm[j1 + 1]] % 12;
        float d15 = 0.5f - d9 * d9 - d10 * d10;
        float d0;

        if (d15 < 0.0f)
        {
            d0 = 0.0f;
        }
        else
        {
            d15 = d15 * d15;
            d0 = d15 * d15 * dot(grad3[k1], d9, d10);
        }

        float d16 = 0.5f - d11 * d11 - d12 * d12;
        float d1;

        if (d16 < 0.0f)
        {
            d1 = 0.0f;
        }
        else
        {
            d16 = d16 * d16;
            d1 = d16 * d16 * dot(grad3[l1], d11, d12);
        }

        float d17 = 0.5f - d13 * d13 - d14 * d14;
        float d2;

        if (d17 < 0.0f)
        {
            d2 = 0.0f;
        }
        else
        {
            d17 = d17 * d17;
            d2 = d17 * d17 * dot(grad3[i2], d13, d14);
        }

        return 70.0f * (d0 + d1 + d2);
    }

    public float Generate(float x, float y)
    {
        float n0, n1, n2; // Noise contributions from the three corners

        // Skew the input space to determine which simplex cell we're in
        float s = (x+y)*F2; // Hairy factor for 2f
        float xs = x + s;
        float ys = y + s;
        int i = FastFloor(xs);
        int j = FastFloor(ys);
 

        float t = (float)(i+j)*G2;
        // Unskew the cell origin back to (x,y) space
        float X0 = i-t;
        float Y0 = j-t;
        // The x,y distances from the cell origin
        float x0 = x-X0; 
        float y0 = y-Y0;

        // For the 2f case, the simplex shape is an equilateral triangle.
        // fetermine which simplex we are in.
        
        // Offsets for second (middle) corner of simplex in (i,j) coords
        int i1;
        int j1; 
        
        if (x0 > y0)
        {
            // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            i1 = 1;
            j1 = 0;
        } 
        else
        {
            // upper triangle, YX order: (0,0)->(0,1)->(1,1)
            i1 = 0;
            j1 = 1;
        }      

        // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
        // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
        // c = (3-sqrt(3))/6
        
        // Offsets for middle corner in (x,y) unskewed coords
        float x1 = x0 - i1 + G2; 
        float y1 = y0 - j1 + G2;
         // Offsets for last corner in (x,y) unskewed coords
        float x2 = x0 - 1.0f + 2.0f * G2;
        float y2 = y0 - 1.0f + 2.0f * G2;

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        int ii = i % 256;
        int jj = j % 256;

        // Calculate the contribution from the three corners
        float t0 = 0.5f - x0 * x0 - y0 * y0;
        
        if(t0 < 0.0f)
        {
             n0 = 0.0f;
        }
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Grad(perm[ii+perm[jj]], x0, y0);
        }

        float t1 = 0.5f - x1 * x1 - y1 * y1;
        if(t1 < 0.0f)
        {
            n1 = 0.0f;
        }
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Grad(perm[ii+i1+perm[jj+j1]], x1, y1);
        }
    
        float t2 = 0.5f - x2 * x2 - y2 * y2;
        
        if(t2 < 0.0f)
        {
            n2 = 0.0f;
        }
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Grad(perm[ii+1+perm[jj+1]], x2, y2);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to return values in the interval [-1,1].
        return (n0 + n1 + n2);
    }

    public void add(float[] p_151606_1_, float p_151606_2_, float p_151606_4_, int p_151606_6_, int p_151606_7_, float p_151606_8_, float p_151606_10_, float p_151606_12_)
    {
        int i = 0;

        for (int j = 0; j < p_151606_7_; ++j)
        {
            float d0 = (float)((p_151606_4_ + j) * p_151606_10_ + yo);

            for (int k = 0; k < p_151606_6_; ++k)
            {
                float d1 = (float)((p_151606_2_ + k) * p_151606_8_ + xo);
                float d5 = (d1 + d0) * F2;
                int l = FastFloor(d1 + d5);
                int i1 = FastFloor(d0 + d5);
                float d6 = (float)(l + i1) * G2;
                float d7 = (float)l - d6;
                float d8 = (float)i1 - d6;
                float d9 = d1 - d7;
                float d10 = d0 - d8;
                int j1;
                int k1;

                if (d9 > d10)
                {
                    j1 = 1;
                    k1 = 0;
                }
                else
                {
                    j1 = 0;
                    k1 = 1;
                }

                float d11 = d9 - (float)j1 + G2;
                float d12 = d10 - (float)k1 + G2;
                float d13 = d9 - 1.0f + 2.0f * G2;
                float d14 = d10 - 1.0f + 2.0f * G2;
                int l1 = l & 255;
                int i2 = i1 & 255;
                int j2 = perm[l1 + perm[i2]] % 12;
                int k2 = perm[l1 + j1 + perm[i2 + k1]] % 12;
                int l2 = perm[l1 + 1 + perm[i2 + 1]] % 12;
                float d15 = 0.5f - d9 * d9 - d10 * d10;
                float d2;

                if (d15 < 0.0f)
                {
                    d2 = 0.0f;
                }
                else
                {
                    d15 = d15 * d15;
                    d2 = d15 * d15 * dot(grad3[j2], d9, d10);
                }

                float d16 = 0.5f - d11 * d11 - d12 * d12;
                float d3;

                if (d16 < 0.0f)
                {
                    d3 = 0.0f;
                }
                else
                {
                    d16 = d16 * d16;
                    d3 = d16 * d16 * dot(grad3[k2], d11, d12);
                }

                float d17 = 0.5f - d13 * d13 - d14 * d14;
                float d4;

                if (d17 < 0.0f)
                {
                    d4 = 0.0f;
                }
                else
                {
                    d17 = d17 * d17;
                    d4 = d17 * d17 * dot(grad3[l2], d13, d14);
                }

                int i3 = i++;
                p_151606_1_[i3] += 70.0f * (d2 + d3 + d4) * p_151606_12_;
            }
        }
    }

    /// <summary>
    /// Faster implementation of floor method.
    /// </summary>
    /// <param name="x">Floating point number</param>
    /// <returns>The largest whole number less than or equal to x.</returns>
    private static int FastFloor(float x)
    {
        return (x > 0) ? ((int)x) : (((int)x) - 1);
    }
    
    /// <summary>
    /// Faster implementation of floor method.
    /// </summary>
    /// <param name="hash">Hash key to acces permutation</param>
    /// <param name="x">X-Index</param>
    /// <param name="y">Y-Index</param>
    /// <returns>The gradient of the given position</returns>
    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 7;      // Convert low 3 bits of hash code
        float u = h < 4 ? x : y;  // into 8 simple gradient directions,
        float v = h < 4 ? y : x;  // and compute the dot product with (x,y).
        return ((h&1) != 0 ? -u : u) + ((h&2) != 0 ? -2.0f * v : 2.0f * v);
    }

    private static float dot(int[] p_151604_0_, float p_151604_1_, float p_151604_3_)
    {
        return (float)p_151604_0_[0] * p_151604_1_ + (float)p_151604_0_[1] * p_151604_3_;
    }

}
