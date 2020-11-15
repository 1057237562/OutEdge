using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;

public class DecorationEntity : EntityBase
{
    public virtual void Start()
    {
        RefreshChunkImplement();
    }

    public virtual void OnDestroy()
    {
        if(lastChunk != null)
            lastChunk.RemoveEntity(gameObject);
    }

    public virtual void FixedUpdate()
    {
        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }
}
