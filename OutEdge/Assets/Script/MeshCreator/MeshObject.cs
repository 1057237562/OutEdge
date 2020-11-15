using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshObject : MonoBehaviour
{
    public List<int> trianglesco = new List<int>();
    public List<int> triangles = new List<int>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();

    private Dictionary<Vector3, int> points = new Dictionary<Vector3, int>();

    private Mesh geometry;
    private Mesh collider;

    private List<int> triangleBound = new List<int>();

    int round(int i,int bound)
    {
        return i % bound;
    }

    public Vector2 GenerateUV(int index)
    {
        int m = index % 4;
        switch (m)
        {
            case 0:
                return new Vector2(0, 0);
            case 1:
                return new Vector2(0, 1);
            case 2:
                return new Vector2(1, 1);
            case 3:
                return new Vector2(1, 0);
        }
        return Vector2.zero;
    }

    //public List<Vector2> uvs = new List<Vector2>();

    public void Clear()
    {
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();
        trianglesco.Clear();
    }

    private void Start()
    {
        Clear();

        geometry = new Mesh();
        collider = new Mesh();

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 0, 1));

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        for (int i = 0; i < 8; i ++)
        {
            if (i % 2 == 0)
            {
                CreateTriangle(new int[3] { round(i, 8), round(i + 1, 8), round(i + 2, 8) });

                CreateTriangle(new int[3] { round(i, 8), round(i + 2, 8), round(i + 3, 8) });
            }
            else
            {
                CreateTriangle(new int[3] { round(i, 8), round(i + 1, 8), round(i + 4, 8) });
            }
        }

        BuildMesh();
    }

    // Start is called before the first frame update
    public void BuildMesh()
    {
        for (int i = uvs.Count; i < vertices.Count; i++)
        {
            uvs.Add(GenerateUV(i));
        }

        geometry.SetVertices(vertices);
        geometry.triangles = triangles.ToArray();
        geometry.uv = uvs.ToArray();

        geometry.RecalculateBounds();
        geometry.RecalculateNormals();
        geometry.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = geometry;

        collider.SetVertices(vertices);
        collider.triangles = trianglesco.ToArray();

        collider.RecalculateBounds();
        collider.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = collider;
    }

    public void AddPoint(Vector3 newPos)
    {
        vertices.Add(newPos);
        points.Add(newPos, vertices.Count - 1);
    }

    public void InitializePoint()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            points.Add(vertices[i], i);
        }
    }

    public int AddPointWithTriangle(Vector3 newPos,int tri)
    {
        int[] others = new int[3] {trianglesco[tri*3], trianglesco[tri * 3+1], trianglesco[tri * 3+2] };
        RemoveTriangles(tri,false);
        int id = vertices.Count;
        AddPoint(newPos);

        CreateTriangle(new int[3] { others[0], others[1], id });
        CreateTriangle(new int[3] { others[0], others[2], id });
        CreateTriangle(new int[3] { others[1], others[2], id });

        BuildMesh();
        return id;
    }

    public void CreateTriangle(Vector3[] vertex)
    {
        if(vertex.Length != 3)
        {
            return;
        }

        int[] index = new int[3];
        points.TryGetValue(vertex[0], out index[0]);
        points.TryGetValue(vertex[1], out index[1]);
        points.TryGetValue(vertex[2], out index[2]);

        CreateTriangle(index);
    }

    public void CreateTriangle(int[] index)
    {
        if (index.Length != 3)
        {
            return;
        }

        triangleBound.Add(1);
        triangleBound.Add(-1);
        trianglesco.Add(index[0]);
        trianglesco.Add(index[1]);
        trianglesco.Add(index[2]);

        //Backward
        trianglesco.Add(index[0]);
        trianglesco.Add(index[2]);
        trianglesco.Add(index[1]);

        triangles.Add(index[0]);
        triangles.Add(index[1]);
        triangles.Add(index[2]);
    }

    public void DeleteNode(int index)
    {
        List<int> i = triangles.FindAll(p=>p== trianglesco[index]);

        foreach (int id in i) {
            RemoveTriangles(id/3);
        }
    }

    public void ModifyPoint(int index,Vector3 newPos)
    {
        vertices[index] = newPos;
        BuildMesh();
    }

    public void ModifyPoint(Vector3 oldPos, Vector3 newPos)
    {
        int index;
        points.TryGetValue(oldPos, out index);
        vertices[index] = newPos;
        BuildMesh();
    }

    public void RemoveTriangles(int ti,bool update = true)
    {
        if (triangleBound[ti] == 0)
        {
            RemoveTriangle(ti);
            triangleBound.RemoveAt(ti);
        }
        else
        {
            if (triangleBound[ti] > 0)
            {
                RemoveTriangle(ti + triangleBound[ti]);
                RemoveTriangle(ti);
                triangleBound.RemoveAt(ti + triangleBound[ti]);
                triangleBound.RemoveAt(ti);
            }
            else
            {
                int tem = triangleBound[ti];
                RemoveTriangle(ti);
                RemoveTriangle(ti + tem);
                triangleBound.RemoveAt(ti);
                triangleBound.RemoveAt(ti + tem);
            }
        }
        RemoveMeshTriangle(ti/2);
        if(update)
            BuildMesh();
    }

    private void RemoveTriangle(int ti)
    {
        trianglesco.RemoveAt(ti * 3);
        trianglesco.RemoveAt(ti * 3);
        trianglesco.RemoveAt(ti * 3);
    }

    private void RemoveMeshTriangle(int ti)
    {
        triangles.RemoveAt(ti * 3);
        triangles.RemoveAt(ti * 3);
        triangles.RemoveAt(ti * 3);
    }
}
