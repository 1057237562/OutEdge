using System;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;
using static ItemManager;

[RequireComponent(typeof(Collider))]
public class EntityBase : MonoBehaviour
{

    public MarchingStack lastChunk;

    public int id;

    [Serializable]
    public class TileDrop
    {
        public Item item;
        public int max;
        public int min;
    }

    public List<TileDrop> tiledrop;
    public List<ItemStack> mater;

    public void RefreshChunkImplement()
    {
        if (lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
        }
        lastChunk = tm.GetChunk(tm.GetId(transform.position));
        lastChunk.AddEntity(gameObject);
    }

    public virtual void Death()
    {
        foreach (TileDrop td in tiledrop)
        {
            System.Random random = new System.Random();
            int next = random.Next(td.min, td.max);
            for (int i = 0; i < next; i++)
            {
                SummonItem(transform.position + new Vector3(0,(i+1)* im.prefabs[td.item.id].transform.localScale.y,0), td.item);
            }
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (lastChunk != null)
        {
            lastChunk.RemoveEntity(gameObject);
        }
    }

    public virtual void LoadEntity()
    {

    }
}
