using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class ItemBase : EntityBase
{
    public static bool autopick = false;

    public ItemManager.Item item;
    public int damage = 0;
    public bool summon = false;

    void OnCollisionEnter(Collision collision)
    {
        if (autopick)
        {
            if (collision.collider.gameObject.GetComponent<PlayerEntity>() != null)
            {
                Inventory.m.attemptAddItem(item, 1,damage);
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        if(transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (lastChunk != null)
                lastChunk.RemoveEntity(gameObject);
        if (summon)
        {
            Spawner.spawner.nowRC--;
        }
    }

    private void Start()
    {
        id = -item.id - 1;
        RefreshChunkImplement();
    }
}
