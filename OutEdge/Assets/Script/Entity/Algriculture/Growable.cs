using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using static TerrainManager;

public class Growable : MonoBehaviour , IConvertGameObjectToEntity
{
    public int waterCost;
    public int grownTime;
    public int randomBiasRange;
    //public List<ItemStack> baseOutput;

    public int curvetype;
    public int ResultType;
    public bool seperated;

    public int id;

    bool restore = false;
    float startTime;
    float matureTime;
    
    public void Restore(float start,float mature)
    {
        restore = true;
        startTime = start;
        matureTime = mature;
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        IGrowable data;
        if (restore)
        {
            data = new IGrowable { startTime = startTime, matureTime = matureTime, curve = curvetype, ResultType = ResultType, seperated = seperated, grownTime = grownTime, randomBiasRange = randomBiasRange };
        }
        else
        {
            data = new IGrowable { startTime = TimeManager.GetCurrentPlayTime(), matureTime = TimeManager.GetCurrentPlayTime() + grownTime + UnityEngine.Random.Range(-randomBiasRange, randomBiasRange), curve = curvetype, ResultType = ResultType, seperated = seperated, grownTime = grownTime, randomBiasRange = randomBiasRange };
        }
        tm.GetChunk(tm.GetId(transform.position)).AddEntity(entity,id);
        dstManager.AddComponentData(entity, data);
    }
}
