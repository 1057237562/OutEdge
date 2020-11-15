using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class AnimalAI : LivingEntity
{
    public AstarBase astar;

    //Vector2 chunk;

    Wandering wandering;
    Vector3 node;

    public float speed = 3.5f;

    bool breeding = false;

    public bool start = false;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 chunkpos = transform.position / TerrainManager.tm.chunksize;
        //int cx = (int)(chunkpos.x > 0 ? chunkpos.x : chunkpos.x - 1);
        //int cz = (int)(chunkpos.z > 0 ? chunkpos.z : chunkpos.z - 1);

        //chunk = new Vector2(cx, cz);
        wandering = gameObject.AddComponent<Wandering>();
        wandering.speed = speed;
        wandering.RegisterEvent(new ParameterizedThreadStart(OnEntityMove));

        RefreshChunkImplement();
    }

    public void FixedUpdate()
    {
        if (start)
        {
            if (astar != null)
            {
                if (astar.path.Count != 0)
                {
                    wandering.enabled = false;
                    if ((Mathf.Abs(transform.position.x - node.x) < 1.5f && Mathf.Abs(transform.position.y - node.y) < 2f && Mathf.Abs(transform.position.z - node.z) < 1.5f) || node == Vector3.zero)
                    {
                        node = astar.path.Pop();
                    }
                    transform.LookAt(new Vector3(node.x, transform.position.y, node.z));
                    transform.position = Vector3.MoveTowards(transform.position, node + new Vector3(0, 1f, 0.0f), speed * Time.deltaTime);
                }
                else
                {
                    wandering.enabled = true;
                    astar = null;
                    node = Vector3.zero;
                    if (breeding)
                    {
                        GameObject prefab = Instantiate(gameObject, transform.position + new Vector3(0, 3f, 0), Quaternion.identity);
                        prefab.GetComponent<EntityBase>().RefreshChunkImplement();
                        breeding = false;
                    }
                }
            }
        }
        //base.FixedUpdate();
    }

    public void OnEntityMove(object position)
    {
        /*Vector3 chunkpos = (Vector3)position / TerrainManager.tm.chunksize;
        int cx = (int)(chunkpos.x > 0 ? chunkpos.x : chunkpos.x - 1);
        int cz = (int)(chunkpos.z > 0 ? chunkpos.z : chunkpos.z - 1);

        Vector2 newchunkpos = new Vector2(cx, cz);

        if(newchunkpos == chunk)
        {
            return;
        }*/

        //Debug.LogWarning(chunk - TerrainManager.tm.loadedArea.position + new Vector2(TerrainManager.tm.Range, TerrainManager.tm.Range));
        //MarchingCubes oldchunk = TerrainManager.tm.GetChunk(chunk - TerrainManager.tm.loadedArea.position + new Vector2(TerrainManager.tm.Range, TerrainManager.tm.Range));

        //Debug.LogWarning(newchunkpos - TerrainManager.tm.loadedArea.position + new Vector2(TerrainManager.tm.Range, TerrainManager.tm.Range));
        try
        {
            //MarchingCubes newchunk = TerrainManager.tm.GetChunk(newchunkpos - TerrainManager.tm.loadedArea.position + new Vector2(TerrainManager.tm.Range, TerrainManager.tm.Range));

            Loom.QueueOnMainThread(new Action(() => { RefreshChunkImplement(); }));
        }
        catch
        {
            Loom.QueueOnMainThread(new Action(() => { Destroy(gameObject); }));
        }

        //chunk = newchunkpos;
    }

    public void BreedWith(GameObject partner)
    {
        //TODO: Breeding
        if (astar != null)
        {
            astar = null;
        }
        AnimalAI pai = partner.GetComponent<AnimalAI>();
        astar = new AstarBase(transform.position, partner.transform.position);
        Thread thread = new Thread(new ThreadStart(() =>
        {
            List<Vector3> nodes = astar.SearchPathToList();
            int half = nodes.Count / 2;

            astar.path = new Stack<Vector3>();
            for (int i = 0; i < half; i++)
            {
                astar.path.Push(nodes[i]);
            }
            start = true;

            if (pai.astar == null)
            {
                pai.astar = new AstarBase();
                pai.astar.path = new Stack<Vector3>();
                for (int i = nodes.Count - 1; i > half; i--)
                {
                    pai.astar.path.Push(nodes[i]);
                }
            }
            pai.start = true;
            breeding = true;
        }));
        thread.Start();

    }
}
