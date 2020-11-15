using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingObject : MonoBehaviour
{

    public MarchingStack parent;

    public virtual void Save()
    {

    }

    public virtual void Unload() { }

    public virtual void LoadChunk(int i,int ii,int iii,bool loaddirect)
    {

    }

    public virtual bool AddBlock(Vector3 pos)
    {
        return false;
    }

    public virtual bool RemoveBlock(Vector3 pos, int v)
    {
        return false;
    }

    public virtual void LoadChunks(MarchingObject[,,] chunkers)
    {

    }
}
