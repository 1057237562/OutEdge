using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEngine.Object;
using static TerrainManager;
using OutEdge;
using Unity.Entities;
using EntityManager = OutEdge.EntityManager;
using Unity.Mathematics;
using Unity.Transforms;

public class MarchingStack
{
    public MarchingObject[] marchingObj = new MarchingObject[loadHeight];
    public Vector2 pos;
    public System.Random localRandomize;
    public int richness = 10;

    List<OreManager.StructDictionary> localStructure = new List<OreManager.StructDictionary>();

    Dictionary<GameObject, EntityManager.EntityDictionary> localEntities = new Dictionary<GameObject, EntityManager.EntityDictionary>();

    Dictionary<Entity,int> localPlants = new Dictionary<Entity, int>();

    public MarchingStack(Vector2 p)
    {
        pos = p;
        localRandomize = new System.Random(pos.GetHashCode() + tm.globalGeneratingCode);
    }

    public float GetHeight(int x,int z)
    {
        if(heightMap != null)
        {
            return heightMap[x, z];
        }
        else
        {
            GenerateHeightMap();
            return heightMap[x, z];
        }
    }

    float[,] heightMap;

    public void GenerateHeightMap()
    {
        heightMap = new float[size, size];
        int[,] terrain = tm.vg.GenerateMap((int)pos.x/4 - 1,(int)pos.y/4 - 1,size/4+3,size/4+3);

        TerrainType[,] types = new TerrainType[size/4 + 1, size/4 + 1];

        for (int i = 0; i < size/4 + 1; i++)
        {
            for (int j = 0; j < size/4 + 1; j++)
            {
                TerrainType c = new TerrainType(0,0);
                for (int k = -1;k <= 1; k++)
                {
                    for (int l = -1;l <= 1; l++)
                    {
                        c += tm.terrainTypes[terrain[k + i + 1, l + j + 1]];
                    }
                }
                c *= 1 / 9f;
                types[i, j] = c;
            }
        }

        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                int tarx = (int)(pos.x + i);
                int tarz = (int)(pos.y + j);
                //return 48 * Mathf.PerlinNoise(tarx / 64f, tarz / 64f) + 48;
                float u = tarx / 4f - Mathf.Floor(tarx / 4f);
                float v = tarz / 4f - Mathf.Floor(tarz / 4f);
                TerrainType terrainType = (1 - u)*(1 - v)*types[(int)Mathf.Floor(i/4f),(int)Mathf.Floor(j/4f)] + (1 - u) * v * types[(int)Mathf.Floor(i / 4f), (int)Mathf.Ceil(j / 4f)]+ u * (1 - v) * types[(int)Mathf.Ceil(i / 4f), (int)Mathf.Floor(j / 4f)] + u * v * types[(int)Mathf.Ceil(i / 4f), (int)Mathf.Ceil(j / 4f)];
                heightMap[i,j] = 2 * terrainType.lambada * tm.ngp.getValue(tarx / 32f, tarz / 32f) + 48 + terrainType.gamma;
            }
        }
    }

    public void Load(int start)
    {
        for (int i = 0; i < loadHeight; i++)
        {
            GameObject c = Instantiate(tm.chunk, new Vector3(pos.x, (start + i - loadHeight/2)*size, pos.y), Quaternion.identity);
            int index = Shrink(start + i - loadHeight / 2,loadHeight);
            marchingObj[index] = c.GetComponent<MarchingObject>();
            marchingObj[index].parent = this;
            marchingObj[index].LoadChunk(Floor(c.transform.position.x / size), Floor(c.transform.position.z / size), Floor(c.transform.position.y / size),true);
        }
        Thread td = new Thread(() => {
            GenerateHeightMap();
            //LoadEntity();
            LoadStructure();
        });
        td.Start();
    }

    public void UnLoad()
    {
        Save();
        if (localStructure.Count != 0)
        {
            foreach (OreManager.StructDictionary structDictionary in localStructure)
            {
                Destroy(structDictionary.prefab);
            }
        }

        if (localEntities.Count != 0)
        {
            foreach (EntityManager.EntityDictionary entityDictionary in localEntities.Values)
            {
               Destroy(entityDictionary.prefab);
            }
        }
        if(localPlants.Count != 0)
        {
            foreach(Entity plant in localPlants.Keys)
            {
                DynamicBuffer<Child> childs = World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<Child>(plant);

                foreach (Child c in childs)
                {
                    World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(c.Value);
                }
                World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(plant);
            }
        }
        localEntities.Clear();
        localPlants.Clear();
        localStructure.Clear();
    }

    public void Relocate(Vector3 position)
    {
        foreach(MarchingObject marchingObject in marchingObj)
        {
            //marchingObject.transform.Translate(0,-256,0);
            marchingObject.GetComponent<MeshRenderer>().enabled = false;
        }
        heightMap = null;
        Thread thread = new Thread(() => {
            pos = new Vector2(position.x, position.z);
            GenerateHeightMap();
            for (int i = 0; i < loadHeight; i++)
            {
                int index = Shrink(Floor(i - loadHeight / 2 + position.y / size), loadHeight);
                marchingObj[index].Unload();
                marchingObj[index].LoadChunk(Floor(position.x / size), Floor(position.z / size), Floor(i - loadHeight / 2 + position.y / size), false);
            }
            //LoadEntity();
            LoadStructure();
        });
        thread.Start();
    }

    public void RemoveEntity(GameObject prefab)
    {
        try
        {
            localEntities.Remove(prefab);
        }
        catch { }
    }

    public void AddEntity(GameObject prefab)
    {
        if (prefab.GetComponent<CenterObject>() != null)
        {
            localEntities.Add(prefab, new EntityManager.EntityDictionary(prefab, 19));
        }
        else
        {
            try
            {
                EntityBase eb = prefab.GetComponent<EntityBase>();
                localEntities.Add(prefab, new EntityManager.EntityDictionary(prefab, eb.id));
            }
            catch { }
        }
    }

    public void AddEntity(Entity entity,int id)
    {
         localPlants.Add(entity, id);
    }

    public GameObject GenerateMobs(GameObject en)
    {
        int xRand = localRandomize.Next(size);
        int zRand = localRandomize.Next(size);

        float y = GetHeight(xRand,zRand);

        try
        {
            en.GetComponent<ItemBase>().summon = true;
        }
        catch { }

        return Instantiate(en, new Vector3(pos.x + xRand,y+en.transform.lossyScale.y/2, pos.y + zRand), Quaternion.identity);
    }

    /*public void FixedUpdate()
    {
        if (Math.Abs((TerrainManager.tm.follower.transform.position - gpos).x) > TerrainManager.tm.Range*size || Math.Abs((TerrainManager.tm.follower.transform.position - gpos).z) > TerrainManager.tm.Range*size)
            UnLoad();
    }*/

    public void NatureTick()
    {
        int total = 0;
        int count = 0;

        Vector2 p = new Vector2(pos.x / size, pos.y / size);
        try
        {
            total += richness;
            count++;
        }
        catch { }
        try
        {
            total += tm.GetChunkD(p + new Vector2(1, 0)).richness;
            count++;
        }
        catch { }
        try
        {
            total += tm.GetChunkD(p + new Vector2(-1, 0)).richness;
            count++;
        }
        catch { }
        try
        {
            total += tm.GetChunkD(p + new Vector2(0, 1)).richness;
            count++;
        }
        catch { }
        try
        {
            total += tm.GetChunkD(p + new Vector2(0, -1)).richness;
            count++;
        }
        catch { }

        int delta = total / count - localEntities.Count;

        if (delta > 0)
        {
            int times = localRandomize.Next(Math.Min(localEntities.Keys.Where(x => x.GetComponent<AnimalAI>() != null).Count() / 2, delta));
            for (int i = 0; i < times; i++)
            {
                RandomBreed();
            }
        }

        richness += delta;
    }

    [ContextMenu("ForceBreed")]
    public void RandomBreed()
    {
        IEnumerable<GameObject> animals = localEntities.Keys.Where(x => x.GetComponent<AnimalAI>() != null);
        if (animals.Count() > 1)
        {
            GameObject target = animals.ElementAt(localRandomize.Next(animals.Count() - 1));
            IEnumerable<GameObject> partners = localEntities.Keys.Where(x => { EntityManager.EntityDictionary tdict; localEntities.TryGetValue(target, out tdict); EntityManager.EntityDictionary dict; localEntities.TryGetValue(x, out dict); return tdict.id == dict.id && x != target; });
            if (partners.Count() > 1)
            {
                GameObject partner = partners.ElementAt(localRandomize.Next(partners.Count() - 1));

                target.GetComponent<AnimalAI>().BreedWith(partner);
            }
        }
    }

    public void Save()
    {
        List<StaticStruct> statics = new List<StaticStruct>();

        foreach (OreManager.StructDictionary structDictionary in localStructure)
        {
            if (structDictionary.prefab != null)
            {
                Vector3 pos = structDictionary.prefab.transform.position;
                Quaternion rot = structDictionary.prefab.transform.rotation;
                statics.Add(new StaticStruct(structDictionary.id, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w));
            }
        }

        Thread thread2 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
        thread2.Start(new FileSystem.Values(statics, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + ".str"));

        List<StaticStruct> entities = new List<StaticStruct>();

        foreach (EntityManager.EntityDictionary entityDictionary in localEntities.Values)
        {
            if (entityDictionary.prefab != null)
            {
                Vector3 pos = entityDictionary.prefab.transform.position;
                Quaternion rot = entityDictionary.prefab.transform.rotation;
                if (entityDictionary.prefab.GetComponent<CenterObject>() != null)
                {
                    entities.Add(new StaticStruct(entityDictionary.id, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w, entityDictionary.prefab.GetComponent<CenterObject>().id.ToString()));
                    entityDictionary.prefab.GetComponent<CenterObject>().Save();
                }
                else if (entityDictionary.prefab.GetComponent<DataEntity>() != null)
                {
                    entities.Add(new StaticStruct(entityDictionary.id, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w, entityDictionary.prefab.GetComponent<DataEntity>().save()));
                }
                else if (entityDictionary.prefab.GetComponent<StructureEntity>() != null)
                {
                    entities.Add(new StaticStruct(entityDictionary.id, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w, entityDictionary.prefab.GetComponent<StructureEntity>().save()));
                }
                else
                {
                    entities.Add(new StaticStruct(entityDictionary.id, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w));
                }
            }
        }

        if (localPlants.Count != 0)
        {
            foreach (KeyValuePair<Entity, int> plant in localPlants)
            {
                IGrowable data = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IGrowable>(plant.Key);
                float3 pos = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(plant.Key).Value;
                float4 rot = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Rotation>(plant.Key).Value.value;
                entities.Add(new StaticStruct(plant.Value, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w, data.ToString()));
            }
        }
        Thread thread3 = new Thread(new ParameterizedThreadStart(FileSystem.SaveData));
        thread3.Start(new FileSystem.Values(entities, Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + ".ent"));
        foreach (MarchingObject obj in marchingObj)
        {
            obj.Save();
        }
    }

    List<StaticStruct> entities;
    List<StaticStruct> statics;

    public void LoadStructure()
    {
        if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size)+ "-" + (int)(pos.y / size) + ".str"))
        {
            statics = FileSystem.DeserializeFromFile<List<StaticStruct>>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + ".str");
        }
        else
        {
            GenerateStructure();
        }
    }

    public void InstantiateES(float minY,float maxY)
    {
        foreach (StaticStruct staticStruct in statics)
        {
            if (staticStruct.y < minY || staticStruct.y > maxY)
            {
                continue;
            }
            localStructure.Add(new OreManager.StructDictionary(Instantiate(OreManager.om.treeGenerators[staticStruct.id].structDictionary.prefab, new Vector3(staticStruct.x, staticStruct.y, staticStruct.z), Quaternion.identity), 0, staticStruct.id));
        }

        foreach (StaticStruct entity in entities)
        {
            if(entity.y < minY || entity.y > maxY)
            {
                continue;
            }
            if (entity.id < 0)
            {
                ItemManager.SummonItem(new Vector3(entity.x, entity.y, entity.z), new ItemManager.Item(-entity.id - 1, 0, "")).transform.rotation = new Quaternion(entity.rotx, entity.roty, entity.rotz, entity.rotw);
            }
            else if (entity.id == 19)
            {
                GameObject center = FileSystem.GenerateGameObject(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/items/" + Math.Abs(int.Parse(entity.data)) + ".oeg", new Vector3(entity.x, entity.y, entity.z), new Quaternion(entity.rotx, entity.roty, entity.rotz, entity.rotw), true);
                center.GetComponent<CenterObject>().Regist();
            }
            else
            {
                GameObject prefab = Instantiate(EntityManager.em.entities[entity.id].prefab, new Vector3(entity.x, entity.y, entity.z), new Quaternion(entity.rotx, entity.roty, entity.rotz, entity.rotw));
                Growable growable = prefab.GetComponent<Growable>();
                if (growable != null)
                {
                    string[] data = entity.data.Split(':');
                    growable.Restore(float.Parse(data[0]), float.Parse(data[1]));
                }
                else
                {
                    DataEntity de = prefab.GetComponent<DataEntity>();
                    EntityBase eb;
                    if (de != null) // trash codes
                    {
                        eb = de;
                        de.load(entity.data);
                    }
                    else
                    {
                        StructureEntity se = prefab.GetComponent<StructureEntity>();
                        if (se != null)
                        {
                            eb = se;
                            se.load(entity.data);
                        }
                        eb = prefab.GetComponent<EntityBase>();
                    }
                    eb.enabled = true;
                    eb.id = entity.id;
                    eb.LoadEntity();

                    Collider[] co = prefab.GetComponents<Collider>();
                    foreach (Collider c in co)
                    {
                        c.isTrigger = false;
                    }

                    prefab.layer = 0;
                    foreach (Transform child in prefab.transform)
                    {
                        if (child.gameObject.layer == 8) child.gameObject.layer = 0;
                        Collider c = child.GetComponent<Collider>();
                        if (c)
                        {
                            c.enabled = true; c.isTrigger = false;
                        }
                    }
                    try
                    {
                        prefab.GetComponent<EntityBase>().enabled = true;
                    }
                    catch { }
                    prefab.GetComponent<EntityBase>().lastChunk = this;
                    localEntities.Add(prefab, new EntityManager.EntityDictionary(prefab, entity.id));
                }
            }
        }
    }

    public void LoadEntity()
    {
        if (File.Exists(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + ".ent"))
        {
            entities = FileSystem.DeserializeFromFile<List<StaticStruct>>(Environment.CurrentDirectory + "/saves/" + StartGame.savePath + "/chunks/" + (int)(pos.x / size) + "-" + (int)(pos.y / size) + ".ent");
        }
        else
        {
            GenerateLivingEntity();
        }
    }

    public void GenerateLivingEntity()
    {
        entities = new List<StaticStruct>();
        foreach (EntityManager.EntityGen ed in EntityManager.em.genbase)
        {
            if (localRandomize.Next(ed.chance) == 0)
            {
                int count = localRandomize.Next(ed.mincount, ed.maxcount);
                for (int i = 0; i < count; i++)
                {
                    int xRand = localRandomize.Next(size);
                    int zRand = localRandomize.Next(size);

                    float y = GetHeight(xRand,zRand);

                    /*GameObject prefab = Instantiate(ed.prefab, new Vector3(pos.x + xRand,y + 3, pos.y + zRand), Quaternion.identity);
                    EntityBase eb = prefab.GetComponent<EntityBase>();
                    eb.id = ed.id;
                    eb.LoadEntity();
                    prefab.GetComponent<EntityBase>().lastChunk = this;*/
                    entities.Add(new StaticStruct(ed.id, pos.x + xRand,y,pos.y + zRand,0,0,0,0));
                }
            }
        }
    }

    public void GenerateStructure()
    {
        statics = new List<StaticStruct>();
        for (int j = 0; j < OreManager.om.treeGenerators.Length; j++)
        {
            OreManager.TreeGenerator tree = OreManager.om.treeGenerators[j];
            for (int i = 0; i < tree.structDictionary.chance; i++)
            {
                int xRand = localRandomize.Next(size);
                int zRand = localRandomize.Next(size);

                float y = GetHeight(xRand,zRand);
                float moist = Mathf.Clamp(2*tm.mg.getValue(xRand/16f, zRand/16f)+32,0,64); // 0.45≈32/70
                if(tree.minAltitude > y || tree.maxAltitude < y)
                {
                    continue;
                }
                if(tree.minMoist > moist || tree.maxMoist < moist)
                {
                    continue;
                }
                
                int id = j;//Instantiate(tree.structDictionary.prefab, new Vector3(xRand + pos.x, y, zRand + pos.y), Quaternion.identity)
                statics.Add(new StaticStruct(id,pos.x + xRand,y-1,pos.y + zRand,0,0,0,0));
            }
        }
    }

    [Serializable]
    public class StaticStruct
    {
        public int id;
        public float x;
        public float y;
        public float z;
        public float rotx;
        public float roty;
        public float rotz;
        public float rotw;
        public string data;

        public StaticStruct(int Id, float posx, float posy, float posz, float rx, float ry, float rz, float rw)
        {
            id = Id;
            x = posx;
            y = posy;
            z = posz;
            rotx = rx;
            roty = ry;
            rotz = rz;
            rotw = rw;
        }
        public StaticStruct(int Id, float posx, float posy, float posz, float rx, float ry, float rz, float rw, string addition)
        {
            id = Id;
            x = posx;
            y = posy;
            z = posz;
            rotx = rx;
            roty = ry;
            rotz = rz;
            rotw = rw;
            data = addition;
        }
    }
}
