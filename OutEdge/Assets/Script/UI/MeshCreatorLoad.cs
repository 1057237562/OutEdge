using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TerrainManager;

public class MeshCreatorLoad : EntityBase
{
    public static Transform meshcreator;

    private void Start()
    {
        GetComponent<GuiObject>().interact.Add(delegate { LoadMeshCreateScene(); });

        RefreshChunkImplement();
    }

    private void OnDestroy()
    {
        if (lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
        }
    }

    public void LoadMeshCreateScene()
    {
        meshcreator = transform;
        SceneManager.LoadScene("MeshCreator", LoadSceneMode.Additive);
    }
}
