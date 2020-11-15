using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeMesh : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float _quality = 0.5f;
    MeshFilter _renderer;
    Mesh _mesh;
    void Start()
    {
        _renderer = GetComponent<MeshFilter>();
        _mesh = _renderer.sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DecimateMesh();
        }
    }

    public void DecimateMesh()
    {
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(_mesh);
        meshSimplifier.SimplifyMesh(_quality);
        var destMesh = meshSimplifier.ToMesh();
        _renderer.sharedMesh = destMesh;
    }
}
