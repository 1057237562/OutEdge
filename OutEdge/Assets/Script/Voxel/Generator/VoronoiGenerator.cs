using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

[RequireComponent(typeof(RawImage))]
public class VoronoiGenerator
{

    private int seed;

   private int[] units;

    public int maxInt;

    public VoronoiGenerator(int s,int mi)
    {
        seed = s;
        maxInt = mi;
        units = new int[3] { 2, 3, 4 };
    }
    public VoronoiGenerator(int s,int mi,int[] u)
    {
        seed = s;
        maxInt = mi;
        units = u;
    }

    private static int DistanceSqr(Vector2Int a, Vector2Int b)
    {
        return (a - b).sqrMagnitude;
    }

    public int[,] GenerateMap(int startx,int startz,int width,int height)
    {
        int[,] heightMap = new int[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                heightMap[i, j] = getValue(startx + i, startz + j);
            }
        }
        return heightMap;
    }

    public int getValue(int x, int y)
    {
        return getValue(0, x, y);
    }
    public int getValue(int level, int x, int y)
    {
        if (level == units.Length - 1)
        {
            return GetClosestRootValueFrom(level, x, y);
        }
        var next = GetClosestRootPositionFrom(level, x, y);
        return getValue(level + 1, next.x, next.y);
    }

    private int GetValueOfCellRoot(int level, int cell_x, int cell_y)
    {
        var rand = new Random(cell_x ^ cell_y + level + (seed << 2));
        return rand.Next(0,maxInt);
    }

    private Vector2Int GetCellRootPosition(int level, int cell_x, int cell_y)
    {
        var rand = new Random((level * cell_x + cell_y) + level * level + seed);
        return new Vector2Int(
            (cell_x << units[level]) + rand.Next(1 << units[level]),
            (cell_y << units[level]) + rand.Next(1 << units[level]));
    }
    private Vector2Int GetClosestRootPositionFrom(int level, int x, int y)
    {
        int cx = x >> units[level], cy = y >> units[level];
        var min_dist = int.MaxValue;
        var ret = Vector2Int.zero;
        for (var i = -1; i <= 1; ++i)
        {
            for (var j = -1; j <= 1; ++j)
            {
                int ax = cx + i, ay = cy + j;
                var pos = GetCellRootPosition(level, ax, ay);
                var dist = DistanceSqr(pos, new Vector2Int(x, y));
                if (dist < min_dist)
                {
                    min_dist = dist;
                    ret = pos;
                }
            }
        }
        return ret;
    }
    private int GetClosestRootValueFrom(int level, int x, int y)
    {
        int cx = x >> units[level], cy = y >> units[level];
        var min_dist = int.MaxValue;
        int ret = 0;
        for (var i = -1; i <= 1; ++i)
        {
            for (var j = -1; j <= 1; ++j)
            {
                int ax = cx + i, ay = cy + j;
                var pos = GetCellRootPosition(level, ax, ay);
                int value = GetValueOfCellRoot(level, ax, ay);
                var dist = DistanceSqr(pos, new Vector2Int(x, y));
                if (dist < min_dist)
                {
                    min_dist = dist;
                    ret = value;
                }
            }
        }
        return ret;
    }
}