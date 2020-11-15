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
public class VoronoiGen : MonoBehaviour
{

    [SerializeField] private int seed_, width_, height_,startx,starty;
    [SerializeField] private int[] units_;
    private void Start()
    {
        GetComponent<RawImage>().texture = RenderVoronoiGraph(startx,starty,width_, height_);
    }

    [ContextMenu("Regen")]
    public void ReGenerate()
    {
        GetComponent<RawImage>().texture = RenderVoronoiGraph(startx, starty, width_, height_);
    }
    private static int DistanceSqr(Vector2Int a, Vector2Int b)
    {
        return (a - b).sqrMagnitude;
    }
    //把地图h渲染到材质上
    private Texture2D RenderVoronoiGraph(int startx,int starty,int width, int height)
    {
        var tex = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point
        };
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                tex.SetPixel(x, y, Convolution(startx*width + x, starty*height + y));
            }
        }
        tex.Apply();
        return tex;
    }

    public Color Convolution(int x,int z)
    {
        Color c = new Color(0, 0, 0);
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1 ; j <= 1; j++)
            {
                c += GetTextureColorAt(x + i, z + j);
            }
        }
        return c / 9;
    }

    //获取该点所在区域的颜色
    private Color GetTextureColorAt(int x, int y)
    {
        return GetTextureColorAt(0, x, y);
    }
    private Color GetTextureColorAt(int level, int x, int y)
    {
        if (level == units_.Length - 1)
        {
            return GetClosestRootColorFrom(level, x, y);
        }
        var next = GetClosestRootPositionFrom(level, x, y);
        return GetTextureColorAt(level + 1, next.x, next.y);
    }
    //获取某一个Cell的根是什么颜色的
    private Color GetColorOfCellRoot(int level, int cell_x, int cell_y)
    {
        var rand = new Random(cell_x ^ cell_y + level + (seed_ << 2));
        return new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
    }

    private Vector2Int GetCellRootPosition(int level, int cell_x, int cell_y)
    {
        var rand = new Random((level * cell_x + cell_y) + level * level + seed_);
        return new Vector2Int(
            (cell_x << units_[level]) + rand.Next(1 << units_[level]),
            (cell_y << units_[level]) + rand.Next(1 << units_[level]));
    }
    private Vector2Int GetClosestRootPositionFrom(int level, int x, int y)
    {
        int cx = x >> units_[level], cy = y >> units_[level];
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
    private Color GetClosestRootColorFrom(int level, int x, int y)
    {
        int cx = x >> units_[level], cy = y >> units_[level];
        var min_dist = int.MaxValue;
        var ret = Color.red;
        for (var i = -1; i <= 1; ++i)
        {
            for (var j = -1; j <= 1; ++j)
            {
                int ax = cx + i, ay = cy + j;
                var pos = GetCellRootPosition(level, ax, ay);
                var color = GetColorOfCellRoot(level, ax, ay);
                var dist = DistanceSqr(pos, new Vector2Int(x, y));
                if (dist < min_dist)
                {
                    min_dist = dist;
                    ret = color;
                }
            }
        }
        return ret;
    }
}