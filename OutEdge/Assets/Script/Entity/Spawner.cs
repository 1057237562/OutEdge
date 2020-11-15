using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainManager;

public class Spawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawn
    {
        public GameObject prefab;
        public int min;
        public int max;
        public float chance;

        public bool nocturnal;

    }

    [Serializable]
    public class ResourceSpawn
    {
        public GameObject prefab;
        public int min;
        public int max;
        public float chance;

    }

    public int maxHostileMobCount = 30;
    int nowCount = 0;
    public int minspawnrange = 2;//Included
    public int maxspawnrange = 3;//Included

    public int nowRC = 0;
    public int maxResourceCount = 15;
    public int minrss = 1;//Included
    public int maxrss = 2;//Included
    public int spawnRDuration = 5;

    public int spawnDuration = 20;

    System.Random localRandom = new System.Random();

    public List<EnemySpawn> enemies = new List<EnemySpawn>();
    public List<ResourceSpawn> resource = new List<ResourceSpawn>();

    public static Spawner spawner;
    
    private void Start()
    {
        spawner = this;
    }

    private void FixedUpdate()
    {
        if ((Time.fixedTime - spawnDuration + 1) % spawnDuration == 0)
        {

            if (nowCount < (int)(maxHostileMobCount * (DayNight.night ? 1 : 0.2f)))
            {
                foreach (EnemySpawn enemy in enemies)
                {
                    if (enemy.nocturnal && !DayNight.night)
                    {
                        continue;
                    }
                    for (int i = -maxspawnrange; i < maxspawnrange + 1; i++)
                    {
                        for (int j = -maxspawnrange; j < maxspawnrange + 1; j++)
                        {
                            if (Math.Abs(i) < minspawnrange && Math.Abs(j) < minspawnrange)
                            {
                                continue;
                            }
                            if (localRandom.NextDouble() < enemy.chance)
                            {
                                int count = localRandom.Next(enemy.min, enemy.max);
                                for (int z = 0; z < count; z++)
                                {
                                    if (nowCount < (int)(maxHostileMobCount * (DayNight.night ? 1 : 0.2f)))
                                    {
                                        tm.chunks[i + range][j + range].GenerateMobs(enemy.prefab);
                                        nowCount++;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            //TODO : Summon Animals and Rechunk
            for (int i = -range; i < range + 1; i++)
            {
                for (int j = -range; j < range + 1; j++)
                {
                    tm.chunks[i + range][j + range].NatureTick();
                }

            }

            GC.Collect();
        }

        if ((Time.fixedTime - spawnRDuration + 1) % spawnRDuration == 0 && nowRC < maxResourceCount)
        {
            foreach (ResourceSpawn rs in resource)
            {
                for (int i = -maxrss; i < maxrss + 1; i++)
                {
                    for (int j = -maxrss; j < maxrss + 1; j++)
                    {
                        if (Math.Abs(i) < minrss && Math.Abs(j) < minrss)
                        {
                            continue;
                        }
                        if (localRandom.NextDouble() < rs.chance)
                        {
                            int count = localRandom.Next(rs.min, rs.max);
                            for (int z = 0; z < count; z++)
                            {
                                if (nowRC < maxResourceCount)
                                {
                                    tm.chunks[i + range][j + range].GenerateMobs(rs.prefab).AddComponent<ResourceItem>();
                                    nowRC++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
